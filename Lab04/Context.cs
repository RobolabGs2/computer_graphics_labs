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

        public Context()
        {
            Polygons = new List<Polygon>();
            Selected = new List<Polygon>();
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
            Pen pen = new Pen(Color.Black, 2);
            Pen selected_pen = new Pen(Color.Red, 2);

            foreach (Polygon p in Polygons)
                if (Selected.Contains(p))
                    p.Draw(g, selected_pen, matrix);
                else
                    p.Draw(g, pen);

            pen.Dispose();
            selected_pen.Dispose();


            var rect = SelectedABBA(matrix);
            if (rect.p1 != rect.p2)
            {
                pen = new Pen(Color.Gray);
                pen.DashStyle = DashStyle.Dash;

                var p1 = rect.p1.ToPointF();
                var p2 = (rect.p2 - rect.p1).ToPointF();

                g.DrawRectangle(pen, p1.X, p1.Y, p2.X, p2.Y);
                pen.Dispose();
            }
        }
    }
}