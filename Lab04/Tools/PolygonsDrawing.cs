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

        private int CountOfInterseptions(Point p1, Point p2, int pcount)
        {
            int countInter = 0;
            LineSegment newedge = new LineSegment(p1, p2);
            for (int i = 0; i < pcount; i++)
            {
                LineSegment polyedge = new LineSegment(p.Points[i], p.Points[i + 1]);
                var inter = newedge.Intersection(polyedge);
                if (inter.onLine)
                    countInter++;
            }
            return countInter;
        }
        public void Restart()
        {
            if (p == null)
                return;
            p.Repair();
           
            if (p.Points.Count <= 2 || CountOfInterseptions(p.Points[p.Points.Count - 1], p.Points[0], p.Points.Count - 1) == 2)
            {
                context.Add(p);
                context.Selected.Clear();
                context.Selected.Add(p);
            }
            
            p = null;
        }

        public void Move(Point point, Graphics graphics)
        {
            if (p == null)
                return;
            Pen pen = new Pen(Color.Blue, 2);
            p.PartialDraw(graphics, pen);
            int count = p.Points.Count();
            LineSegment newedge = new LineSegment(p.Points[count - 1], point); 
            newedge.Draw(graphics, pen);
            onLine = false;
            pen.Color = Color.Red;
            
            for (int i = 0; i < count - 2; i++)
            {
                LineSegment polyedge = new LineSegment(p.Points[i], p.Points[i + 1]);
                var inter = newedge.Intersection(polyedge);
                if (inter.onLine)
                {
                    inter.p.Draw(graphics, pen);
                    onLine = true;
                }
            }
            pen.Dispose();
        }

        public void Down(Point point, Graphics graphics)
        {
            if (p == null)
                p = new Polygon();

            if (!onLine || p.Points.Count == 0)
                p.Add(point);
            onLine = false;
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
