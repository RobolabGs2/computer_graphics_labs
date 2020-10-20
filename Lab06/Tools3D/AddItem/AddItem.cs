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
                context.world.entities.Add(point.p);
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
                context.world.entities.Add(point.p);
                context.Redraw();
                context.world.entities.Remove(point.p);
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

            cube.Add(new Polygon(new Base3D.Point[] { cube.points[0], cube.points[1], cube.points[2], cube.points[3] }));
            cube.Add(new Polygon(new Base3D.Point[] { cube.points[4], cube.points[0], cube.points[3], cube.points[7] }));
            cube.Add(new Polygon(new Base3D.Point[] { cube.points[5], cube.points[4], cube.points[7], cube.points[6] }));
            cube.Add(new Polygon(new Base3D.Point[] { cube.points[1], cube.points[5], cube.points[6], cube.points[2] }));
            cube.Add(new Polygon(new Base3D.Point[] { cube.points[0], cube.points[1], cube.points[5], cube.points[4] }));
            cube.Add(new Polygon(new Base3D.Point[] { cube.points[3], cube.points[2], cube.points[6], cube.points[7] }));

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
            for (int i = 0; i < count; i+=2)
            {
                tetra.Add(new Polygon(new Base3D.Point[] { cube.polygons[i].points[0], cube.polygons[4].points[2], cube.polygons[count - 1 - i].points[3] }));
                tetra.Add(new Polygon(new Base3D.Point[] { cube.polygons[i].points[0], cube.polygons[5].points[1], cube.polygons[i + 1].points[3] }));
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
                var tetra = GenerateOcta(point.p);
                context.world.entities.Add(tetra);
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
                var tetra = GenerateOcta(point.p);
                context.world.entities.Add(tetra);
                context.Redraw();
                context.world.entities.Remove(tetra);
            }
        }

        Polytope GenerateOcta(Base3D.Point location)
        {
            Matrix move = Matrix.Move(location);
            Polytope cube = GenerateCube(location);
            Polytope octa = new Polytope();

            foreach(var edge in cube.polygons)
            {
                var sump = new Base3D.Point();
                foreach (var p in edge.points)
                    sump += p;
                octa.Add((sump / 4) * move);
            }

            octa.Add(new Polygon(new Base3D.Point[] { octa.points[0], octa.points[1], octa.points[2], octa.points[3] }));

            for (int i = 0; i < octa.points.Count - 2; i++)
            {
                octa.Add(new Polygon(new Base3D.Point[] { octa.points[i], octa.points[(i + 1) % 4], octa.points[4] }));
                octa.Add(new Polygon(new Base3D.Point[] { octa.points[i], octa.points[5], octa.points[(i + 1) % 4] }));
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
            circle.Add(new Polygon(circle.points));

            icosa.Add(circle.points[circle.points.Count / 20]);

            for (int i = 1; i < 10; i++)
                icosa.Add(circle.points[circle.points.Count / 20 + i * 24]); //120/5

            double iz = 1 + Math.Sqrt(5) / 2;
            icosa.Add(new Base3D.Point { X = 0, Y = 0, Z = iz } * move);
            icosa.Add(new Base3D.Point { X = 0, Y = 0, Z = -iz } * move);
            for (int i = 0; i < 10; i+=5)
            {
                icosa.Add(new Polygon(new Base3D.Point[] { icosa.points[i], icosa.points[i + 1], icosa.points[i + 2], icosa.points[i + 3], icosa.points[i + 4] })); //пятигранник на окружности
                //шапочка и барабан
                for (int j = 0; j < 5; j++)
                {
                    icosa.Add(new Polygon(new Base3D.Point[] { icosa.points[i + j], icosa.points[(i + j + 1) % 5], icosa.points[i % 5] }));
                    icosa.Add(new Polygon(new Base3D.Point[] { icosa.points[i + j], icosa.points[i + 1], icosa.points[i + 2], icosa.points[i + 3], icosa.points[i + 4] })); 
                }
            }

            return icosa;
        }
    }
}
