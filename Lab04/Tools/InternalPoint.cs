using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab04.Tools
{
    class InternalPoint : ITool
    {
        public Bitmap image => Properties.Resources.Point;
        private Context ctx;
        private readonly Pen beamPen = new Pen(Color.FromArgb(128, 255, 0, 0));
        private readonly Pen debugPenLeft = new Pen(Color.FromArgb(128, 0, 0, 128), 8);
        private readonly Pen debugPenRight = new Pen(Color.FromArgb(128, 0, 128, 0), 8);

        public void Init(Context context)
        {
            ctx = context;
        }

        public Matrix Draw(Point start, Point finish, Graphics graphics)
        {
            finish.Draw(graphics, beamPen);
            if (ctx.Debug)
                graphics.DrawLine(beamPen,
                    (float) finish.X, graphics.ClipBounds.Top,
                    (float) finish.X, graphics.ClipBounds.Bottom);
            var selected = ctx.Polygons
                .Where(ctx.Debug ? DebugInternalPoint(finish, graphics) : p => p.IsInternal(finish)).ToList();
            if ((Control.ModifierKeys & Keys.Shift) != 0)
            {
                ctx.Selected.AddRange(selected);
            }
            else if ((Control.ModifierKeys & Keys.Control) != 0)
            {
                ctx.Selected.RemoveAll(p => selected.Contains(p));
            }
            else
            {
                ctx.Selected = selected;
            }
            return Matrix.Ident();
        }

        private Func<Polygon, bool> DebugInternalPoint(Point finish, Graphics graphics)
        {
            return pol =>
            {
                if (pol.Points.Count < 3)
                    return false;
                var count = pol.Edges().Select(segment =>
                {
                    if (!(finish.X < Math.Max(segment.P1.X, segment.P2.X)
                          && finish.X >= Math.Min(segment.P1.X, segment.P2.X)))
                        return 0;

                    var s = segment.Sign(finish);
                    if (s < 0)
                    {
                        graphics.DrawLine(debugPenLeft, segment.P1.ToPointF(), segment.P2.ToPointF());
                    }
                    else if (s > 0)
                    {
                        graphics.DrawLine(debugPenRight, segment.P1.ToPointF(), segment.P2.ToPointF());
                    }

                    return s;
                }).Sum();
                return count != 0;
            };
        }

        public bool Active()
        {
            return true;
        }
    }
}