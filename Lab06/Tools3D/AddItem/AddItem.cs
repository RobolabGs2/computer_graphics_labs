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
            Item.AddButton(tab.AddButton(Properties.Resources.Icosahedron, true), GenerateIcosa, context);
            Item.AddButton(tab.AddButton(Properties.Resources.Dodecahedron, true), GenerateDodeca, context);

            MultiItem<Spline, Spline>.AddButton(tab.AddButton(Properties.Resources.Spline), 
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
            cube.Add(new Polygon(new int[] { 0, 4, 5, 1 }));
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

            tetra.Add(new Polygon(new int[] { 0, 2, 1 }));
            tetra.Add(new Polygon(new int[] { 0, 1, 3 }));
            tetra.Add(new Polygon(new int[] { 0, 3, 2 }));
            tetra.Add(new Polygon(new int[] { 1, 2, 3 }));

            return tetra;
        }


        Polytope GenerateOcta()
        {
            Polytope cube = GenerateCube();
            Polytope octa = new Polytope();

            foreach(var edge in cube.polygons)
            {
                var sump = new Point();
                foreach (var p in edge.Points(cube))
                    sump += p;
                sump /= 4;
                octa.Add(new Point { X = sump.X, Y = sump.Y, Z = sump.Z });
            }

            octa.Add(new Polygon(new int[] { 0, 5, 1 }));
            octa.Add(new Polygon(new int[] { 2, 1, 5 }));

            octa.Add(new Polygon(new int[] { 0, 3, 5 }));
            octa.Add(new Polygon(new int[] { 2, 5, 3 }));

            octa.Add(new Polygon(new int[] { 0, 4, 3 }));
            octa.Add(new Polygon(new int[] { 2, 3, 4 }));

            octa.Add(new Polygon(new int[] { 0, 1, 4 }));
            octa.Add(new Polygon(new int[] { 2, 4, 1 }));


            return octa;
        }

        private Point nextCirclePoint(double z, double angle)
        {
            return new Point { X = Math.Sin(Math.PI * angle / 180), Y = Math.Cos(Math.PI * angle / 180), Z = z };
        }
        Polytope GenerateIcosa()
        {
            Polytope icosa = new Polytope();

            for (int angle = 0; angle < 360; angle += 72)
            {
                icosa.Add(nextCirclePoint(0.5, angle));
                icosa.Add(nextCirclePoint(-0.5, angle + 36));
            }

            icosa.Add(new Base3D.Point { X = 0, Y = 0, Z = Math.Sqrt(5) / 2 });
            icosa.Add(new Base3D.Point { X = 0, Y = 0, Z = -Math.Sqrt(5) / 2 });
            //конусы снизу и сверху и треугольнки внутри
            for (int i = 0; i < 10; i++)
            {
                if (i % 2 == 0)
                {
                    icosa.Add(new Polygon(new int[] { i, 10, (i + 2) % 10 }));
                    icosa.Add(new Polygon(new int[] { (i + 2) % 10, (i + 1) % 10, i }));
                }
                else
                {
                    icosa.Add(new Polygon(new int[] { (i + 2) % 10, 11, i }));
                    icosa.Add(new Polygon(new int[] { i, (i + 1) % 10, (i + 2) % 10}));
                }   


            }
            return icosa;
        }

        Polytope GenerateDodeca()
        {
            Polytope icosa = GenerateIcosa();
            Polytope dodeca = new Polytope();

            foreach (var edge in icosa.polygons)
                 dodeca.Add(new Point { X = edge.Points(icosa).Sum(p => p.X) / 3, Y = edge.Points(icosa).Sum(p => p.Y) / 3, Z = edge.Points(icosa).Sum(p => p.Z) / 3 });

            dodeca.Add(new Polygon(new int[] { 16, 12, 8, 4, 0 }));
            for (int i = 0, j = 3; i < dodeca.points.Count; i += 4, j += 4)
            {
                    dodeca.Add(new Polygon(new int[] { (i + 4) % 20, (i + 5) % 20, i + 3, i + 1, i }));
                    dodeca.Add(new Polygon(new int[] { j, j - 1, (20 + j - 4) % 20 - 1, (20 + j - 4) % 20, i + 1 }));
            }
            dodeca.Add(new Polygon(new int[] { 2, 6, 10, 14, 18 }));
            
            return dodeca;
        }
    }
}
