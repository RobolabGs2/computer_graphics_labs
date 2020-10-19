using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab06.Base3D
{
    public class Group: Entity
    {
        public List<Entity> entities = new List<Entity>();

        public Group()
        { }
        public Group(IEnumerable<Entity> entities)
        {
            this.entities = entities.ToList();
        }

        public void Add(Entity entity)
        {
            entities.Add(entity);
        }

        public override void Apply(Matrix matrix)
        {
            foreach (Entity entity in entities)
                entity.Apply(matrix);
        }

        public override IEnumerable<Point> Points()
        {
            foreach (var e in entities)
                foreach (var p in e.Points())
                    yield return p;
            yield break;
        }

        public override (Point pMin, Point pMax) ABBA()
        {
            if (entities.Count == 0)
                return base.ABBA();
            var result = entities[0].ABBA();
            foreach (Entity entity in entities.Skip(1))
            {
                var current = entity.ABBA();
                result.pMax = Point.Max(current.pMax, result.pMax);
                result.pMin = Point.Min(current.pMin, result.pMin);
            }
            return result;
        }
    }
}
