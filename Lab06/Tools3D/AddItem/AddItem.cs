using Lab06.Base3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

            tab.AddButton(Properties.Resources.Reverse, true);

            var pontButton = tab.AddButton(Properties.Resources.Point, true);


            pontButton.ButtonClick += t => {
                context.pictureBox.MouseClick += AddPoint;
                context.pictureBox.MouseMove += MovePoint;
            };

            pontButton.ButtonDisable += t => {
                context.pictureBox.MouseClick -= AddPoint;
                context.pictureBox.MouseMove -= MovePoint;
            };

            var cubeButton = tab.AddButton(Properties.Resources.Cube, true);

            cubeButton.ButtonClick += t => {
                context.pictureBox.MouseClick += AddCube;
                context.pictureBox.MouseMove += MoveCube;
            };

            cubeButton.ButtonDisable += t => {
                context.pictureBox.MouseClick -= AddCube;
                context.pictureBox.MouseMove -= MoveCube;
            };
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
            cube.Add(new Polygon(new Base3D.Point[] { cube.points[4], cube.points[5], cube.points[6], cube.points[7] }));
            cube.Add(new Polygon(new Base3D.Point[] { cube.points[0], cube.points[1], cube.points[5], cube.points[4] }));
            cube.Add(new Polygon(new Base3D.Point[] { cube.points[2], cube.points[3], cube.points[7], cube.points[6] }));

            return cube;
        }
    }
}
