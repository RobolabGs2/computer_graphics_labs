using Lab06.Graph3D;
using Lab06.Materials3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Lab06.Base3D
{
    public class Context
    {
        public Camera camera = new Camera();
        public World world = new World();

        public IDrawing drawing;
        public PictureBox pictureBox;
        public Bitmap bitmap;

        public double scale = 100;

        public Action<Graphics> Posteffect = g => {};
        public KeyEventHandler KeyUp = (sender, args) => {};
        public KeyEventHandler KeyDown = (sender, args) => { };

        public Context(PictureBox pictureBox)
        {
            this.pictureBox = pictureBox;
            pictureBox.SizeChanged += (o, s) => Resize();
            bitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
        }

        public Matrix DrawingMatrix()
        {
            return
                camera.Projection() *
                Matrix.Scale(new Point { X = scale, Y = scale, Z = scale }) *
                Matrix.Move(new Base3D.Point
                {
                    Y = -bitmap.Width / 2,
                    Z = -bitmap.Height / 2
                }) *
            Matrix.Scale(new Point { X = 1, Y = -1, Z = -1 });
        }

        public Matrix InvertDrawingMatrix()
        {
            return
                Matrix.Scale(new Point { X = 1, Y = -1, Z = -1 }) *
                Matrix.Move(new Base3D.Point
                {
                    Y = bitmap.Width / 2,
                    Z = bitmap.Height / 2
                }) *
                Matrix.Scale(new Point { X = 1 / scale, Y = 1 / scale, Z = 1 / scale }) *
                camera.InvertProjection();
        }


        /// <summary>
        /// Преобразует точку на экране в соответствующую точку на плоскости XY
        /// </summary>
        public (Base3D.Point p, bool front) ScreenToXY(int x, int y)
        {
            Point p1 = (new Point { Y = x, Z = y } * InvertDrawingMatrix());
            Point p2 = (new Point { Y = x, Z = y, X = 1 } * InvertDrawingMatrix());

            double denum = p2.Z - p1.Z;
            double numX = -(p2.X - p1.X) * p2.Z;
            double numY = -(p2.Y - p1.Y) * p2.Z;

            return (new Point { X = numX / denum + p2.X, Y = numY / denum + p2.Y },
                Math.Sign(denum) * Math.Sign(camera.location.Z) < 10);
        }

        private void Resize()
        {
            if (pictureBox.Width <= 0 || pictureBox.Height <= 0)
                return;
            Bitmap bmp = new Bitmap(pictureBox.Width, pictureBox.Height);
            (bmp, bitmap) = (bitmap, bmp);
            Redraw();
            pictureBox.Image = bitmap;
            bmp?.Dispose();
        }

        public Base3D.Point SeletedPivot()
        {
            return new Group(world.selected).Pivot();
        }

        public bool ScreenPointInGroup(int x, int y, Group group)
        {
            return group.entities.Any(e => {
                if (e is Polytope p)
                    return ScreenPointInPolytope(x, y, p);
                return false;
            });
        }

        public bool ScreenPointInPolytope(int x, int y, Polytope poly)
        {
            var dravingMatrix = DrawingMatrix();
            List<Base3D.Point> polyPoints = poly.points.Select(p => p * dravingMatrix).ToList();

            return poly.polygons.Any(polygon =>
            {
                var points = polygon.Points(polyPoints);
                if (!points.All(p => BeforeScreen(p.X)))
                    return false;
                int count = 0;
                for (int i = 0; i < points.Count; ++i)
                {
                    var p1 = points[i];
                    var p2 = points[(i + 1) % points.Count];

                    if (!(x < Math.Max(p1.Y, p2.Y)
                          && x >= Math.Min(p1.Y, p2.Y)))
                        continue;
                    count += Math.Sign(x * (p2.Z - p1.Z) + y * (-p2.Y + p1.Y) + p2.Y * p1.Z - p1.Y * p2.Z);
                }
                return count != 0;
            });
        }

        public void Redraw()
        {
            drawing.Draw(bitmap);
            Graphics g = Graphics.FromImage(bitmap);
            Posteffect(g);
            g.Dispose();
            pictureBox.Image = bitmap;
        }

        public bool BeforeScreen(double t)
        {
            return camera.BeforeScreen(t / scale);
        }
    }
}
