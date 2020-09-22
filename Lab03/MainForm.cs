using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab03
{
    public partial class mainForm : Form
    {
        List<IDrawingTool> tools;
        IDrawingTool Selected;
        ColorBar colorBar;
        Bitmap mainBitmap;
        LinkedList<Bitmap> buffer = new LinkedList<Bitmap>();
        private int Scale = 1;
        public mainForm(List<IDrawingTool> tools)
        {
            InitializeComponent();
            this.tools = tools;

            colorBar = new ColorBar(colorPictureBox, new List<Color>{
                Color.Black, Color.Red, Color.Green, Color.DarkBlue, Color.Yellow,
                Color.White, Color.Orange, Color.Lime, Color.Blue, Color.Gray
            });
            mainPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            mainBitmap = new Bitmap(mainPictureBox.Width/Scale, mainPictureBox.Height/Scale);
            CallForBitmap(bmp => { });

            foreach (IDrawingTool tool in tools)
            {
                tool.Color = Color.Yellow;

                Button button = new Button {
                    Width = toolPanel.Width - 8,
                    Text = tool.Name
                };

                button.Click += (sender, args) =>
                {
                    Selected = tool;
                };
                toolPanel.Controls.Add(button);

                FlowLayoutPanel panel = new FlowLayoutPanel
                {
                    Width = toolPanel.Width - 8,
                    Height = toolPanel.Width / 2
                };
                toolPanel.Controls.Add(panel);

                tool.Init(panel);
            }
            Selected = tools[0];
        }        

        private void colorPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            Color color = colorBar.Click(e.X, e.Y);
            foreach (var tool in tools)
                tool.Color = color;
        }

        private void CallForBitmap(Action<FastBitmap> func)
        {
            FastBitmap fastBitmap = new FastBitmap(mainBitmap, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            func(fastBitmap);
            fastBitmap.Dispose();
            mainPictureBox.Image = mainBitmap;
        }

        private void mainPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                buffer.AddLast((Bitmap)mainBitmap.Clone());
                if (buffer.Count > 100)
                {
                    buffer.First().Dispose();
                    buffer.RemoveFirst();
                }
                CallForBitmap(bmp => Selected.MouseDown(e.X/Scale, e.Y/Scale, bmp));
            }
            else if (e.Button == MouseButtons.Right)
                CallForBitmap(bmp => { Selected.Start(bmp); });
        }

        private void mainPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                CallForBitmap(bmp => Selected.MouseUp(e.X/Scale, e.Y/Scale, bmp));
        }

        private void mainPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X < 0 || e.Y < 0 || e.X >= mainPictureBox.Width || e.Y >= mainPictureBox.Height)
                return;
            if (e.Button == MouseButtons.Left)
                CallForBitmap(bmp => Selected.MouseMove(e.X/Scale, e.Y/Scale, bmp));
        }

        private void mainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 26 && Control.ModifierKeys == Keys.Control && buffer.Count > 0)
            {
                mainBitmap?.Dispose();
                mainBitmap = buffer.Last();
                buffer.RemoveLast();
                mainPictureBox.Image = mainBitmap;
            }
        }
    }

    class ColorBar
    {
        private PictureBox picture;
        List<Color> colors;
        int x, y;
        int rowCount;
        int colCount;
        int boxSide;

        public ColorBar(PictureBox picture, List<Color> colors)
        {
            this.picture = picture;
            this.colors = colors;
            rowCount = (int)Math.Ceiling(Math.Sqrt(colors.Count * picture.Width / picture.Height));
            double x = (double)colors.Count / rowCount;
            colCount = (int)Math.Ceiling(x);
            boxSide = Math.Min(picture.Width / rowCount, picture.Height / colCount);
            picture.Paint += Paint;
            picture.Size = new Size(rowCount * boxSide, colCount * boxSide);
            x = 0;
            y = 0;
        }

        public Color Click(int x, int y)
        {
            this.x = x; this.y = y;
            Graphics g = picture.CreateGraphics();
            Paint(null, new PaintEventArgs(g, new Rectangle()));
            g.Dispose();
            int row = 0, col = 0;
            foreach (Color color in colors)
            {
                if (boxSide * row <= x && x < boxSide * row + boxSide
                    && boxSide * col <= y && y < boxSide * col + boxSide)
                    return color;
                row += 1;
                if (row >= rowCount)
                {
                    row = 0;
                    col += 1;
                }
            }
            return Color.Black;
        }

        private void Paint(object sender, PaintEventArgs e)
        {
            SolidBrush whiteBrush = new SolidBrush(Color.White);
            int row = 0, col = 0;
            foreach (Color color in colors)
            {
                e.Graphics.FillRectangle(whiteBrush, boxSide * row, boxSide * col, boxSide, boxSide);
                using (SolidBrush brush = new SolidBrush(color))
                    e.Graphics.FillRectangle(brush, boxSide * row + 2, boxSide * col + 2, boxSide - 4, boxSide - 4);

                if (boxSide * row <= x && x < boxSide * row + boxSide
                    && boxSide * col <= y && y < boxSide * col + boxSide)
                    e.Graphics.DrawRectangle(new Pen(Color.Black, 2), boxSide * row + 1, boxSide * col + 1, boxSide - 2, boxSide - 2);
                row += 1;
                if (row >= rowCount)
                {
                    row = 0;
                    col += 1;
                }
            }
            whiteBrush.Dispose();
        }
    }
}
