using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Lab04.Tools
{
    class Division : ITool
    {
        public Bitmap image => Properties.Resources.Devision;
        private readonly Pen _leftPen = new Pen(Color.Blue, 3);
        private readonly Pen _rightPen = new Pen(Color.Lime, 3);
        private readonly Pen _linePen = new Pen(Color.Black);
        private readonly Pen _lineFullPen = new Pen(Color.FromArgb(128, 128, 128, 128));

        private Context context;

        public void Init(Context context)
        {
            this.context = context;
        }

        public Matrix Draw(Point start, Point finish, Graphics graphics)
        {
            if (start == finish)
                return Matrix.Ident();
            var lineSegment = new LineSegment(start, finish);
            DrawHelperLine(lineSegment, graphics);
            foreach (var point in context.Polygons.SelectMany(poly => poly.Points))
                point.Draw(graphics, lineSegment.Sign(point) > 0 ? _leftPen : _rightPen);
            return Matrix.Ident();
        }

        private void DrawHelperLine(LineSegment segment, Graphics graphics)
        {
            _linePen.CustomEndCap = new AdjustableArrowCap(5, 5);
            segment.Draw(graphics, _linePen);
            _linePen.EndCap = LineCap.Flat;
            segment.DrawFullLine(graphics, _linePen);
            segment.P1.Draw(graphics, _linePen);
        }

        public bool Active()
        {
            return true;
        }
    }
}