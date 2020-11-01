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
        public int[] indexes;
        public int[] normals;
        public int[] texture;
        private Point[] vetexBuffer;

        public Polygon(IEnumerable<int> indexes)
        {
            this.indexes = indexes.ToArray();
            vetexBuffer = new Point[this.indexes.Length];
        }

        public Polygon(IEnumerable<int> indexes, IEnumerable<int> texture, IEnumerable<int> normals)
        {
            this.indexes = indexes.ToArray();
            this.normals = normals?.ToArray();
            this.texture = texture?.ToArray();
            vetexBuffer = new Point[this.indexes.Length];
        }
        public Polygon(IEnumerable<int> indexes, IEnumerable<int> normals)
        {
            this.indexes = indexes.ToArray();
            this.normals = normals?.ToArray();
            vetexBuffer = new Point[this.indexes.Length];
        }

        public Point[] Points(List<Point> points)
        {
            for (int i = 0; i < indexes.Length; ++i)
                vetexBuffer[i] = points[indexes[i]];
            return vetexBuffer;
        }

        public Point[] Points(Polytope parent)
        {
            return Points(parent.points);
        }

        public (Point pMin, Point pMax) ABBA(Polytope parent)
        {
            if (indexes.Length == 0)
                return (new Point(), new Point());
            
            var points = Points(parent);
            
            return points.Skip(1).Aggregate((points[0], points[0]),
                (abba, p) => (Point.Min(abba.Item1, p), Point.Max(abba.Item2, p)));
        }
    }
}
