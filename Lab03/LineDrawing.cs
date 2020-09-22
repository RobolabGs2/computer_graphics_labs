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
        public string Name => "Line";

        Point? lastPoint = null;

        CheckBox drawing; //включение/выключение свободного рисования

        CheckBox smoothing; //включение/выключение сглаживания(алгоритм Ву

        CheckBox circle; //включение/выключение рисования кругов

        public Color Color { set; get; }
        static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        public void Init(Control container)
        {
            container.Controls.Add(drawing = new CheckBox { Text = "Draw", Checked = false });
            container.Controls.Add(smoothing = new CheckBox { Text = "Smoothing", Checked = false});
            container.Controls.Add(circle = new CheckBox { Text = "Circle", Checked = false });
            circle.CheckedChanged += Circle_CheckedChanged;
        }
        private void Circle_CheckedChanged(Object sender, EventArgs e)
        {

            if (circle.Checked)
            {
                drawing.Checked = false;
                drawing.AutoCheck = false;
                smoothing.Checked = false;
                smoothing.AutoCheck = false;
            }
            else
            {
                drawing.AutoCheck = true;
                smoothing.AutoCheck = true;
            }
        }
        public void MouseDown(int x, int y, FastBitmap bitmap)
        {
            if (circle.Checked)
            {
                if (lastPoint != null)
                {
                    BresenhamCircle(x, y, bitmap);
                    lastPoint = null;
                }
                else
                    lastPoint = new Point(x, y);
            }
            else
            {
                if (lastPoint != null)
                    if (smoothing.Checked)
                        WuLine(x, y, bitmap);
                    else BresenhamLine(x, y, bitmap);
                lastPoint = new Point(x, y);
            }
        }
        private void BresenhamLine(int x0, int y0, FastBitmap bitmap)
        {
            int x1 = lastPoint.Value.X;
            int y1 = lastPoint.Value.Y;

            int deltax = Math.Abs(x1 - x0);
            int deltay = Math.Abs(y1 - y0);

            int stepx = x0 < x1 ? 1 : -1; //направление роста координат (шаг)
            int stepy = y0 < y1 ? 1 : -1; 
            int error = (deltax > deltay ? deltax : -deltay) / 2; //эффективнее работает
            for (; ;)
            {
                bitmap.SetPixel(x0, y0, Color);
                if (x0 == x1 && y0 == y1) break; //выход, когда пришли от начала к конечной точке
                int deltaerr = error;
                if (deltaerr > -deltax)
                {
                    error -= deltay;
                    x0 += stepx; 
                }
                if (deltaerr < deltay)
                {
                    error += deltax;
                    y0 += stepy; 
                }
            }
                
        }
        void BresenhamCircle(int x0, int y0, FastBitmap bitmap)
        {
            int x = 0;
            int y = (int)Math.Sqrt(Math.Pow(lastPoint.Value.X - x0, 2) + Math.Pow(lastPoint.Value.Y - y0, 2));
            int delta = 1 - 2 * y;
            int error = 0;
            while (y >= 0)
            {

                bitmap.TrySetPixel(x0 + x, y0 + y, Color);
                bitmap.TrySetPixel(x0 + x, y0 - y, Color);

                bitmap.TrySetPixel(x0 - x, y0 + y, Color);
                bitmap.TrySetPixel(x0 - x, y0 - y, Color);


                error = 2 * (delta + y) - 1;
                if (delta < 0 && error <= 0)
                {
                    ++x;
                    delta += 2 * x + 1;
                    continue;
                }
                error = 2 * (delta - x) - 1;
                if (delta > 0 && error > 0)
                {
                    --y;
                    delta += 1 - 2 * y;
                    continue;
                }
                ++x;
                delta += 2 * (x - y);
                --y;
            }
        }
        int Round(double x) 
        { 
            return (int)(x + 0.5); 
        }

        double getFracPart(double x)
        {
            if (x < 0) return (1 - (x - Math.Floor(x)));
            return (x - Math.Floor(x));
        }

        //возвращает новый цвет для пикселя в соответствии с интенсивностью 
        Color getColor(double intensity, Color c)
        {
            return Double.IsNaN(intensity) ? Color : Color.FromArgb((int)(c.R * (1 - intensity) + Color.R * intensity),
                (int)(c.G * (1 - intensity) + Color.G * intensity),
                (int)(c.B * (1 - intensity) + Color.B * intensity));
        }

        public void WuLine(int x0, int y0, FastBitmap bitmap)
        {
            int x1 = lastPoint.Value.X;
            int y1 = lastPoint.Value.Y;

            bool angle = Math.Abs(y1 - y0) > Math.Abs(x1 - x0); // проверка угла наклона
            if (angle)
            {
                Swap(ref x0, ref y0);
                Swap(ref x1, ref y1);
            }
            if (x0 > x1) //направление по ox
            {
                Swap(ref x0, ref x1);
                Swap(ref y0, ref y1);
            }

            //закрашиваем пиксель для начальной точки
            double deltax = x1 - x0;
            double deltay = y1 - y0;
            double gradient = deltay / deltax;

            double xEnd = Round(x0);
            double yEnd = y0 + gradient * (xEnd - x0);
            double xGap = 1 - getFracPart(x0 + 0.5);
            double xPixel1 = xEnd;
            double yPixel1 = (int)yEnd;

            if (angle)
            {
                if (xPixel1 < bitmap.Width && yPixel1 < bitmap.Height - 1
                        && xPixel1 > 0 && yPixel1 > 0)
                {
                    bitmap.SetPixel((int)yPixel1, (int)xPixel1, getColor((1 - getFracPart(yEnd)) * xGap, bitmap.GetPixel((int)yPixel1, (int)xPixel1)));
                    bitmap.SetPixel((int)yPixel1 + 1, (int)xPixel1, getColor(getFracPart(yEnd) * xGap, bitmap.GetPixel((int)yPixel1 + 1, (int)xPixel1)));
                }
            }
            else
            {
                if (xPixel1 < bitmap.Width && yPixel1 < bitmap.Height - 1
                        && xPixel1 > 0 && yPixel1 > 0)
                {
                    bitmap.SetPixel((int)xPixel1, (int)yPixel1, getColor((1 - getFracPart(yEnd)) * xGap, bitmap.GetPixel((int)xPixel1, (int)yPixel1)));
                    bitmap.SetPixel((int)xPixel1, (int)yPixel1 + 1, getColor(getFracPart(yEnd) * xGap, bitmap.GetPixel((int)xPixel1, (int)yPixel1 + 1)));
                }
            }
            double y = yEnd + gradient;

            //закрашиваем пиксель для конечной точки
            xEnd = Round(x1);
            yEnd = y1 + gradient * (xEnd - x1);
            xGap = getFracPart(x1 + 0.5);
            double xPixel2 = xEnd;
            double yPixel2 = (int)yEnd;
            if (angle)
            {
                if (yPixel2 < bitmap.Width - 1 && xPixel2 < bitmap.Height
                        && xPixel2 >= 0 && yPixel2 >= 0)
                {
                    bitmap.SetPixel((int)yPixel2, (int)xPixel2, getColor((1 - getFracPart(yEnd)) * xGap, bitmap.GetPixel((int)yPixel2, (int)xPixel2)));
                    bitmap.SetPixel((int)yPixel2 + 1, (int)xPixel2, getColor(getFracPart(yEnd) * xGap, bitmap.GetPixel((int)yPixel2 + 1, (int)xPixel2)));
                }
            }
            else
            {
                if (xPixel2 < bitmap.Width && yPixel2 < bitmap.Height - 1
                        && xPixel2 >= 0 && yPixel2 >= 0)
                {
                    bitmap.SetPixel((int)xPixel2, (int)yPixel2, getColor((1 - getFracPart(yEnd)) * xGap, bitmap.GetPixel((int)xPixel2, (int)yPixel2)));
                    bitmap.SetPixel((int)xPixel2, (int)yPixel2 + 1, getColor(getFracPart(yEnd) * xGap, bitmap.GetPixel((int)xPixel2, (int)yPixel2 + 1)));
                }
            }

            //закрашиваем пиксели от начала до конца 
            if (angle)
                for (int x = (int)(xPixel1 + 1); x <= xPixel2 - 1; x++)
                {
                    if (y < bitmap.Width - 1 && x < bitmap.Height
                        && x >= 0 && y >= 0)
                    {
                        bitmap.SetPixel((int)y, x, getColor(1 - (y - (int)y), bitmap.GetPixel((int)y, x)));
                        bitmap.SetPixel((int)y + 1, x, getColor(y - (int)y, bitmap.GetPixel((int)y + 1, x)));
                    }
                    y += gradient;
                }
            else
                for (int x = (int)(xPixel1 + 1); x <= xPixel2 - 1; x++)
                {
                    if (x < bitmap.Width && y < bitmap.Height  - 1
                        && x >= 0 && y >= 0)
                    {
                        bitmap.SetPixel(x, (int)y, getColor(1 - (y - (int)y), bitmap.GetPixel(x, (int)y)));
                        bitmap.SetPixel(x, (int)y + 1, getColor(y - (int)y, bitmap.GetPixel((int)x, (int)y + 1)));
                    }
                    y += gradient;
                }
        }
        public void MouseMove(int x, int y, FastBitmap bitmap)
        {
            if (drawing.Checked)
                MouseDown(x, y, bitmap);
        }

        public void MouseUp(int x, int y, FastBitmap bitmap)
        {
            if (drawing.Checked)
                lastPoint = null;
        }

        public void Start(FastBitmap bitmap)
        {
            lastPoint = null;
        }
    }
}
