using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Lab06.Base3D
{
    public class Point: Entity
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double T { get; set; }

        public Point()
        {
            T = 1;
        }

        public Point Copy()
        {
            return new Point { X = X, Y = Y, Z = Z, T = T };
        }

        public double DistanceTo(Point another)
        {
            return (another - this).Length();
        }

        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z + T * T);
        }

        public Point Normal()
        {
            return this / Length();
        }

        /// Внезапно, унарный минус не переворачивает четвёртую координату, просто потому что.
        public static Point operator -(Point p)
        {
            return new Point { X = -p.X, Y = -p.Y, Z = -p.Z };
        }

        public static Point operator /(Point p, double v)
        {
            return new Point { X = p.X / v, Y = p.Y / v, Z = p.Z / v, T = p.T / v };
        }

        public static Point operator *(Point p, double v)
        {
            return new Point { X = p.X * v, Y = p.Y * v, Z = p.Z * v, T = p.T * v };
        }

        public static Point operator -(Point p1, Point p2)
        {
            return new Point { X = p1.X - p2.X, Y = p1.Y - p2.Y, Z = p1.Z - p2.Z, T = p1.T - p2.T };
        }

        public static Point operator +(Point p1, Point p2)
        {
            return new Point { X = p1.X + p2.X, Y = p1.Y + p2.Y, Z = p1.Z + p2.Z, T = p1.T + p2.T };
        }

        public override (Point, Point) ABBA()
        {
            return (Copy(), Copy());
        }

        ///  Выдаёт максимальную по каждой координате
        public static Point Max(Point p1, Point p2)
        {
            return new Point
            {
                X = Math.Max(p1.X, p2.X),
                Y = Math.Max(p1.Y, p2.Y),
                Z = Math.Max(p1.Z, p2.Z),
                T = Math.Max(p1.T, p2.T),
            };
        }

        //  Выдаёт минимульную по каждой координате
        public static Point Min(Point p1, Point p2)
        {
            return new Point
            {
                X = Math.Min(p1.X, p2.X),
                Y = Math.Min(p1.Y, p2.Y),
                Z = Math.Min(p1.Z, p2.Z),
                T = Math.Min(p1.T, p2.T),
            };
        }

        public override IEnumerable<Point> Points()
        {
            yield return this;
            yield break;
        }

        ///  Возвращает точку, у которой T единица, 
        ///  применяя его к другим координатам
        public Point FlattenT()
        {
            return this / T;
        }

        public override void Apply(Matrix m)
        {
            (X, Y, Z, T) =
                (
                    X * m[0, 0] + Y * m[1, 0] + Z * m[2, 0] + T * m[3, 0],
                    X * m[0, 1] + Y * m[1, 1] + Z * m[2, 1] + T * m[3, 1],
                    X * m[0, 2] + Y * m[1, 2] + Z * m[2, 2] + T * m[3, 2],
                    X * m[0, 3] + Y * m[1, 3] + Z * m[2, 3] + T * m[3, 3]
                );
        }
    }
}
