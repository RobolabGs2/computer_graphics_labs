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
        public BaseMaterial Matreial { get; } = new Materials3D.SolidMaterial();

        public virtual IEnumerable<Entity> Children()
        {
            yield break;
        }

        public virtual (Point pMin, Point pMax) ABBA()
        {
            return (new Point { }, new Point { });
        }

        public virtual Point Pivot()
        {
            var abba = ABBA();
            return (abba.pMin + abba.pMax) / 2;
        }

        public abstract void Apply(Matrix matrix);
    }
}
