using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab04
{
    public class Polygon
    {
        public List<Point> Points { get; set; }

        public Polygon()
        {
            Points = new List<Point>();
        }

        public (Point p1, Point p2) ABBA()
        {
            Point p1 = new Point {
                X = Points.Select(p => p.X).Min(),
                Y = Points.Select(p => p.Y).Min() };

            Point p2 = new Point {
                X = Points.Select(p => p.X).Max(),
                Y = Points.Select(p => p.Y).Max() };

            return (p1, p2);
        }

        public void Add(Point point)
        {
            Points.Add(point);
        }

        private void Drawpoint(Graphics g, Pen pen)
        {
            float r = pen.Width;
            g.DrawEllipse(pen, (float)Points[0].X - r, (float)Points[0].Y - r, r * 2, r * 2);
        }

        public void PartialDraw(Graphics g, Pen pen)
        {
            if (Points.Count == 0)
                return;
            if (Points.Count == 1)
            {
                Drawpoint(g, pen);
                return;
            }
            for (int i = 1; i < Points.Count; ++i)
                g.DrawLine(pen, (float)Points[i - 1].X, (float)Points[i - 1].Y,
                    (float)Points[i].X, (float)Points[i].Y);
        }
        public void PartialDraw(Graphics g, Pen pen, Matrix matrix)
        {
            if (Points.Count == 0)
                return;
            if (Points.Count == 1)
            {
                Drawpoint(g, pen);
                return;
            }
            for (int i = 1; i < Points.Count; ++i)
            {
                Point p1 = Points[i - 1] * matrix;
                Point p2 = Points[i] * matrix;
                g.DrawLine(pen, (float)p1.X, (float)p1.Y,
                    (float)p2.X, (float)p2.Y);
            }
        }

        public void Draw(Graphics g, Pen pen)
        {
            PartialDraw(g, pen);
            if (Points.Count > 2)
                g.DrawLine(pen,
                    (float)Points[Points.Count - 1].X, (float)Points[Points.Count - 1].Y,
                    (float)Points[0].X, (float)Points[0].Y);
        }
        public void Draw(Graphics g, Pen pen, Matrix matrix)
        {
            PartialDraw(g, pen, matrix);

            if (Points.Count > 2)
            {
                Point p1 = Points[Points.Count - 1] * matrix;
                Point p2 = Points[0] * matrix;
                g.DrawLine(pen,
                    (float)p1.X, (float)p1.Y,
                    (float)p2.X, (float)p2.Y);
            }
        }
    }
}
