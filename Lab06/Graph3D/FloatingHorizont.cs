using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab06.Base3D;
using Lab06.Materials3D;
using Lab06.AuxTools3D;

namespace Lab06.Graph3D
{
    class FloatingHorizont : IDrawing
    {
        public Context context;
        FastBitmap fastBitmap;
        public Node node;
        int width;
        int height;

        public double xStart;
        public double xEnd;
        public double xStep;
        public double yStart;
        public double yEnd;

        public void Draw(Bitmap bitmap)
        {
            var graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Constants.backColore);
            graphics.Dispose();

            width = bitmap.Width;
            height = bitmap.Height;

            fastBitmap = new FastBitmap(bitmap, System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            DrawHorizont();

            fastBitmap.Dispose();
        }

        void DrawHorizont()
        {
            var maxHor = new double[width];
            var minHor = new double[width];
            for (int i = 0; i < width; i++)
            {
                maxHor[i] = double.MinValue;
                minHor[i] = double.MaxValue;
            }
            var dx = width / 2;
            var dy = height / 2;

            var angle = context.camera.leftAngle;
            var dangle = context.camera.downAngle;
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);
            var dcos = Math.Cos(dangle);
            var dsin = Math.Sin(dangle);
            for (double i = xEnd; i > xStart; i -= xStep)
            {
                for (int y = -width / 2; y < width / 2; y++)
                {
                    var j = (y + yStart) / context.scale;

                    var rotatei = cos * i - sin * j;
                    var rotatej = sin * i + cos * j;
                    
                    if ((int)y + dx >= width || (int)y + dx < 0)
                        continue;
                    var z = dcos * rotatei + dsin * node.Get(rotatei, rotatej);

                    if (minHor[(int)y + dx] > z)
                    {
                        minHor[(int)y + dx] = z;
                        fastBitmap.TrySetPixel((int)y + dx, (int)(z * context.scale) + dy, Constants.borderColore);
                    }

                    if (maxHor[(int)y + dx] < z)
                    {
                        maxHor[(int)y + dx] = z;
                        fastBitmap.TrySetPixel((int)y + dx, (int)(z * context.scale) + dy, Constants.textColore);
                    }
                       
                }

            }
            
        }
    }
}
