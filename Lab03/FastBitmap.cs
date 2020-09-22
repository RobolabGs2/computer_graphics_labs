using System;
using System.Drawing;
using System.Drawing.Imaging;

//  TODO: вынести этот чёртов битмап в какой-нибудь отдельный проект с тулсами
namespace Lab03
{
    // Не enum, потому что используется как индекс при работе с массивами
    public static class ColorChannel
    {
        public const int R = 2;
        public const int G = 1;
        public const int B = 0;
        public const int A = 3;
    }
    public unsafe class FastBitmap : IDisposable
    {

        private readonly Bitmap _bitmap;

        public readonly BitmapData Data;

        // Bytes per pixel
        public readonly int Bpp;
        public readonly byte* Start;
        public int Width => _bitmap.Width;
        public int Height => _bitmap.Height;

        public FastBitmap(Bitmap bitmap, ImageLockMode lockMode, PixelFormat pixelFormat)
        {
            _bitmap = bitmap;
            Data = bitmap.LockBits(new Rectangle(new Point(0, 0), bitmap.Size), lockMode, pixelFormat);
            Bpp = Data.Stride / Data.Width;
            Start = (byte*) Data.Scan0;
        }

        public unsafe struct MyColor
        {
            private byte* data;
            public MyColor(byte* data)
            {
                this.data = data;
            }

            public byte A => data[ColorChannel.A];
            public byte R => data[ColorChannel.R];
            public byte G => data[ColorChannel.G];
            public byte B => data[ColorChannel.B];
            public void Set(byte r, byte g, byte b)
            {
                data[ColorChannel.R] = r;
                data[ColorChannel.G] = g;
                data[ColorChannel.B] = b;
            }
            public void Set(byte a, byte r, byte g, byte b)
            {
                data[ColorChannel.A] = a;
                Set(r, g, b);
            }
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

        public Bitmap[] Map(params Action<PixelFormat, MyColor, MyColor>[] map)
        {
            PixelMap[] pixmap = new PixelMap[map.Length];
            for (int i = 0; i < map.Length; ++i)
            {
                var func = map[i];
                pixmap[i] = (pc, src, dst) => 
                        func(pc, new MyColor(src), new MyColor(dst));
            }
            return Map(pixmap);
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
        public void SetPixel(int x, int y, Color color)
        {
            var pixel = GetRawPixel(x, y);
            ColorToPixel(pixel, color);
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
                    return Color.FromArgb(pixel[ColorChannel.R], pixel[ColorChannel.G], pixel[ColorChannel.B]);
                case PixelFormat.Format32bppArgb:
                    return Color.FromArgb(pixel[ColorChannel.A], pixel[ColorChannel.R], pixel[ColorChannel.G], pixel[ColorChannel.B]);
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
                    pixel[ColorChannel.R] = color.R;
                    pixel[ColorChannel.G] = color.G;
                    pixel[ColorChannel.B] = color.B;
                    break;
                case PixelFormat.Format32bppArgb:
                    pixel[ColorChannel.R] = color.R;
                    pixel[ColorChannel.G] = color.G;
                    pixel[ColorChannel.B] = color.B;
                    pixel[ColorChannel.A] = color.A;
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