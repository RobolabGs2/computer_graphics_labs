using Lab06.Base3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab06.Tools3D
{
    class JustImage: IToolPage
    {
        public Bitmap Image { get; }

        public JustImage(Bitmap bitmap)
        {
            Image = bitmap;
        }

        public void Init(ToolTab tab, Context context)
        { }
    }
}
