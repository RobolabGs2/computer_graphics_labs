using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Lab03
{
    class FillTool : IDrawingTool
    {
        public string Name => "Fill";

        public Color Color { get; set; }
        CheckBox usePicture;
        PictureBox picture;
        private Bitmap bmp;

        public void Init(Control container)
        {
            Button button = new Button
            {
                Width = container.Width * 3 / 4 - 8,
                Text = "Load Picture"
            };

            usePicture = new CheckBox
            {
                Width = container.Width / 4 - 8
            };

            picture = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Width = container.Width - 8,
                Height = container.Height - button.Height - 16
            };

            button.Click += LoadClick;

            container.Controls.Add(button);
            container.Controls.Add(usePicture);
            container.Controls.Add(picture);
        }


        private void Push(Stack<Point> points, Point p, int maxW, int maxH)
        {
            if (p.X < 0 || p.X >= maxW || p.Y < 0 || p.Y >= maxH)
                return;
            points.Push(p);
        }

        private void Fill(int x, int y, FastBitmap bitmap, Func<int, int, Color> func)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            
            bool[,] flags = new bool[width, height];
            Color replaced = bitmap.GetPixel(x, y);
            Stack<Point> points = new Stack<Point>();
            points.Push(new Point(x, y));
            while(points.Count > 0)
            {
                Point p = points.Pop();
                if (flags[p.X, p.Y]) continue;
                flags[p.X, p.Y] = true;
                if (bitmap.GetPixel(p.X, p.Y) != replaced) continue;
                bitmap.SetPixel(p.X, p.Y, func(p.X, p.Y));
                Push(points, new Point(p.X, p.Y + 1), width, height);
                Push(points, new Point(p.X, p.Y - 1), width, height);
                Push(points, new Point(p.X + 1, p.Y), width, height);
                Push(points, new Point(p.X - 1, p.Y), width, height);
            }
        }

        public void MouseDown(int x, int y, FastBitmap bitmap)
        {
            if (!usePicture.Checked)
            {
                Fill(x, y, bitmap, (X, Y) => Color);
                return;
            }
            if (bmp == null)
            {
                MessageBox.Show("Вы не выбрали картинку!", "Окошко-всплывашка", MessageBoxButtons.OK,
                             MessageBoxIcon.Error);
                return;
            }
            FastBitmap fastBitmap = new FastBitmap(bmp, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            int addX = fastBitmap.Width + bitmap.Width - x;
            int addY = fastBitmap.Height + bitmap.Height - y;
            Fill(x, y, bitmap, (X, Y) => fastBitmap.GetPixel((X + addX) % fastBitmap.Width, 
                (Y + addY) % fastBitmap.Height));
            fastBitmap.Dispose();

        }

        public void MouseMove(int x, int y, FastBitmap bitmap)
        { }

        public void MouseUp(int x, int y, FastBitmap bitmap)
        { }

        public void Start(FastBitmap bitmap)
        { }

        private void LoadClick(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    Image img;
                    try
                    { img = Image.FromFile(filePath); }
                    catch
                    {
                        MessageBox.Show("Не вышло загрузить картинку :(", "Окошко-всплывашка", MessageBoxButtons.OK,
                               MessageBoxIcon.Information);
                        return;
                    }
                    bmp?.Dispose();
                    bmp = new Bitmap(img);
                    picture.Image = bmp;
                }
            }
        }
    }
}
