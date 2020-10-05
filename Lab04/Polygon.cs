using Lab04.Tools;
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

        // Меняет направление на по-часовой при необходимости
        public void Repair()
        {
            if (Area() < 0)
                Points.Reverse();
        }

        public void Apply(Matrix matrix)
        {
            Points = Points.Select(p => p * matrix).ToList();
            Repair();
        }

        public void Add(Point point)
        {
            Points.Add(point);
        }

        private void DrawPoint(Graphics g, Pen pen, Matrix matrix = null)
        {
            Point p = (matrix != null) ? Points[0] * matrix : Points[0];
            p.Draw(g, pen);
        }

        public void PartialDraw(Graphics g, Pen pen, Matrix matrix = null)
        {
            if (Points.Count == 0)
                return;
            if (Points.Count == 1)
            {
                DrawPoint(g, pen, matrix);
                return;
            }

            for (int i = 1; i < Points.Count; ++i)
            {
                Point p1 = (matrix != null) ? Points[i - 1] * matrix : Points[i - 1];
                Point p2 = (matrix != null) ? Points[i] * matrix : Points[i];
                g.DrawLine(pen, (float) p1.X, (float) p1.Y,
                    (float) p2.X, (float) p2.Y);
            }
        }

        public void Draw(Graphics g, Pen pen)
        {
            PartialDraw(g, pen);
            if (Points.Count > 2)
                g.DrawLine(pen,
                    (float) Points[Points.Count - 1].X, (float) Points[Points.Count - 1].Y,
                    (float) Points[0].X, (float) Points[0].Y);
        }

        public void Draw(Graphics g, Pen pen, Matrix matrix)
        {
            PartialDraw(g, pen, matrix);

            if (Points.Count > 2)
            {
                Point p1 = (matrix != null) ? Points[Points.Count - 1] * matrix : Points[Points.Count - 1];
                Point p2 = (matrix != null) ? Points[0] * matrix : Points[0];
                g.DrawLine(pen,
                    (float) p1.X, (float) p1.Y,
                    (float) p2.X, (float) p2.Y);
            }
        }

        public IEnumerable<LineSegment> Edges()
        {
            for (var i = 0; i < Points.Count - 1; ++i)
            {
                yield return new LineSegment(Points[i], Points[i + 1]);
            }

            yield return new LineSegment(Points.Last(), Points.First());
        }

        public bool IsInternal(Point p)
        {
            if (Points.Count < 3)
                return false;
            var count = Edges().Select(segment =>
            {
                if (!(p.X < Math.Max(segment.P1.X, segment.P2.X)
                      && p.X >= Math.Min(segment.P1.X, segment.P2.X)))
                    return 0;
                return segment.Sign(p);
            }).Sum();
            return count != 0;
        }

        public bool IsConvex()
        {
            if (Points.Count < 3)
                return false;
            for (int i = 0; i < Points.Count; ++i)
            {
                LineSegment line = new LineSegment(Points[i], Points[(i + 1) % Points.Count]);
                if (line.Sign(Points[(i + 2) % Points.Count]) > 0)
                    return false;
            }

            return true;
        }

        public List<Polygon> Split(Point p1, Point p2)
        {
            var cycled = new CycledVertexes(this);
            cycled.SetCurrent(p1);
            var second = cycled.Split(cycled.First(v => v.Value == p2));
            if (second == null)
            {
                return new List<Polygon>();
            }
            return new List<Polygon>
            {
                cycled.ToPolygon(), second.ToPolygon()
            };
        }

        public IEnumerable<Polygon> Triangulate()
        {
            return new CycledVertexes(this).Triangulate().Select(p=>p.ToPolygon());
        }

        public Polygon Intersection(Polygon other)
        {
            return new Polygon();
        }
    }
}