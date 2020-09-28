using Lab04.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab04.Tools
{
    class Arrow : ITool
    {
        public Bitmap image => Resources.Arrow;
        Context _context;
        AdjustableArrowCap bigArrow;
        public bool Active()
        {

            _context.Pen.EndCap = _context.Pen.EndCap == LineCap.Custom ? LineCap.Flat : LineCap.Custom;

            return false;
        }   

        public Matrix Draw(Point start, Point end, Graphics graphics)
        {
            return Matrix.Ident();
        }

        public void Init(Context context)
        {
            this._context = context;
            bigArrow = new AdjustableArrowCap(5, 5);
            _context.Pen.CustomEndCap = bigArrow;
        }
    }
}
