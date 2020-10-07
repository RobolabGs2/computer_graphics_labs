using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab04
{
    public struct Point
    {
        public double X { get; set; }
        public double Y { get; set; }

        public static Point operator -(Point p)
        {
            return new Point {X = -p.X, Y = -p.Y};
        }

        public static Point operator /(Point p1, double v)
        {
            return new Point { X = p1.X / v, Y = p1.Y / v };
        }

        public static Point operator *(Point p1, double v)
        {
            return new Point { X = p1.X * v, Y = p1.Y * v };
        }

        public static Point operator -(Point p1, Point p2)
        {
            return new Point { X = p1.X - p2.X, Y = p1.Y - p2.Y };
        }

        public static Point operator +(Point p1, Point p2)
        {
            return new Point { X = p1.X + p2.X, Y = p1.Y + p2.Y };
        }

        public static bool operator ==(Point p1, Point p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y;
        }

        public static bool operator !=(Point p1, Point p2)
        {
            return !(p1 == p2);
        }

        public double DistanceTo(Point another)
        {
            var (x, y) = (X - another.X, Y - another.Y);
            return Math.Sqrt(x * x + y * y);
        }

        public System.Drawing.PointF ToPointF()
        {
            return new System.Drawing.PointF((float) X, (float) Y);
        }

        public void Draw(Graphics graphics, Pen pen, float r = 2)
        {
            graphics.DrawEllipse(pen, (float)X - r, (float)Y - r, r * 2, r * 2);
        }

        public static (Point p1, Point p2) ABBA(IEnumerable<Point> points)
        {
            var (minX, minY, maxX, maxY) = points.Aggregate(
                (double.MaxValue, double.MaxValue, double.MinValue, double.MinValue),
                (tuple, point) => (Math.Min(tuple.Item1, point.X), Math.Min(tuple.Item2, point.Y),
                    Math.Max(tuple.Item3, point.X), Math.Max(tuple.Item4, point.Y)));
            return (new Point {X = minX, Y = minY}, new Point {X = maxX, Y = maxY});
        }

    }
}