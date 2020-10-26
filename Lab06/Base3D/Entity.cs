using Lab06.Materials3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab06.Base3D
{
    public abstract class Entity
    {
        public BaseMaterial Matreial { get; set; } = new Materials3D.SolidMaterial();

        public virtual IEnumerable<Point> Points()
        {
            yield break;
        }

        public virtual (Point pMin, Point pMax) ABBA()
        {
            var start = Points().FirstOrDefault();
            return Points().Aggregate((start, start), (abba, p) => (Point.Min(abba.Item1, p), Point.Max(abba.Item2, p)));
        }

        public virtual Point Pivot()
        {
            var abba = ABBA();
            return (abba.pMin + abba.pMax) / 2;
        }

        public abstract void Apply(Matrix matrix);
    }
}
