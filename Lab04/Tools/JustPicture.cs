using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab04.Tools
{
    class JustPicture : ITool
    {
        public Bitmap image { get; set; }
        
        public JustPicture(Bitmap bmp)
        {
            image = bmp;
        }

        public void Init(Context context)
        {
        }
        public Matrix Draw(Point start, Point end, Graphics graphics)
        {
            return Matrix.Ident();
        }

        public bool Active()
        {
            return true;
        }
    }
}
