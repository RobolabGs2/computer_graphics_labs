using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        //  TODO: Оптимизируй эту хрень!!!
        public (Point p1, Point p2) SelectedABBA()
        {
            if (Selected.Count == 0)
                return (new Point(), new Point());
            Point p1 = new Point
            {
                X = Selected.Select(p => p.ABBA().p1.X).Min(),
                Y = Selected.Select(p => p.ABBA().p1.Y).Min()
            };

            Point p2 = new Point
            {
                X = Selected.Select(p => p.ABBA().p2.X).Max(),
                Y = Selected.Select(p => p.ABBA().p2.Y).Max()
            };

            return (p1, p2);
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
                if(Selected.Contains(p))
                    p.Draw(g, Pen, matrix);
                else
                    p.Draw(g, pen_black);

            pen_black.Dispose();
            //Pen.Dispose();


            var rect = SelectedABBA();
            if(rect.p1 != rect.p2)
            {
                pen_black = new Pen(Color.Gray);
                pen_black.DashStyle = DashStyle.Dash;

                Point p1 = rect.p1* matrix;
                Point p2 = rect.p2  - rect.p1;

                g.DrawRectangle(pen_black, (float)p1.X, (float)p1.Y,
                    (float)p2.X, (float)p2.Y);
                pen_black.Dispose();
            }
        }
    }
}
