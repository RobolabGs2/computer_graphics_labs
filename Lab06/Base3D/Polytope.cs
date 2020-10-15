using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab06.Base3D
{
    public class Polytope: Entity
    {
        public List<Polygon> polygons = new List<Polygon>();
        
        //  Тут должны жить те же точки, что и в полигонах, 
        //  что бы можно было получить точки без повторов
        public List<Point> points = new List<Point>();

        public void Add(Point p)
        {
            points.Add(p);
        }

        public void Add(Polygon p)
        {
            polygons.Add(p);
        }

        public override (Point pMin, Point pMax) ABBA()
        {
            if (points.Count == 0)
                return (new Point(), new Point());
            return points.Skip(1).Aggregate(points[0].ABBA(),
                (abba, p) => (Point.Min(abba.Item1, p), Point.Max(abba.Item2, p)));
        }

        public override IEnumerable<Entity> Children()
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
