using Lab06.Materials3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
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
            points = points.Select(p => p * matrix).ToList();
        }
    }
}
