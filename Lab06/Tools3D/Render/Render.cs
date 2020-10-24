using Lab06.Base3D;
using Lab06.Graph3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab06.Tools3D.Render
{
    class Render : IToolPage
    {
        public Bitmap Image => Properties.Resources.Render;

        public void Init(ToolTab tab, Context context)
        {
            tab.AddButton(Properties.Resources.Skeleton, false).ButtonClick += b =>
               { 
                   context.drawing = new Skeleton(context, false);
                   context.Redraw();
               };
            tab.AddButton(Properties.Resources.Cube, false).ButtonClick += b =>
            {
                context.drawing = new Skeleton(context, true);
                context.Redraw();
            };
            tab.AddButton(Properties.Resources.Render, false).ButtonClick += b =>
            {
                context.drawing = new ZBuffer(context);
                context.Redraw();
            };
        }
    }
}
