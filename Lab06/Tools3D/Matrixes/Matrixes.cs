using Lab06.Base3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab06.Tools3D.Matrixes
{
    class Matrixes : IToolPage
    {
        Context context;
        public Bitmap Image => Properties.Resources.Matrix;

        public void Init(ToolTab tab, Context context)
        {
            this.context = context;
            tab.AddButton(Properties.Resources.Parallel, false).Click +=
                    (o, s) =>
                    {
                        context.projection = Matrix.Ident();
                        context.Redraw();
                    };
            tab.AddButton(Properties.Resources.Projection, false).Click +=
                    (o, s) =>
                    {
                        context.projection = Matrix.Projection(0.001, 0, 0);
                        context.Redraw();
                    };
            tab.AddButton(Properties.Resources.Reverse, false).Click +=
                    (o, s) =>
                    {
                        context.projection = Matrix.Projection(-0.001, 0, 0);
                        context.Redraw();
                    };
        }
    }
}
