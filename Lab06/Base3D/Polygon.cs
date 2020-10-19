using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab06.Base3D
{
    public class Polygon : Entity
    {
        public List<Point> points = new List<Point>();

        public Polygon()
        { }

        public Polygon(IEnumerable<Point> points)
        {
            this.points = points.ToList();
        }

        public void Add(Point p)
        {
            points.Add(p);
        }

        public override (Point pMin, Point pMax) ABBA()
        {
            if (points.Count == 0)
                return (new Point(), new Point());
            return points.Skip(1).Aggregate(points[0].ABBA(),
                (abba, p) => (Point.Min(abba.Item1, p), Point.Max(abba.Item2, p)));
        }

        public override IEnumerable<Point> Points()
        {
            return points;
        }

        public override void Apply(Matrix matrix)
        {
            foreach (Point p in points)
                p.Apply(matrix);
        }
    }
}
