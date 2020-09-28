using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Lab04.Tools
{
    class Scale : ITool
    {
        public Bitmap image => Properties.Resources.Scale;

        Polygon poly;
        double delta = 50;
        Pen pen = new Pen(Color.Blue);

        public void Init(Context context)
        {
            poly = new Polygon();
            poly.Add(new Point { X = 0, Y = -40 });
            poly.Add(new Point { X = 0, Y = 0 });
            poly.Add(new Point { X = 40, Y = 0 });
            poly.Add(new Point { X = 24, Y = 0 });
            poly.Add(new Point { X = 0, Y = -24 });
            poly.Add(new Point { X = 0, Y = -16 });
            poly.Add(new Point { X = 16, Y = 0 });
        }

        public bool Active()
        {
            return true;
        }

        public Matrix Draw(Point start, Point end, Graphics graphics)
        {
            Matrix matrix = Matrix.Move(-start) * Matrix.Scale(
                    ((end - start) * Matrix.Scale(new Point { X = 1, Y = -1 }) +
                    new Point { X = delta, Y = delta }) / delta) * Matrix.Move(start);

            poly.PartialDraw(graphics, pen, Matrix.Move(start) * matrix);
            return matrix;
        }
    }
}
