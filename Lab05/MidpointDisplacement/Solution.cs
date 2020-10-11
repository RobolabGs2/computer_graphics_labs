using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Point = System.Drawing.PointF;

namespace Lab05.MidpointDisplacement
{
    public static class PointExt
    {
        private static float Sqr(float x) => x * x;
        private static int Sqr(int x) => x * x;

        public static float DistanceTo(this Point start, Point finish) =>
            (float) Math.Sqrt(Sqr(finish.X - start.X) + Sqr(finish.Y - start.Y));
    }

    public class Solution : AbstractSolution
    {
        private readonly PictureBox canvas = new PictureBox
        {
            Dock = DockStyle.Left,
            BackColor = Color.Beige,
            BorderStyle = BorderStyle.FixedSingle,
        };

        private readonly Control picturePanel;

        private readonly Control panel = new Panel()
        {
            Dock = DockStyle.Right,
            Width = 200,
        };

        private readonly Settings settings = new Settings();
        private MidpointDisplacement line = new MidpointDisplacement(new Settings());

        public Solution() : base("Midpoint displacement")
        {
            picturePanel = new Panel
            {
                Dock = DockStyle.Left,
                Controls =
                {
                    rightTrackBar,
                    canvas,
                    leftTrackBar,
                }
            };
            canvas.Click += (sender, args) =>
            {
                line.Iteration();
                Draw();
            };
            leftTrackBar.Maximum = rightTrackBar.Maximum = settings.Height;
            leftTrackBar.Value = settings.H1;
            rightTrackBar.Value = settings.H2;
            leftTrackBar.ValueChanged += (sender, args) => { settings.H1 = leftTrackBar.Value; };
            rightTrackBar.ValueChanged += (sender, args) => { settings.H2 = rightTrackBar.Value; };
            panel.Controls.AddRange(UIAttribute.GetControlsByProperties(settings).Reverse().SelectMany(tuple =>
            {
                var (label, text, update) = tuple;
                label.Dock = DockStyle.Top;
                label.Height = 32;
                label.Padding = new Padding(0, 12, 0, 0);
                text.Dock = DockStyle.Top;
                // По хорошему надо было у Settings делать не поля, а свойства, и события изменений этих свойств
                if (label.Text.Equals(Settings.LabelH1))
                {
                    leftTrackBar.ValueChanged += (sender, args) => update();
                    text.TextChanged += (sender, args) =>
                        leftTrackBar.Value = Math.Max(0, Math.Min(settings.H1, leftTrackBar.Maximum));
                }

                if (label.Text.Equals(Settings.LabelH2))
                {
                    rightTrackBar.ValueChanged += (sender, args) => update();
                    text.TextChanged += (sender, args) =>
                        rightTrackBar.Value = Math.Max(0, Math.Min(settings.H2, rightTrackBar.Maximum));
                }

                if (label.Text.Equals(Settings.LabelH))
                {
                    text.TextChanged += (sender, args) =>
                    {
                        leftTrackBar.Value = Math.Min(leftTrackBar.Value, settings.Height);
                        rightTrackBar.Value = Math.Min(rightTrackBar.Value, settings.Height);
                        leftTrackBar.Maximum = rightTrackBar.Maximum = settings.Height;
                    };
                }

                return new Control[]
                {
                    text, label,
                };
            }).ToArray());
            var button = new Button {Text = "Применить", Dock = DockStyle.Bottom, Height = 32};
            button.Click += (sender, args) =>
            {
                line = new MidpointDisplacement(settings);
                Draw();
            };
            panel.Controls.Add(button);
        }

        private static TrackBar MakeTrackBar(TickStyle tickStyle)
        {
            return new TrackBar
            {
                Orientation = Orientation.Vertical, TickStyle = tickStyle, Dock = DockStyle.Left,
                Padding = Padding.Empty, Margin = Padding.Empty
            };
        }

        private Bitmap bitmap;

        private readonly Pen pen = new Pen(Color.Black, 1.5F);

        private readonly TrackBar leftTrackBar = MakeTrackBar(TickStyle.Both);
        private readonly TrackBar rightTrackBar = MakeTrackBar(TickStyle.TopLeft);

        void Draw()
        {
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.White);
                graphics.TranslateTransform(0, bitmap.Height);
                graphics.ScaleTransform(1, -1);
                graphics.ScaleTransform(bitmap.Width / (float) settings.Length,
                    bitmap.Height / (float) settings.Height);
                line.Draw(graphics, pen);
            }

            canvas.Image = bitmap;
        }

        public override Control[] Controls => new[] {picturePanel, panel};

        public override Size Size
        {
            set
            {
                picturePanel.Width = value.Width - panel.Width;
                canvas.Width = picturePanel.ClientSize.Width - 2 * leftTrackBar.Width;
                bitmap?.Dispose();
                bitmap = new Bitmap(canvas.Width, canvas.Height);
                Draw();
            }
        }
    }
}