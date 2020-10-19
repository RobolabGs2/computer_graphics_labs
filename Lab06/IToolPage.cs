using Lab06.Base3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab06
{
    public interface IToolPage
    {
        Bitmap Image { get; }

        void Init(ToolTab tab, Context context);
    }
}
