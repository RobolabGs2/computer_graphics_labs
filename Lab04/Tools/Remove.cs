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
        private Context _context;
        public void Init(Context context)
        {
            _context = context;
        }
        public Matrix Draw(Point start, Point end, Graphics graphics)
        {
            return Matrix.Ident();
        }

        public bool Active()
        {
            _context.Polygons.RemoveAll(polygon => _context.Selected.Contains(polygon));
            _context.Selected.Clear();
            return false;
        }
    }
}
