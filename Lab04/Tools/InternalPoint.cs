using System;
using System.Collections.Generic;
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
                    var count = pol.Edges().Where(segment =>
                    {
                        if (segment.P1 == segment.P2)
                        {
                            return false;
                        }
                        if (finish.Y < Math.Min(segment.P1.Y, segment.P2.Y) ||
                            finish.Y > Math.Max(segment.P1.Y, segment.P2.Y) ||
                            finish.X > Math.Max(segment.P1.X, segment.P2.X))
                        {
                            return false;
                        }

                        if (finish.X < Math.Min(segment.P1.X, segment.P2.X))
                        {
                            return true;
                        }

                        if (segment.P1.Y > segment.P2.Y)
                        {
                            segment = new LineSegment(segment.P2, segment.P1);
                        }

                        if (Math.Abs(finish.Y - segment.P1.Y) < 0.0001)
                        {
                            return false;
                        }

                        if (Math.Abs(finish.Y - segment.P2.Y) < 0.0001)
                        {
                            return true;
                        }

                        // if (segment
                        // .Intersection(new LineSegment(finish,
                        // new Point {X = Math.Max(segment.P1.X, segment.P2.X) + 100F, Y = finish.Y}))
                        // .onLine)
                        // {
                        // return true;
                        // }

                        // return false;
                        return segment.Sign(finish) <= 0;
                    }).Select(seg =>
                    {
                        graphics.DrawLine(_debugPen, seg.P1.ToPointF(), seg.P2.ToPointF());
                        return seg;
                    }).Count();
                    debug += "\n" + count.ToString();
                    return count % 2 != 0;
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
