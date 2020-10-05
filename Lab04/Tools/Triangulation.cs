using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab04.Properties;

namespace Lab04.Tools
{
    class Triangulation : ITool
    {
        public Bitmap image => Resources.Triangulation;
        private Context context;

        public void Init(Context context)
        {
            this.context = context;
        }

        public Matrix Draw(Point start, Point finish, Graphics graphics)
        {
            return Matrix.Ident();
        }

        public bool Active()
        {
            context.Polygons.UnionWith(context.Selected = context.Selected.SelectMany(p => p.Triangulate()).ToHashSet());
            return false;
        }
    }
}