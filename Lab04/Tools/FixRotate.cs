using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab04.Tools
{
    class FixRotate : ITool
    {
        public Bitmap image { get; }
        
        Matrix matrix;
        
        Context context;

        public FixRotate(double angle, Bitmap bitmap)
        {
            image = bitmap;
            matrix = Matrix.Rotate(angle);
        }

        public void Init(Context context)
        {
            this.context = context;
        }

        public bool Active()
        {
            var abba = context.SelectedABBA();
            Point delta = (abba.p1 + abba.p2) / 2;
            context.Apply(Matrix.Move(-delta) * matrix * Matrix.Move(delta));
            return false;
        }

        public Matrix Draw(Point start, Point end, Graphics graphics)
        {
            return Matrix.Ident();
        }
    }
}
