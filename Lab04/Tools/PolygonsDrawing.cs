using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Lab04.Tools
{
    class PolygonsDrawing : ITool
    {
        public Bitmap image => Properties.Resources.Polygon;
        Context context;
        Polygon p;
        bool onLine;

        public void Init(Context context)
        {
            this.context = context;
            onLine = false;
        }

        public void Restart()
        {
            if (p == null)
                return;
            if (p.Area() < 0)
                p.Points.Reverse();
            context.Add(p);
            context.Selected.Clear();
            context.Selected.Add(p);
            p = null;
        }

        public void Move(Point point, Graphics graphics)
        {
            if (p == null)
                return;
            Pen pen = new Pen(Color.Blue, 2);
            p.PartialDraw(graphics, pen);
            graphics.DrawLine(pen, 
                (float)p.Points[p.Points.Count - 1].X, (float)p.Points[p.Points.Count - 1].Y,
                (float)point.X, (float)point.Y);

            pen.Color = Color.Red;
            int count = p.Points.Count();
            onLine = false;
            for (int i = 0; i < count - 2; i++)
            {
                LineSegment newedge = new LineSegment(p.Points[count - 1], point);
                LineSegment polyedge = new LineSegment(p.Points[i], p.Points[i + 1]);
                float r = 2;
                var inter = newedge.Intersection(polyedge);
                if (inter.onLine)
                {
                    graphics.DrawEllipse(pen, (float)inter.p.X - r, (float)inter.p.Y - r, r * 2, r * 2);
                    onLine = true;
                }
            }
            pen.Dispose();
        }

        public void Down(Point point, Graphics graphics)
        {
            if (p == null)
                p = new Polygon();
            if (!onLine)
                p.Add(point);
            Move(point, graphics);
        }

        public Matrix Draw(Point start, Point end, Graphics graphics)
        {
            return Matrix.Ident();
        }
        public bool Active()
        {
            p = null;
            return true;
        }
    }
}
