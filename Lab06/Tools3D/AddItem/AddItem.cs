using Lab06.Base3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Point = Lab06.Base3D.Point;

namespace Lab06.Tools3D.AddItem
{
    class AddItem : IToolPage
    {
        Context context;
        OpenFileDialog openFileDialog = new OpenFileDialog();
        public Bitmap Image => Properties.Resources.Cube;

        string filePath;

        public void Init(ToolTab tab, Context context)
        {
            this.context = context;
            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            openFileDialog.Filter = "obj files (*.obj)|*.obj";
            openFileDialog.RestoreDirectory = true;

            Item.AddButton(tab.AddButton(Properties.Resources.Point, true), GeneratePoint, context);
            Item.AddButton(tab.AddButton(Properties.Resources.Cube, true), GenerateCube, context);
            Item.AddButton(tab.AddButton(Properties.Resources.Octahedron, true), GenerateOcta, context);
            Item.AddButton(tab.AddButton(Properties.Resources.Tetrahedron, true), GenerateTetra, context);
            MultiItem<Spline, Spline>.AddButton(tab.AddButton(Properties.Resources.Mouse), 
                () => new Spline(), 
                (total, partial) => total.Add(partial.points.First()),
                GeneratePoint,
                context
            );
            var load = tab.AddButton(Properties.Resources.Load, true);
            load.ButtonClick += b => ChangeFile();
            Item.AddButton(load, GenerateObj, context);
        }

        void ChangeFile()
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                filePath = openFileDialog.FileName;
        }

        Entity GenerateObj()
        {
            try
            {
                return Obj.Parse(filePath);
            }
            catch
            {
                MessageBox.Show("Не вышло загрузить файл", "Окошко-всплывашка", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return new Group();
            }
        }

        Spline GeneratePoint()
        {
            Spline spline = new Spline();
            spline.Add(new Base3D.Point());
            return spline;
        }

        Polytope GenerateCube()
        {
            Polytope cube = new Polytope();
            cube.Add(new Base3D.Point { X = 1, Y = 1, Z = 1 });
            cube.Add(new Base3D.Point { X = -1, Y = 1, Z = 1 });
            cube.Add(new Base3D.Point { X = -1, Y = -1, Z = 1 });
            cube.Add(new Base3D.Point { X = 1, Y = -1, Z = 1 });
            cube.Add(new Base3D.Point { X = 1, Y = 1, Z = -1 });
            cube.Add(new Base3D.Point { X = -1, Y = 1, Z = -1 });
            cube.Add(new Base3D.Point { X = -1, Y = -1, Z = -1 });
            cube.Add(new Base3D.Point { X = 1, Y = -1, Z = -1 });

            cube.Add(new Polygon(new int[] { 0, 1, 2, 3 }));
            cube.Add(new Polygon(new int[] { 4, 0, 3, 7 }));
            cube.Add(new Polygon(new int[] { 5, 4, 7, 6 }));
            cube.Add(new Polygon(new int[] { 1, 5, 6, 2 }));
            cube.Add(new Polygon(new int[] { 0, 1, 5, 4 }));
            cube.Add(new Polygon(new int[] { 3, 2, 6, 7 }));

            return cube;
        }

        Polytope GenerateTetra()
        {
            Polytope cube = GenerateCube();
            Polytope tetra = new Polytope();

            tetra.Add(cube.points[0]);
            tetra.Add(cube.points[2]);
            tetra.Add(cube.points[5]);
            tetra.Add(cube.points[7]);
            var count = cube.polygons.Count - 2;

            //  TODO: надо пофиксить
            var костыль = new int[]{ 0, 5, 1, 2, -3, 2, 7, 3};
            for (int i = 0; i < count; i+=2)
            {
                tetra.Add(new Polygon(new int[] { костыль[cube.polygons[i].indexes[0]], костыль[cube.polygons[4].indexes[2]], костыль[cube.polygons[count - 1 - i].indexes[3]] }));
                tetra.Add(new Polygon(new int[] { костыль[cube.polygons[i].indexes[0]], костыль[cube.polygons[5].indexes[1]], костыль[cube.polygons[i + 1].indexes[3]] }));
            }

            return tetra;
        }


        Polytope GenerateOcta()
        {
            Polytope cube = GenerateCube();
            Polytope octa = new Polytope();

            foreach(var edge in cube.polygons)
            {
                var sump = new Base3D.Point();
                foreach (var p in edge.Points(cube))
                    sump += p;
                octa.Add(sump / 4);
            }

            octa.Add(new Polygon(new int[] { 0, 1, 2, 3 }));

            for (int i = 0; i < octa.points.Count - 2; i++)
            {
                octa.Add(new Polygon(new int[] { i, (i + 1) % 4, 4 }));
                octa.Add(new Polygon(new int[] { i, 5, (i + 1) % 4 }));
            }

            return octa;
        }

        private Base3D.Point nextCirclePoint(double x, double y,double z, double angle)
        {
            double a = Math.Cos(Math.PI * angle / 180);
            return new Base3D.Point { X = x + Math.Sin(Math.PI * angle / 180), Y = y + Math.Cos(Math.PI * angle / 180),  Z =z };
        }

        Polytope GenerateIcosa()
        {
            Polytope circle = new Polytope();
            Polytope icosa = new Polytope();
            Base3D.Point a = new Base3D.Point { X = 0, Y = 0, Z = 0};

            for (int angle = 0; angle < 360; angle += 3)
            {
                circle.Add(nextCirclePoint(a.X, a.Y, 1, angle));
                //for (int angle = 0; angle < 360; angle += 3)
                circle.Add(nextCirclePoint(a.X, a.Y, -1, angle));
            }
            circle.Add(new Polygon(circle.points.Select((p, i) => i).ToList()));

            icosa.Add(circle.points[circle.points.Count / 20]);

            for (int i = 1; i < 10; i++)
                icosa.Add(circle.points[circle.points.Count / 20 + i * 24]); //120/5

            double iz = 1 + Math.Sqrt(5) / 2;
            icosa.Add(new Base3D.Point { X = 0, Y = 0, Z = iz });
            icosa.Add(new Base3D.Point { X = 0, Y = 0, Z = -iz });
            for (int i = 0; i < 10; i+=5)
            {
                //  Зачем тебе пятигранники, икосаэдр же из треугольничков сделан?
                icosa.Add(new Polygon(new int[] { i, i + 1, i + 2, i + 3, i + 4 })); //пятигранник на окружности
                //шапочка и барабан
                for (int j = 0; j < 5; j++)
                {
                    icosa.Add(new Polygon(new int[] { i + j, (i + j + 1) % 5, i % 5 }));
                    icosa.Add(new Polygon(new int[] { i + j, i + 1, i + 2, i + 3, i + 4 })); 
                }
            }

            return icosa;
        }
    }
}
