using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lab04
{
    public interface ITool
    {
        Bitmap image { get; }
        void Init(Context context);
        Matrix Draw(Point start, Point end, Graphics graphics);
        // true если инструмент остаётся активным, false если оно одноразовое
        bool Active();
    }
}
