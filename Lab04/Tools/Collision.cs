using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab04.Tools
{
    class Collision : ITool
    {
        public Bitmap image => Properties.Resources.Collision;
        Context context;
        LineSegment line;

        public bool Active()
        {
            line = null;
            return true;
        }

        public Matrix Draw(Point start, Point end, Graphics graphics)
        {
            if (start == end)
                return Matrix.Ident();
            Pen pen = new Pen(Color.Black);
            Pen green_pen = new Pen(Color.Lime, 3);
            graphics.DrawLine(pen, (float)start.X, (float)start.Y, (float)end.X, (float)end.Y);

            LineSegment lineSegment = new LineSegment(start, end);
            LineSegment polyedge;
            foreach (var poly in context.Polygons)
            {
                int count = poly.Points.Count();
                for (int i = 0; i < count; i++)
                {
                    polyedge = new LineSegment(poly.Points[(i + count) % count], poly.Points[(i + count + 1) % count]);
                    float r = 2;
                    var inter = lineSegment.Intersection(polyedge);
                    if (inter.onLine) graphics.DrawEllipse(green_pen, (float)inter.p.X - r, (float)inter.p.Y - r, r * 2, r * 2);
                }
            }
            return Matrix.Ident();
        }

        public void Init(Context context)
        {
            this.context = context;
        }
    }
}
