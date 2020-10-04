using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab04.Properties;

namespace Lab04.Tools
{
    class SelectAll: ITool
    {
        public Bitmap image => Resources.SelectAll;
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
            if (context.Selected.Count == context.Polygons.Count)
            {
                context.Selected.Clear();
                return false;
            }
            context.Selected.Clear();
            context.Selected.UnionWith(context.Polygons);
            return false;
        }
    }
}
