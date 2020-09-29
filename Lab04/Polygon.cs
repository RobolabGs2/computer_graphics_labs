using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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
            return Point.ABBA(Points);
        }

        public double Area()
        {
            double area = 0;
            int count = Points.Count() - 1;

            for (int i = 0; i < count; i++)
                area += Points[i].X * Points[i + 1].Y -
                    Points[i + 1].X * Points[i].Y;
            area += Points[count].X * Points[0].Y -
                    Points[0].X * Points[count].Y;


            return area / 2;
        }

        public void Add(Point point)
        {
            Points.Add(point);
        }

        private void Drawpoint(Graphics g, Pen pen, Matrix matrix = null)
        {
            Point p = (matrix != null) ? Points[0] * matrix : Points[0];
            float r = pen.Width;
            g.DrawEllipse(pen, (float)p.X - r, (float)p.Y - r, r * 2, r * 2);
        }

        public void PartialDraw(Graphics g, Pen pen, Matrix matrix = null)
        {
            if (Points.Count == 0)
                return;
            if (Points.Count == 1)
            {
                Drawpoint(g, pen, matrix);
                return;
            }
            for (int i = 1; i < Points.Count; ++i)
            {
                Point p1 = (matrix != null) ? Points[i - 1] * matrix : Points[i - 1];
                Point p2 = (matrix != null) ? Points[i] * matrix : Points[i];
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
        public void Draw(Graphics g, Pen pen, Matrix matrix = null)
        {
            PartialDraw(g, pen, matrix);

            if (Points.Count > 2)
            {
                Point p1 = (matrix != null) ? Points[Points.Count - 1] * matrix : Points[Points.Count - 1];
                Point p2 = (matrix != null) ? Points[0] * matrix : Points[0];
                g.DrawLine(pen,
                    (float)p1.X, (float)p1.Y,
                    (float)p2.X, (float)p2.Y);
            }
        }

        public IEnumerable<LineSegment> Edges()
        {
            for (var i = 0; i < Points.Count - 1; ++i)
            {
                yield return new LineSegment(Points[i], Points[i+1]);
            }
            yield return new LineSegment(Points.Last(), Points.First());
        }

        public bool IsInternal(Point p)
        {
            // return Points.Count >= 3 && Edges().Select(segment => segment.Sign(p)).Sum() < 0;
            // return Points.Count >= 3 && Edges().All(segment => segment.Sign(p) < 0);
            return Points.Count >= 3 && Edges().Where(segment =>
            {
                if(p.Y < Math.Min(segment.P1.Y, segment.P2.Y) || p.Y > Math.Max(segment.P1.Y, segment.P2.Y))
                    return false;

                if (p.X < Math.Min(segment.P1.X, segment.P2.X))
                    return true;
                return segment
                    .Intersection(new LineSegment(p, new Point {X = Math.Max(segment.P1.X, segment.P2.X), Y = p.Y}))
                    .onLine;
                if (segment.P1.Y > segment.P2.Y)
                {
                    segment = new LineSegment(segment.P2, segment.P1);
                }

                return segment.Sign(p) > 0;
            }).Count() %2==1;
        }
    }
}
