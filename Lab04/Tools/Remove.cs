using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab04.Properties;

namespace Lab04.Tools
{
    class Remove: ITool
    {
        public Bitmap image => Resources.Remove;
        private Context context;
        public void Init(Context context)
        {
            this.context = context;
        }
        public Matrix Draw(Point start, Point end, Graphics graphics)
        {
            return Matrix.Ident();
        }

        public bool Active()
        {
            context.Polygons.ExceptWith(context.Selected);
            context.Selected.Clear();
            return false;
        }
    }
}
