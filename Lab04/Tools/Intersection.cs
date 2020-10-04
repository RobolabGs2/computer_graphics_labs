using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab04.Tools
{
    class Intersection : ITool
    {
        public Bitmap image => Properties.Resources.Intersection;
        private Context context;

        public bool Active()
        {
            if (context.Selected.Count < 2)
                return false;

            foreach (var p in context.Selected)
                if (!p.IsConvex())
                {
                    MessageBox.Show("Обнаружен впуклый полигон!!", "Окошко-всплывашка", MessageBoxButtons.OK,
                           MessageBoxIcon.Information);
                    return false;
                }

            Polygon result = context.Selected.Aggregate((p1, p2) => p1.Intersection(p2));
            context.Selected.Clear();
            if(result.Points.Count > 0)
            {
                context.Add(result);
                context.Selected.Add(result);
            }
            return false;
        }

        public Matrix Draw(Point start, Point end, Graphics graphics)
        {
            return Matrix.Ident();
        }

        public void Init(Context context)
        {
            this.context = context;
        }
    }
}
