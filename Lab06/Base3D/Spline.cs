using System.Collections.Generic;
using System.Linq;

namespace Lab06.Base3D
{
    class Spline: Entity
    {
        public List<Point> points = new List<Point>();
        public override IEnumerable<Point> Points() => points;

        public void Add(Point point)
        {
            points.Add(point);
        }

        public override void Apply(Matrix matrix)
        {
            points = points.Select(p => p * matrix).ToList();
        }
    }
}
