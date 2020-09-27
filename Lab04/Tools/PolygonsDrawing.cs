using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab04.Tools
{
    class PolygonsDrawing : ITool
    {
        public Bitmap image => Properties.Resources.Polygon;
        Context context;
        Polygon p;

        public void Init(Context context)
        {
            this.context = context;
        }

        public void Restart()
        {
            if (p == null)
                return;
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
            pen.Dispose();
        }

        public void Down(Point point, Graphics graphics)
        {
            if (p == null)
                p = new Polygon();
            p.Add(point);
            Move(point, graphics);
        }

        public Matrix Draw(Point start, Point end, Graphics graphics)
        {
            return Matrix.Ident();
        }
        public bool Active()
        {
            return true;
        }
    }
}
