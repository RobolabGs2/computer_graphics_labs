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

namespace Lab06.Base3D
{
    public class Context
    {
        public Camera camera = new Camera();
        public World world = new World();
        public Matrix projection = Matrix.Projection(0.001, 0, 0);

        public IDrawing drawing;
        public PictureBox pictureBox;
        public Bitmap bitmap;

        public double scale = 100;
        public bool cutNegative = true;

        public Action<Graphics> Posteffect = g => {};

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
                projection *
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
                projection.Invert() *
                Matrix.Scale(new Point { X = 1 / scale, Y = 1 / scale, Z = 1 / scale }) *
                camera.InvertProjection();
        }


        /// <summary>
        /// Преобразует точку на экране в соответствующую точку на плоскости XY
        /// </summary>
        public (Base3D.Point p, bool front) ScreenToXY(int x, int y)
        {
            Point p1 = (new Point { Y = x, Z = y } * InvertDrawingMatrix()).FlattenT();
            Point p2 = (new Point { Y = x, Z = y, X = 1 } * InvertDrawingMatrix()).FlattenT();

            double denum = p2.Z - p1.Z;
            double numX = -(p2.X - p1.X) * p2.Z;
            double numY = -(p2.Y - p1.Y) * p2.Z;

            return (new Point { X = numX / denum + p2.X, Y = numY / denum + p2.Y },
                Math.Sign(denum) * Math.Sign(camera.location.Z) < 10);
        }

        /// <summary>
        /// Тут плоскость задаёются уравнением X*x+Y*y+Z*z+T=0,
        /// где X Y Z T это координаты точки
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Point ScreenToPlane(Point plane, int x, int y)
        {
            Point p0 = (new Point { Y = x, Z = y } * InvertDrawingMatrix()).FlattenT();
            Point p1 = (new Point { Y = x, Z = y, X = 1 } * InvertDrawingMatrix()).FlattenT();

            double A = p1.X - p0.X;
            double B = p1.Y - p0.Y;
            double C = p1.Z - p0.Z;
            double t = -(plane.X * p0.X + plane.Z * p0.Z + plane.Z * p0.Z + plane.T) /
                (A * plane.X + B * plane.Y + C * plane.Z);

            return new Point
            {
                X = p0.X + A * t,
                Y = p0.Y + B * t,
                Z = p0.Z + C * t,
            };
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
            return poly.polygons.Any(polygon =>
            {
                var points = polygon.points.Select(p => p * DrawingMatrix()).ToList();
                if (!points.All(p => BeforeScreen(p.X)))
                    return false;
                points = points.Select(p => p.FlattenT()).ToList();
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
            Graphics g = Graphics.FromImage(bitmap);
            drawing.Draw(g);
            Posteffect(g);
            g.Dispose();
            pictureBox.Image = bitmap;
        }

        public bool BeforeScreen(double depth)
        {
            return !cutNegative || depth > -1000;
        }
    }
}
