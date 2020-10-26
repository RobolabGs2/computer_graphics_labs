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
        public List<Point> normals = new List<Point>();

        public void Add(Point p)
        {
            points.Add(p);
        }

        public void Add(Polygon p)
        {
            polygons.Add(p);
        }

        public void AddNormal(Point n)
        {
            normals.Add(n);
        }
        
        public override IEnumerable<Point> Points()
        {
            return points;
        }

        public override void Apply(Matrix matrix)
        {
            Point zero = new Point() * matrix;
            for (int i = 0; i < normals.Count; ++i)
                normals[i] = normals[i] * (matrix) - zero;
            for (int i = 0; i < points.Count; ++i)
                points[i] = points[i] * (matrix);
        }
    }
}
