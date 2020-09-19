using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab03
{
    class PixelDrawing : IDrawingTool
    {
        public Color Color { get;  set; }

        public string Name => "Pixel Draw";


        public void Init(Control container)
        { }

        public void MoseDown(int x, int y, FastBitmap bitmap)
        {
            bitmap.SetPixel(x, y, Color);
        }

        public void MoseMove(int x, int y, FastBitmap bitmap)
        {
            MoseDown(x, y, bitmap);
        }

        public void MoseUp(int x, int y, FastBitmap bitmap)
        { }

        public void Start(FastBitmap bitmap)
        { }
    }
}
