using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab04.Tools
{
    class InternalPoint : ITool
    {
        public Bitmap image => Properties.Resources.Point;
        private Context _ctx;
        private readonly Pen _beamPen = new Pen(Color.FromArgb(128, 255, 0, 0));
        private readonly Pen _debugPen = new Pen(Color.FromArgb(128, 0, 0, 128), 8);
        public void Init(Context context)
        {
            _ctx = context;
        }

        public Matrix Draw(Point start, Point finish, Graphics graphics)
        {
            var debug = "";
            graphics.DrawLine(_beamPen, finish.ToPointF(), new PointF(graphics.ClipBounds.Right, (float) finish.Y));

            _ctx.Selected = _ctx.Polygons.Where(
                pol =>
                {
                    if (pol.Points.Count < 3)
                        return false;
                    var count = pol.Edges().Select(segment =>
                    {
                        if (!(finish.X < Math.Max(segment.P1.X, segment.P2.X)
                            && finish.X >= Math.Min(segment.P1.X, segment.P2.X)))
                            return 0;
                        return segment.Sign(finish);
                    }).Sum();
                    return count != 0;
                }).ToList();
            // _ctx.Selected = _ctx.Polygons.Where(finish => finish.IsInternal(end)).ToList();
            graphics.DrawString(debug, new Font("Consolas", 10), _beamPen.Brush, finish.ToPointF());
            return Matrix.Ident();
        }

        public bool Active()
        {
            return true;
        }
    }
}
