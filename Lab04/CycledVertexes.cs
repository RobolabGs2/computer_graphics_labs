using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lab04
{
    public class Vertex : IEnumerable<Point>
    {
        public Vertex(Point value)
        {
            Value = value;
        }

        public Vertex Next { get; set; }
        public Vertex Previous { get; set; }
        public Point Value { get; set; }

        public Vertex Double()
        {
            return Next = Next.Previous = new Vertex(Value) {Next = Next, Previous = this};
        }

        public IEnumerator<Point> GetEnumerator()
        {
            return new VertexPointEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class VertexPointEnumerator : IEnumerator<Point>
    {
        private readonly VertexEnumerator enumerator;

        internal VertexPointEnumerator(Vertex start)
        {
            enumerator = new VertexEnumerator(start);
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            return enumerator.MoveNext();
        }

        public void Reset()
        {
            enumerator.Reset();
        }

        public Point Current => enumerator.Current.Value;

        object IEnumerator.Current => Current;
    }

    public class VertexEnumerator : IEnumerator<Vertex>
    {
        private readonly Vertex start;

        internal VertexEnumerator(Vertex start)
        {
            this.start = start;
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (Current == null)
            {
                Current = start;
                return true;
            }

            if (Current.Next == start)
                return false;
            Current = Current.Next;
            return true;
        }

        public void Reset()
        {
            Current = start;
        }

        public Vertex Current { get; private set; }

        object IEnumerator.Current => Current;
    }

    public class CycledVertexes : IEnumerable<Vertex>
    {
        public Vertex V;

        public CycledVertexes(Polygon origin)
        {
            var v = origin.Points.Select(p => new Vertex(p)).ToList();
            V = v.First();
            var last = v.Aggregate((v1, v2) =>
            {
                v2.Previous = v1;
                v1.Next = v2;
                return v2;
            });
            last.Next = V;
            V.Previous = last;
        }

        public CycledVertexes(Vertex vertex)
        {
            V = vertex;
        }

        public void FindConvexVertex()
        {
            while (new LineSegment(V.Previous.Value, V.Value).Sign(V.Next.Value) > 0)
            {
                V = V.Next;
            }
        }

        public Vertex FindIntrudingVertex()
        {
            var a = V.Previous;
            var b = V;
            var c = V.Next;
            var triangle = new Polygon {Points = {a.Value, b.Value, c.Value}};
            var ca = new LineSegment(c.Value, a.Value);
            return this.Skip(2).TakeWhile(p => p != a)
                .Where(p => triangle.IsInternal(p.Value))
                .Aggregate((Vertex) null,
                    (v1, v2) => v1 == null ? v2 : (ca.Distance(v1.Value) < ca.Distance(v2.Value) ? v2 : v1));
        }

        public IEnumerable<CycledVertexes> Triangulate()
        {
            while (V.Count() > 3)
            {
                FindConvexVertex();
                var d = FindIntrudingVertex();
                if (d == null)
                {
                    d = V.Next;
                    V = V.Previous;
                }

                foreach (var r in Split(d).Triangulate())
                {
                    yield return r;
                }
            }

            yield return this;
        }

        public CycledVertexes Split(Vertex by)
        {
            if (V == by)
            {
                return null;
            }

            var dv = V.Double();
            var db = by.Double();
            V.Next = db;
            db.Previous = V;

            dv.Previous = by;
            by.Next = dv;
            return new CycledVertexes(by);
        }

        public void SetCurrent(Point p)
        {
            V = this.First(point => p == point.Value);
        }

        public Polygon ToPolygon()
        {
            return new Polygon {Points = V.ToList()};
        }


        public IEnumerator<Vertex> GetEnumerator()
        {
            return new VertexEnumerator(V);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}