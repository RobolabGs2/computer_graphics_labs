using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab03
{
    class LineDrawing : IDrawingTool
    {
        class NullablePoint
        {
            public int X { get; set; }
            public int Y { get; set; }
            public NullablePoint(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        public string Name => "Line";

        NullablePoint lastPoint = null;

        CheckBox checkbox;

        public Color Color { set; get; }

        public void Init(Control container)
        {
            container.Controls.Add(checkbox = new CheckBox { Text = "Draw", Checked = false});
        }

        public void MoseDown(int x, int y, FastBitmap bitmap)
        {
            if (lastPoint != null)
                DrawLine(x, y, bitmap);
            lastPoint = new NullablePoint(x, y);
        }
        private void DrawLine(double x, double y, FastBitmap bitmap)
        {
            double dx = lastPoint.X - x;
            double dy = lastPoint.Y - y;
            double len = Math.Sqrt(dx * dx + dy * dy);
            for(int i = 0; i < len; ++i)
            {
                bitmap.SetPixel((int)Math.Round(x) , (int)Math.Round(y), Color);
                x += dx / len;
                y += dy / len;
            }
        }

        public void MoseMove(int x, int y, FastBitmap bitmap)
        {
            if (checkbox.Checked)
                MoseDown(x, y, bitmap);
        }

        public void MoseUp(int x, int y, FastBitmap bitmap)
        {
            if (checkbox.Checked)
                lastPoint = null;
        }

        public void Start(FastBitmap bitmap)
        {
            lastPoint = null;
        }
    }
}
