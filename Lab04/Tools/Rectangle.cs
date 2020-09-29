using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab04.Tools
{
    class Rectangle: ITool
    {
        public Bitmap image => Properties.Resources.Rectangle;
        
        private Context context;

        public void Init(Context context)
        {
            this.context = context;
        }

        public Matrix Draw(Point start, Point end, Graphics graphics)
        {
            Point p1 = new Point { X = Math.Min(start.X, end.X), Y = Math.Min(start.Y, end.Y) };
            Point p2 = new Point { X = Math.Max(start.X, end.X), Y = Math.Max(start.Y, end.Y) };
            Point del = p2 - p1;
            Pen pen = new Pen(Color.Gray) {DashStyle = System.Drawing.Drawing2D.DashStyle.Dash};
            graphics.DrawRectangle(pen, (float) p1.X, (float)p1.Y, (float) del.X, (float)del.Y);
            pen.Dispose();

            var selected = context.Polygons.Where(p => {
                var abba = p.ABBA();
                return abba.p1.X > p1.X && abba.p1.Y > p1.Y && abba.p2.X < p2.X && abba.p2.Y < p2.Y;
            }).ToList();
            if ((Control.ModifierKeys & Keys.Shift) != 0)
            {
                context.Selected.AddRange(selected);
            }
            else if ((Control.ModifierKeys & Keys.Control) != 0)
            {
                context.Selected.RemoveAll(p => selected.Contains(p));
            }
            else
            {
                context.Selected = selected;
            }
            return Matrix.Ident();
        }
        public bool Active()
        {
            return true;
        }
    }
}
