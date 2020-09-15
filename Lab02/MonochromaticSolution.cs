using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Lab02
{
    unsafe class MonochromaticSolution : ISolution
    {
        // Преобразует пиксель из RGB в какой-то один цвет, накапливая его значения в гистограмму
        public class Monochromatic
        {
            private readonly PixelMap _pixelMapper;
            private int[] _hist = new int[256];
            public Color HistogramColor { get; }

            public int[] Histogram
            {
                get
                {
                    var res = _hist;
                    _hist = new int[256];
                    return res;
                }
            }

            // Работает как FastBitmap.PixelMap, но возвращает значение, которое нужно сохранить в гистограмму
            public delegate byte PixelMap(PixelFormat format, byte* src, byte* dst);

            public Monochromatic(string name, PixelMap pixelMapper, Color histogramColor)
            {
                Name = name;
                _pixelMapper = pixelMapper;
                HistogramColor = histogramColor;
            }

            public void PixelMapper(PixelFormat format, byte* src, byte* dst)
            {
                _hist[_pixelMapper(format, src, dst)]++;
            }

            public string Name { get; }
        }

        public string Name { get; }

        private readonly IList<PictureBox> _pictures = new List<PictureBox>();

        private readonly IList<Monochromatic> _monochromatics;

        private Chart _chart;

        public MonochromaticSolution(string name, IList<Monochromatic> monochromatics)
        {
            _monochromatics = monochromatics;
            Name = name;
        }

        // Для указания легенды сериям нужно лишь её название
        private const string LegendName = "Legend";

        public void Init(Control container)
        {
            _chart = new Chart
            {
                Height = container.Height / 2 - 6,
                Width = container.Width - 6
            };
            _chart.Legends.Add(LegendName);
            foreach (var greyazator in _monochromatics)
            {
                var pictureWithDesc = new FlowLayoutPanel
                {
                    Height = container.Height / 2 - 3,
                    Width = container.Width / _monochromatics.Count - 6,
                    FlowDirection = FlowDirection.TopDown
                };
                var textBox = new Label
                {
                    Text = greyazator.Name,

                    Width = pictureWithDesc.Width - 6,
                    Height = 16,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                pictureWithDesc.Controls.Add(textBox);
                var pict = new PictureBox
                {
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Width = pictureWithDesc.Width - 6,
                    Height = pictureWithDesc.Height - 32
                };
                pictureWithDesc.Controls.Add(pict);
                _chart.ChartAreas.Add(greyazator.Name);
                _pictures.Add(pict);
                container.Controls.Add(pictureWithDesc);
            }

            container.Controls.Add(_chart);
        }

        public void Show(FastBitmap bitmap)
        {
            var res = bitmap.Map(_monochromatics
                .Select<Monochromatic, FastBitmap.PixelMap>(monochromatic => monochromatic.PixelMapper).ToArray());
            for (var i = 0; i < _pictures.Count; i++)
            {
                _pictures[i].Image?.Dispose();
                _pictures[i].Image = res[i];
            }

            _chart.Series.Clear();
            foreach (var g in _monochromatics)
            {
                var s = g.Name;
                var series = new Series(s)
                {
                    Name = s,
                    ChartType = SeriesChartType.Area,
                    ChartArea = s,
                    Legend = LegendName,
                    Color = g.HistogramColor,
                };
                foreach (var y in g.Histogram)
                {
                    series.Points.Add(y);
                }
                _chart.Series.Add(series);
                _chart.ChartAreas[s].RecalculateAxesScale();
            }
        }
    }
}