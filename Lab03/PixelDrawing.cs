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

        public void MouseDown(int x, int y, FastBitmap bitmap)
        {
            bitmap.SetPixel(x, y, Color);
        }

        public void MouseMove(int x, int y, FastBitmap bitmap)
        {
            MouseDown(x, y, bitmap);
        }

        public void MouseUp(int x, int y, FastBitmap bitmap)
        { }

        public void Start(FastBitmap bitmap)
        { }
    }
}
