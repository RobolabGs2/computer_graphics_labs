using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace Lab04
{
    public class Context
    {
        public List<Polygon> Polygons { get; set; }
        public List<Polygon> Selected { get; set; }
        public Pen Pen { get; set; }
        public Context()
        {
            Polygons = new List<Polygon>();
            Selected = new List<Polygon>();
            Pen = new Pen(Color.Red, 2);

        }

        public (Point p1, Point p2) SelectedABBA(Matrix m = null)
        {
            if (Selected.Count == 0)
                return (new Point(), new Point());
            return Point.ABBA(Selected.SelectMany(p => m == null ? p.Points : p.Points.Select(pt => pt * m)));
        }

        public void Add(Polygon p)
        {
            Polygons.Add(p);
        }

        public void Apply(Matrix matrix)
        {
            foreach (Polygon p in Selected)
                p.Points = p.Points.Select(point => point * matrix).ToList();
        }

        public void Draw(Graphics g, Matrix matrix)
        {
            Pen pen_black = new Pen(Color.Black, 2);

            foreach (Polygon p in Polygons)
                if (Selected.Contains(p))
                {
                    p.Draw(g, Pen, matrix);
                    if (Pen.EndCap == LineCap.Custom)
                    {
                        for (int i = 0; i < p.Points.Count; i++)
                        {
                            g.DrawString(i.ToString(), new Font("Consolas", 12), pen_black.Brush, (p.Points[i] * matrix).ToPointF());
                        }
                    }
                }
                else
                    p.Draw(g, pen_black);

            pen_black.Dispose();
            //Pen.Dispose();


            var rect = SelectedABBA(matrix);
            if (rect.p1 != rect.p2)
            {
                pen_black = new Pen(Color.Gray);
                pen_black.DashStyle = DashStyle.Dash;

                var p1 = rect.p1.ToPointF();
                var p2 = (rect.p2 - rect.p1).ToPointF();

                g.DrawRectangle(pen_black, (float)p1.X, (float)p1.Y,
                    (float)p2.X, (float)p2.Y);
                pen_black.Dispose();
            }
        }
    }
}