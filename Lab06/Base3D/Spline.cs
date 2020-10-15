using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab06.Base3D
{
    class Spline: Entity
    {
        public List<Point> points = new List<Point>();

        public void Add(Point point)
        {
            points.Add(point);
        }

        public override void Apply(Matrix matrix)
        {
            foreach (Point p in points)
                p.Apply(matrix);
        }
    }
}
