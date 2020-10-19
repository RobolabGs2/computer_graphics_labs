using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Windows.Forms.VisualStyles;

namespace Lab05.BezierCurve
{
    class BezierCurve
    {
        public readonly LinkedList<PointF> points;
        private static List<List<int>> matrix = new List<List<int>> {new List<int>{  1, -3,  3, -1 }, //правый верхний 1 на -1 и 6 на -6
                                                                     new List<int>{  0,  3, -6,  3 },
                                                                     new List<int>{  0,  0,  3, -3 },
                                                                     new List<int>{  0,  0,  0,  1 } };
        public BezierCurve()
        {
            points = new LinkedList<PointF>();
        }

        public void addPoint(PointF p)
        {
            points.AddLast(p);
        }

        public void Clear()
        {
            points.Clear();
        }
        public int Count()
        {
            return points.Count;
        }
        private PointF Bezier3(PointF p0, PointF p1, PointF p2, PointF p3, float t)
        {
            List<PointF> mt = new List<PointF> { p0, p1, p2, p3 };
            List<PointF> res = new List<PointF>();
            PointF r = new PointF();
            for (int i = 0; i < matrix.Count; i++)
            {
                float x = 0;
                float y = 0;
                for (int j = 0; j < matrix[i].Count; j++)
                {
                    x += mt[j].X * matrix[j][i];
                    y += mt[j].Y * matrix[j][i];
                }
                res.Add(new PointF(x, y));
            }
            for (int i = 0; i < res.Count; i++)
            {
                r.X += res[i].X * (float)Math.Pow(t, i);
                r.Y += res[i].Y * (float)Math.Pow(t, i);
            }
            return r;
            /*return new PointF((float)(Math.Pow((1 - t), 3) * p0.X + 3 * t * Math.Pow((1 - t), 2) * p1.X + 3 * (1 - t) * Math.Pow(t, 2) * p2.X + Math.Pow(t, 3) * p3.X),
            (float)(Math.Pow((1 - t), 3) * p0.Y + 3 * t * Math.Pow((1 - t), 2) * p1.Y + 3 * (1 - t) * Math.Pow(t, 2) * p2.Y + Math.Pow(t, 3) * p3.Y));*/
        }
        public void Draw(Graphics g, Pen pen, bool ellipse = false)
        {
            var copypoints = new LinkedList<PointF>();
            int count = (points.Count + 1) / 2 - 1;
            bool flag = points.Count % 2 != 0;
            foreach(var point in points)
            {
                copypoints.AddLast(point);
            }
            LinkedListNode<PointF> p0 = copypoints.First;
            while (count != 0)
            {
                LinkedListNode<PointF> p1, p2, p3;
                p1 = p0.Next;
                p2 = p1.Next;
                if (flag && count == 1)
                    p3 = p2;
                else if (!flag && count == 1 && !ellipse)
                {
                    p2 = p2.Next;
                    p3 = p2;
                }
                else if (!flag && count == 1 && ellipse)
                {
                    p2 = p1;
                    p3 = p2.Next;
                    count++;
                    flag = true;
                }
                else p3 = p2.Next;
                var p = new PointF((p2.Value.X + p3.Value.X) / 2, (p2.Value.Y + p3.Value.Y) / 2); //p'

                PointF q0 = p0.Value;
                for (float t = (float)0.12; t <= 1.0; t += (float)0.02)
                {
                    var q1 = Bezier3(p0.Value, p1.Value, p2.Value, p, t);
                    g.DrawLine(pen, q0, q1);
                    q0 = q1;
                }
                g.DrawEllipse(new Pen(Color.Red, 4), p.X - 2, p.Y - 2, 4, 4);
                p0 = copypoints.AddAfter(p2, p); //p'
                count--;
            }
        }
    }
}
