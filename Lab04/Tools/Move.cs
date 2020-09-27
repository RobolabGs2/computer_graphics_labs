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
            return Matrix.Move(end - start);
        }
        public bool Active()
        {
            return true;
        }
    }
}
