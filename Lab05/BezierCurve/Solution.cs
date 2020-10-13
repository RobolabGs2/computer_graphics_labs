using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab05.BezierCurve
{
    public class Solution : AbstractSolution
    {
        private readonly PictureBox picture = new PictureBox
        {
            Dock = DockStyle.Left,
            BorderStyle = BorderStyle.FixedSingle,
        };
        private readonly Pen pen = new Pen(Color.Black, 2F);
        private readonly Pen dashpen = new Pen(Color.Gray, 1.5F);
        private Bitmap bitmap;

        private BezierCurve bcurve;

        public Solution() : base("Составные кривые Безье")
        {
            bcurve = new BezierCurve();
            var draw = new Button { Text = "Нарисовать", Dock = DockStyle.Bottom, Height = 30, Width = 50};
            var clear = new Button { Text = "Очистить", Dock = DockStyle.Bottom, Height = 30, Width = 50};
            picture.Controls.Add(draw);
            picture.Controls.Add(clear);
            picture.MouseClick += Picture_MouseClick;
            draw.Click += Draw;
            clear.Click += Clear;
        }

        private void Clear(object sender, EventArgs e)
        {
            bitmap?.Dispose();
            bitmap = new Bitmap(picture.Width, picture.Height);
            picture.Image = bitmap;
            bcurve.Clear();
        }

        private void Draw(object sender, EventArgs e)
        {
            bitmap?.Dispose();
            bitmap = new Bitmap(picture.Width, picture.Height);
            dashpen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            using (var g = Graphics.FromImage(bitmap))
            {
                var start = bcurve.points.First;
                float r = 2;
                while (start != bcurve.points.Last)
                {
                    var finish = start.Next;
                    g.DrawLine(bcurve.points.Count == 2? pen : dashpen, start.Value, finish.Value);
                    g.DrawEllipse(pen, start.Value.X - r, start.Value.Y - r, r * 2, r * 2);
                    start = finish;
                }
                g.DrawEllipse(pen, bcurve.points.Last.Value.X - r, bcurve.points.Last.Value.Y - r, r * 2, r * 2);
                bcurve.Draw(g, pen);
            }
          
            picture.Image = bitmap;
        }

        private void Picture_MouseClick(object sender, MouseEventArgs e)
        {
            bcurve.points.AddLast(new Point { X = e.X, Y = e.Y });
            float r = 2;
            Graphics.FromImage(bitmap).DrawEllipse(pen, e.X - r, e.Y - r, r * 2, r * 2);
            picture.Image = bitmap;
        }

        public override Control[] Controls => new[] { picture };

        public override Size Size
        {
            set
            {
                picture.Width = value.Width;
                bitmap?.Dispose();
                bitmap = new Bitmap(picture.Width, picture.Height);
            }
        }
        
    }
}
