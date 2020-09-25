using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab04.Tools
{
    class Move : ITool
    {
        public Bitmap image => Properties.Resources.Move;

        public void Init(Context context)
        { }
        public Matrix Draw(Point start, Point end, Graphics graphics)
        {
            Matrix m = Matrix.Ident();
            m[2, 0] = end.X - start.X;
            m[2, 1] = end.Y - start.Y;
            return m;
        }
    }
}
