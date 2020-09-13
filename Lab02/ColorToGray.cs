using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms.Layout;

namespace Lab02
{
    class ColorToGray : ISolution
    {
        static byte NTSC_RGB(byte R, byte G, byte B)
        {
            return (byte) (0.3 * R + 0.59 * G + 0.11 * B);
        }

        static byte sRGB(byte R, byte G, byte B)
        {
            return (byte) (0.21 * R + 0.72 * G + 0.07 * B);
        }

        class Greyazator
        {
            private Func<byte, byte, byte, byte> _grayzator;
            private int[] _hist = new int[256];

            public int[] Histogram
            {
                get
                {
                    var res = _hist;
                    _hist = new int[256];
                    return res;
                }
            }

            public Greyazator(string name, Func<byte, byte, byte, byte> grayzator)
            {
                Name = name;
                this._grayzator = grayzator;
            }

            public byte ColorToGray(byte r, byte g, byte b)
            {
                var res = _grayzator(r, g, b);
                _hist[res]++;
                return res;
            }

            public string Name { get; }
        }

        public string Name => "Оттенки серого";

        private IList<PictureBox> pictures = new List<PictureBox>();

        private IList<Greyazator> greyazators = new List<Greyazator>
        {
            new Greyazator("sRGB", sRGB),
            new Greyazator("NTSC RGB", NTSC_RGB),
            new Greyazator("Разность", (r, g, b) => (byte) (Math.Abs(sRGB(r, g, b) - NTSC_RGB(r, g, b))))
        };

        private Chart chart;

        public void Init(Control container)
        {
            foreach (var greyazator in greyazators)
            {
                var pictureWithDesc = new FlowLayoutPanel
                {
                    Height = container.Height / 2 - 3,
                    Width = container.Width / greyazators.Count - 6,
                    FlowDirection = FlowDirection.TopDown
                };
                var textBox = new Label
                {
                    Text = greyazator.Name,

                    Width = pictureWithDesc.Width - 6,
                    Height = 16,
                };
                pictureWithDesc.Controls.Add(textBox);
                var pict = new PictureBox
                {
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Width = pictureWithDesc.Width - 6,
                    Height = pictureWithDesc.Height - 32
                };
                pictureWithDesc.Controls.Add(pict);

                pictures.Add(pict);
                container.Controls.Add(pictureWithDesc);
            }

            container.Controls.Add(chart = new Chart
            {
                Height = container.Height / 2 - 6,
                Width = container.Width - 6
            });
        }

        public unsafe delegate void PixelMap(PixelFormat format, byte* src, byte* dst);

        public void Show(Bitmap bitmap)
        {
            foreach (var picture in pictures)
            {
                picture.Image?.Dispose();
            }

            using (var readBitmap = new FastBitmap(bitmap, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb))
            {
                unsafe
                {
                    var res = readBitmap.Map(greyazators.Select<Greyazator, PixelMap>(greyazator =>
                        (PixelFormat format, byte* src, byte* dst) =>
                            dst[0] = dst[1] = dst[2] = greyazator.ColorToGray(src[0], src[1], src[2])).ToArray());
                    for (var i = 0; i < 3; i++)
                    {
                        pictures[i].Image = res[i];
                    }
                }
            }

            chart.ChartAreas.Clear();
            chart.Series.Clear();
            chart.Legends.Clear();
            var legend = new Legend("Legend");
            chart.Legends.Add(legend);

            foreach (var g in greyazators)
            {
                var s = g.Name;
                var area = new ChartArea(s);
                chart.ChartAreas.Add(area);
                var series = new Series(s)
                {
                    Name = s,
                    ChartType = SeriesChartType.Column,
                    ChartArea = area.Name,
                    Legend = legend.Name,
                };
                var histogram = g.Histogram;
                var sum = histogram.Sum();
                for (var x = 0; x < histogram.Length; ++x)
                {
                    series.Points.Add(histogram[x] * 100 / sum);
                }

                chart.Series.Add(series);
            }
        }
    }

    internal unsafe class FastBitmap : IDisposable
    {
        private Bitmap bitmap;
        public BitmapData data;

        public byte* start;

        // Bytes per pixel
        public int bpp { get; }

        public FastBitmap(Bitmap bitmap, ImageLockMode lockMode, PixelFormat pixelFormat)
        {
            this.bitmap = bitmap;
            data = bitmap.LockBits(new Rectangle(new Point(0, 0), bitmap.Size), lockMode, pixelFormat);
            bpp = data.Stride / data.Width;
            start = (byte*) data.Scan0;
        }

        public Bitmap[] Map(params ColorToGray.PixelMap[] map)
        {
            var count = map.Length;
            var result = new Bitmap[count];
            var write = new FastBitmap[count];
            for (var i = 0; i < count; i++)
            {
                var b = new Bitmap(bitmap.Width, bitmap.Height);
                result[i] = b;
                write[i] = new FastBitmap(b, ImageLockMode.WriteOnly, data.PixelFormat);
            }

            var writeRows = new byte*[count];
            for (var y = 0; y < data.Height; y++)
            {
                var row = Row(y);
                for (var i = 0; i < count; i++)
                    writeRows[i] = write[i].Row(y);

                for (var x = 0; x < data.Width; x++)
                {
                    var pixel = row + x * bpp;
                    for (var i = 0; i < count; i++)
                        map[i](data.PixelFormat, pixel, writeRows[i] + x * write[i].bpp);
                }
            }

            for (var i = 0; i < count; i++)
                write[i].Dispose();

            return result;
        }

        public Bitmap[] Map(params Func<Color, Color>[] map)
        {
            var mapWithBytes = new ColorToGray.PixelMap[map.Length];
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
            return start + y * data.Stride;
        }

        public byte* GetRawPixel(int x, int y)
        {
            return start + y * data.Stride + x * bpp;
        }

        public Color GetPixel(int x, int y)
        {
            var pixel = GetRawPixel(x, y);
            return PixelToColor(pixel);
        }

        public Color PixelToColor(byte* pixel)
        {
            return PixelToColor(data.PixelFormat, pixel);
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
            ColorToPixel(data.PixelFormat, pixel, color);
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
            bitmap.UnlockBits(data);
        }
    }
}