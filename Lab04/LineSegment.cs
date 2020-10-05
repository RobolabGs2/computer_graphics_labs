using System;
using System.Collections.Generic;
using System.Drawing;
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
            double denum = (A * l.B - l.A * B);
            if (denum == 0)
                return (new Point { X = double.NaN, Y = double.NaN }, false);
            double x = (l.C * B - C * l.B ) / denum;
            double y = -(l.C * A - C * l.A) / denum;
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

        public double Distance(Point p)
        {
            return (A * p.X + B * p.Y + C)/Math.Sqrt(A*A+B*B);
        }

        public void Draw(Graphics graphics, Pen pen)
        {
            graphics.DrawLine(pen, (float)P1.X, (float)P1.Y, (float)P2.X, (float)P2.Y);
        }

        public void DrawFullLine(Graphics graphics, Pen pen)
        {
            var bounds = graphics.VisibleClipBounds;
            if (Math.Abs(P1.X - P2.X) < 1000*double.Epsilon)
            {
                graphics.DrawLine(pen, (float)P1.X, bounds.Top, (float)P1.X, bounds.Bottom);
                return;
            }
            graphics.DrawLine(pen, bounds.Left, (float)CalcY(bounds.Left), bounds.Right, (float)CalcY(bounds.Right));
        }

        public bool aimsAt(LineSegment other)
        {
            double s1 = other.A * P1.X + other.B * P1.Y + other.C;
            double s2 = other.A * P2.X + other.B * P2.Y + other.C;
            return  Math.Sign(s1) == Math.Sign(s2) && Math.Abs(s1) >= Math.Abs(s2);
        }

        public double CalcY(double x)
        {
            return -(A * x + C) / B;
        }

        public double CalcX(double y)
        {
            return -(B * y + C) / A;
        }
    }
}
