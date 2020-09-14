using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Lab02
{
    public unsafe class FastBitmap : IDisposable
    {
        private readonly Bitmap _bitmap;

        public readonly BitmapData Data;

        // Bytes per pixel
        public readonly int Bpp;
        public readonly byte* Start;

        public FastBitmap(Bitmap bitmap, ImageLockMode lockMode, PixelFormat pixelFormat)
        {
            _bitmap = bitmap;
            Data = bitmap.LockBits(new Rectangle(new Point(0, 0), bitmap.Size), lockMode, pixelFormat);
            Bpp = Data.Stride / Data.Width;
            Start = (byte*) Data.Scan0;
        }

        public delegate void PixelMap(PixelFormat format, byte* src, byte* dst);

        public Bitmap[] Map(params PixelMap[] map)
        {
            var count = map.Length;
            var result = new Bitmap[count];
            var write = new FastBitmap[count];
            for (var i = 0; i < count; i++)
            {
                var b = new Bitmap(_bitmap.Width, _bitmap.Height);
                result[i] = b;
                write[i] = new FastBitmap(b, ImageLockMode.WriteOnly, Data.PixelFormat);
            }

            var writeRows = new byte*[count];
            for (var y = 0; y < Data.Height; y++)
            {
                var row = Row(y);
                for (var i = 0; i < count; i++)
                    writeRows[i] = write[i].Row(y);
                for (var x = 0; x < Data.Width; x++)
                {
                    var pixel = row + x * Bpp;
                    for (var i = 0; i < count; i++)
                        map[i](Data.PixelFormat, pixel, writeRows[i] + x * write[i].Bpp);
                }
            }

            for (var i = 0; i < count; i++)
                write[i].Dispose();

            return result;
        }

        public Bitmap[] Map(params Func<Color, Color>[] map)
        {
            var mapWithBytes = new PixelMap[map.Length];
            for (var i = 0; i < map.Length; ++i)
            {
                var func = map[i];
                mapWithBytes[i] = (format, source, dst) =>
                {
                    ColorToPixel(format, dst, func(PixelToColor(format, source)));
                };
            }

            return Map(mapWithBytes);
        }

        public byte* Row(int y)
        {
            return Start + y * Data.Stride;
        }

        public byte* GetRawPixel(int x, int y)
        {
            return Start + y * Data.Stride + x * Bpp;
        }

        public Color GetPixel(int x, int y)
        {
            var pixel = GetRawPixel(x, y);
            return PixelToColor(pixel);
        }

        public Color PixelToColor(byte* pixel)
        {
            return PixelToColor(Data.PixelFormat, pixel);
        }

        public static Color PixelToColor(PixelFormat format, byte* pixel)
        {
            switch (format)
            {
                case PixelFormat.Format24bppRgb:
                case PixelFormat.Format32bppRgb:
                    return Color.FromArgb(pixel[0], pixel[1], pixel[2]);
                case PixelFormat.Format32bppArgb:
                    return Color.FromArgb(pixel[3], pixel[0], pixel[1], pixel[2]);
                default:
                    throw new NotSupportedException($"Unsupported pixel format:{format}");
            }
        }

        public void ColorToPixel(byte* pixel, Color color)
        {
            ColorToPixel(Data.PixelFormat, pixel, color);
        }

        public static void ColorToPixel(PixelFormat format, byte* pixel, Color color)
        {
            switch (format)
            {
                case PixelFormat.Format24bppRgb:
                case PixelFormat.Format32bppRgb:
                    pixel[0] = color.R;
                    pixel[1] = color.G;
                    pixel[2] = color.B;
                    break;
                case PixelFormat.Format32bppArgb:
                    pixel[0] = color.R;
                    pixel[1] = color.G;
                    pixel[2] = color.B;
                    pixel[3] = color.A;
                    break;
                default:
                    throw new NotSupportedException($"Unsupported pixel format:{format}");
            }
        }

        public void Dispose()
        {
            _bitmap.UnlockBits(Data);
        }
    }
}