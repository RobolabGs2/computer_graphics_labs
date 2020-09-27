using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab04.Tools
{
    class Devision: ITool
    {
        public Bitmap image => Properties.Resources.Devision;

        Context context;

        public void Init(Context context)
        {
            this.context = context;
        }

        public Matrix Draw(Point start, Point end, Graphics graphics)
        {
            if (start == end)
                return Matrix.Ident();
            Pen pen = new Pen(Color.Black);
            Pen blue_pen = new Pen(Color.Blue, 3);
            Pen green_pen = new Pen(Color.Lime, 3);
            graphics.DrawLine(pen, (float)start.X, (float)start.Y, (float)end.X, (float)end.Y);

            context.Polygons.ForEach(poly => poly.Points.ForEach(point =>{
                double sign = point.X * (end - start).Y - point.Y * (end - start).X + end.X * start.Y - start.X * end.Y;
                float r = 2;
                graphics.DrawEllipse(sign > 0 ? blue_pen: green_pen, (float)point.X - r, (float)point.Y - r, r * 2, r * 2);
            }));
            return Matrix.Ident();
        }
        public bool Active()
        {
            return true;
        }
    }
}
