using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab04.Tools
{
    class Rotate : ITool
    {
        public Bitmap image => Properties.Resources.Rotate;

        public void Init(Context context)
        {
        }

        public Matrix Draw(Point start, Point finish, Graphics g)
        {
            if (start == finish)
            {
                return Matrix.Ident();
            }
            var r = start.DistanceTo(finish);
            var delta = finish - start;
            var cosPhi = delta.X / r;
            var sinPhi = delta.Y / r;
            DrawCircle(start, finish, g, cosPhi, r);
            return Matrix.InPoint(start, Matrix.Rotate(cosPhi, sinPhi));
        }

        private static void DrawCircle(Point start, Point finish, Graphics g, double cosPhi, double r)
        {
            var angel = (float)(Math.Acos(cosPhi) * 180 / Math.PI);
            if (finish.Y > start.Y)
            {
                angel = 360 - angel;
            }
            var p = new Pen(Color.Green);
            g.DrawLine(p, start.ToPointF(), finish.ToPointF());
            var textPoint = new PointF((float) (start.X + r / 4 - 16), (float) start.Y - 16);
            g.DrawString(angel.ToString("#.##°"), new Font("Consolas", 10), p.Brush, textPoint);
            g.DrawPie(p, (float) (start.X - r / 2), (float) (start.Y - r / 2), (float) r, (float) r, 360 - angel, angel);
        }

        public bool Active()
        {
            return true;
        }
    }
}