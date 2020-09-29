using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Lab04
{
    public class LineSegment
    {
        public Point P1 { get; }
        public Point P2 { get; }

        // For equation of a line A*x + B*y + C = 0
        public double A => P2.Y - P1.Y;
        public double B => - P2.X + P1.X;
        public double C => P2.X * P1.Y - P1.X * P2.Y;

        public LineSegment(Point p1, Point p2)
        {
            P1 = p1;
            P2 = p2;
        }

        public (Point p, bool onLine) Intersection(LineSegment l)
        {
            double x = (l.C * B - C * l.B ) / (A * l.B - l.A * B);
            double y = -(C + A * x) / B;
            bool onLine = x <= Math.Max(P1.X, P2.X) && x >= Math.Min(P1.X, P2.X)
                && x <= Math.Max(l.P1.X, l.P2.X) && x >= Math.Min(l.P1.X, l.P2.X)
                && y <= Math.Max(P1.Y, P2.Y) && y >= Math.Min(P1.Y, P2.Y)
                && y <= Math.Max(l.P1.Y, l.P2.Y) && y >= Math.Min(l.P1.Y, l.P2.Y);
            return (new Point { X = x, Y = y}, onLine);
        }

        public int Sign(Point p)
        {
            return Math.Sign(A * p.X + B * p.Y + C);
        }
    }
}
