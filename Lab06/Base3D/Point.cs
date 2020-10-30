using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Lab06.Base3D
{
    public struct Point
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public double DistanceTo(Point another)
        {
            return (another - this).Length();
        }

        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public Point Normal()
        {
            return this / Length();
        }

        public static Point operator -(Point p)
        {
            return new Point { X = -p.X, Y = -p.Y, Z = -p.Z };
        }

        public static Point operator /(Point p, double v)
        {
            return new Point { X = p.X / v, Y = p.Y / v, Z = p.Z / v};
        }

        public static Point operator *(Point p, double v)
        {
            return new Point { X = p.X * v, Y = p.Y * v, Z = p.Z * v };
        }

        public static Point operator *(double v, Point p)
        {
            return new Point { X = p.X * v, Y = p.Y * v, Z = p.Z * v };
        }

        public static Point operator -(Point p1, Point p2)
        {
            return new Point { X = p1.X - p2.X, Y = p1.Y - p2.Y, Z = p1.Z - p2.Z};
        }

        public static Point operator +(Point p1, Point p2)
        {
            return new Point { X = p1.X + p2.X, Y = p1.Y + p2.Y, Z = p1.Z + p2.Z };
        }

        public double DotProd(Point p)
        {
            return X * p.X + Y * p.Y + Z * p.Z;
        }

        /// <summary>
        /// Умножение как вектора, то есть не трогая T
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Point vectorMult(double v)
        {
            return new Point { X = X * v, Y = Y * v, Z = Z * v};
        }

        public Point vectorAdd(Point p)
        {
            return new Point { X = X + p.X, Y = Y + p.Y, Z = Z + p.Z};
        }

        ///  Выдаёт максимальную по каждой координате
        public static Point Max(Point p1, Point p2)
        {
            return new Point
            {
                X = Math.Max(p1.X, p2.X),
                Y = Math.Max(p1.Y, p2.Y),
                Z = Math.Max(p1.Z, p2.Z),
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
            };
        }

        public double Angle(Point p)
        {
            return Math.Acos((p.X * X + p.Y * Y + p.Z * Z) /
                Math.Sqrt(X * X + Y * Y + Z * Z) / 
                Math.Sqrt(p.X * p.X + p.Y * p.Y + p.Z * p.Z) );
        }
    }
}
