using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace Lab06.Base3D
{
    public class Polygon
    {
        public List<int> indexes;
        public List<int> normals;

        public Polygon()
        {
            indexes = new List<int>();
        }

        public Polygon(IEnumerable<int> indexes)
        {
            this.indexes = indexes.ToList();
        }

        public Polygon(IEnumerable<int> indexes, List<int> normals)
        {
            this.indexes = indexes.ToList();
            this.normals = normals;
        }

        public List<Point> Points(List<Point> points)
        {
            List<Point> result = new List<Point>(indexes.Count);
            for (int i = 0; i < indexes.Count; ++i)
                result.Add(points[indexes[i]]);
            return result;
        }

        public List<Point> Points(Polytope parent)
        {
            return Points(parent.points);
        }

        public (Point pMin, Point pMax) ABBA(Polytope parent)
        {
            if (indexes.Count == 0)
                return (new Point(), new Point());
            
            var points = Points(parent);
            
            return points.Skip(1).Aggregate((points[0], points[0]),
                (abba, p) => (Point.Min(abba.Item1, p), Point.Max(abba.Item2, p)));
        }
    }
}
