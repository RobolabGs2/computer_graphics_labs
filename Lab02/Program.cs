using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Lab02
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var mainForm = new MainForm(new List<ISolution>
            {
                new StubSolution(), new StubSolution(bitmap =>
                {
                    var res = (Bitmap) bitmap.Clone();
                    res.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    return res;
                }, "Поворот"),
                new EmptySolution()
            });
            Application.Run(mainForm);
        }
    }

    // Только для тестов
    class StubSolution : ISolution
    {
        public StubSolution(Func<Bitmap, Bitmap> transform = null, string name = "Test solution")
        {
            Name = name;
            _transform = transform;
        }

        public string Name { get; }
        private readonly PictureBox _pictureBox = new PictureBox
        {
            SizeMode = PictureBoxSizeMode.Zoom
        };
        private readonly Func<Bitmap, Bitmap> _transform;

        public void Init(Control container)
        {
            _pictureBox.Size = container.Size;
            container.Controls.Add(_pictureBox);
        }

        public void Show(Bitmap bitmap)
        {
            if (_transform != null)
            {
                bitmap = _transform(bitmap);
            }

            _pictureBox.Image = bitmap;
        }
    }

    class EmptySolution : ISolution
    {
        public EmptySolution(string name = "Empty")
        {
            Name = name;
        }

        public string Name { get; }

        public void Init(Control container)
        {
        }

        public void Show(Bitmap bitmap)
        {
        }
    }
}