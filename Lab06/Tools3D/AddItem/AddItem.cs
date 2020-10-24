using Lab06.Base3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab06.Tools3D.AddItem
{
    class AddItem : IToolPage
    {
        Context context;
        public Bitmap Image => Properties.Resources.Cube;

        public void Init(ToolTab tab, Context context)
        {
            this.context = context;

            var pontButton = tab.AddButton(Properties.Resources.Point, true);


            pontButton.ButtonClick += t => {
                context.pictureBox.MouseClick += AddPoint;
                context.pictureBox.MouseMove += MovePoint;
            };

            pontButton.ButtonDisable += t => {
                context.pictureBox.MouseClick -= AddPoint;
                context.pictureBox.MouseMove -= MovePoint;
            };

            pontButton.ButtonDisable += b => context.Redraw();

            var cubeButton = tab.AddButton(Properties.Resources.Cube, true);

            cubeButton.ButtonClick += t => {
                context.pictureBox.MouseClick += AddCube;
                context.pictureBox.MouseMove += MoveCube;
            };

            cubeButton.ButtonDisable += t => {
                context.pictureBox.MouseClick -= AddCube;
                context.pictureBox.MouseMove -= MoveCube;
            };

            cubeButton.ButtonDisable += b => context.Redraw();

            var tetraButton = tab.AddButton(Properties.Resources.Tetrahedron, true);

            tetraButton.ButtonClick += t => {
                context.pictureBox.MouseClick += AddTetra;
                context.pictureBox.MouseMove += MoveTetra;
            };

            tetraButton.ButtonDisable += t => {
                context.pictureBox.MouseClick -= AddTetra;
                context.pictureBox.MouseMove -= MoveTetra;
            };

            tetraButton.ButtonDisable += b => context.Redraw();

            var octaButton = tab.AddButton(Properties.Resources.Octahedron, true);

            octaButton.ButtonClick += t => {
                context.pictureBox.MouseClick += AddOcta;
                context.pictureBox.MouseMove += MoveOcta;
            };

            octaButton.ButtonDisable += t => {
                context.pictureBox.MouseClick -= AddOcta;
                context.pictureBox.MouseMove -= MoveOcta;
            };

            octaButton.ButtonDisable += b => context.Redraw();

            /*var icosaButton = tab.AddButton(Properties.Resources.Tetrahedron, true);

            icosaButton.ButtonClick += t => {
                context.pictureBox.MouseClick += AddIcosa;
                context.pictureBox.MouseMove += MoveIcosa;
            };

            icosaButton.ButtonDisable += t => {
                context.pictureBox.MouseClick -= AddIcosa;
                context.pictureBox.MouseMove -= MoveIcosa;
            };

            icosaButton.ButtonDisable += b => context.Redraw();*/
        }

        private void AddPoint(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            var point = context.ScreenToXY(e.X, e.Y);
            if (point.front)
            {
                Spline spline = new Spline();
                spline.Add(point.p);
                context.world.entities.Add(spline);
                context.world.selected.Clear();
                context.world.selected.Add(spline);
                context.Redraw();
            }
        }

        private void MovePoint(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.None)
                return;
            var point = context.ScreenToXY(e.X, e.Y);
            if (point.front)
            {
                Spline spline = new Spline();
                spline.Add(point.p);
                context.world.entities.Add(spline);
                context.Redraw();
                context.world.entities.Remove(spline);
            }
        }

        private void AddCube(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            var point = context.ScreenToXY(e.X, e.Y);
            if (point.front)
            {
                var cube = GenerateCube(point.p);
                context.world.entities.Add(cube);
                context.world.selected.Clear();
                context.world.selected.Add(cube);
                context.Redraw();
            }
        }



        private void MoveCube(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.None)
                return;
            var point = context.ScreenToXY(e.X, e.Y);
            if (point.front)
            {
                var cube = GenerateCube(point.p);
                context.world.entities.Add(cube);
                context.Redraw();
                context.world.entities.Remove(cube);
            }
        }

        Polytope GenerateCube(Base3D.Point location)
        {
            Matrix move = Matrix.Move(location);
            Polytope cube = new Polytope();
            cube.Add(new Base3D.Point { X = 1, Y = 1, Z = 1 } * move);
            cube.Add(new Base3D.Point { X = -1, Y = 1, Z = 1 } * move);
            cube.Add(new Base3D.Point { X = -1, Y = -1, Z = 1 } * move);
            cube.Add(new Base3D.Point { X = 1, Y = -1, Z = 1 } * move);
            cube.Add(new Base3D.Point { X = 1, Y = 1, Z = -1 } * move);
            cube.Add(new Base3D.Point { X = -1, Y = 1, Z = -1 } * move);
            cube.Add(new Base3D.Point { X = -1, Y = -1, Z = -1 } * move);
            cube.Add(new Base3D.Point { X = 1, Y = -1, Z = -1 } * move);

            cube.Add(new Polygon(new int[] { 0, 1, 2, 3 }));
            cube.Add(new Polygon(new int[] { 4, 0, 3, 7 }));
            cube.Add(new Polygon(new int[] { 5, 4, 7, 6 }));
            cube.Add(new Polygon(new int[] { 1, 5, 6, 2 }));
            cube.Add(new Polygon(new int[] { 0, 1, 5, 4 }));
            cube.Add(new Polygon(new int[] { 3, 2, 6, 7 }));

            return cube;
        }

        private void AddTetra(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            var point = context.ScreenToXY(e.X, e.Y);
            if (point.front)
            {
                var tetra = GenerateTetra(point.p);
                context.world.entities.Add(tetra);
                context.world.selected.Clear();
                context.world.selected.Add(tetra);
                context.Redraw();
            }
        }

        private void MoveTetra(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.None)
                return;
            var point = context.ScreenToXY(e.X, e.Y);
            if (point.front)
            {
                var tetra = GenerateTetra(point.p);
                context.world.entities.Add(tetra);
                context.Redraw();
                context.world.entities.Remove(tetra);
            }
        }

        Polytope GenerateTetra(Base3D.Point location)
        {
            Matrix move = Matrix.Move(location);
            Polytope cube = GenerateCube(location);
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

        private void AddOcta(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            var point = context.ScreenToXY(e.X, e.Y);
            if (point.front)
            {
                var octa = GenerateOcta(point.p);
                context.world.entities.Add(octa);
                context.world.selected.Clear();
                context.world.selected.Add(octa);
                context.Redraw();
            }
        }

        private void MoveOcta(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.None)
                return;
            var point = context.ScreenToXY(e.X, e.Y);
            if (point.front)
            {
                var octa = GenerateOcta(point.p);
                context.world.entities.Add(octa);
                context.Redraw();
                context.world.entities.Remove(octa);
            }
        }

        Polytope GenerateOcta(Base3D.Point location)
        {
            Matrix move = Matrix.Move(location);
            Polytope cube = GenerateCube(location);
            Polytope octa = new Polytope();

            foreach(var edge in cube.polygons)
            {
                var sump = new Base3D.Point() * move;
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

        private void AddIcosa(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            var point = context.ScreenToXY(e.X, e.Y);
            if (point.front)
            {
                var tetra = GenerateIcosa(point.p);
                context.world.entities.Add(tetra);
                context.Redraw();
            }
        }

        private void MoveIcosa(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.None)
                return;
            var point = context.ScreenToXY(e.X, e.Y);
            if (point.front)
            {
                var tetra = GenerateIcosa(point.p);
                context.world.entities.Add(tetra);
                context.Redraw();
                context.world.entities.Remove(tetra);
            }
        }

        private Base3D.Point nextCirclePoint(double x, double y,double z, double angle)
        {
            double a = Math.Cos(Math.PI * angle / 180);
            return new Base3D.Point { X = x + Math.Sin(Math.PI * angle / 180), Y = y + Math.Cos(Math.PI * angle / 180),  Z =z };
        }
        Polytope GenerateIcosa(Base3D.Point location)
        {
            Matrix move = Matrix.Move(location);
            Polytope circle = new Polytope();
            Polytope icosa = new Polytope();
            Base3D.Point a = new Base3D.Point { X = 0, Y = 0, Z = 0};

            for (int angle = 0; angle < 360; angle += 3)
            {
                circle.Add(nextCirclePoint(a.X, a.Y, 1, angle) * move);
                //for (int angle = 0; angle < 360; angle += 3)
                circle.Add(nextCirclePoint(a.X, a.Y, -1, angle) * move);
            }
            circle.Add(new Polygon(circle.points.Select((p, i) => i).ToList()));

            icosa.Add(circle.points[circle.points.Count / 20]);

            for (int i = 1; i < 10; i++)
                icosa.Add(circle.points[circle.points.Count / 20 + i * 24]); //120/5

            double iz = 1 + Math.Sqrt(5) / 2;
            icosa.Add(new Base3D.Point { X = 0, Y = 0, Z = iz } * move);
            icosa.Add(new Base3D.Point { X = 0, Y = 0, Z = -iz } * move);
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
