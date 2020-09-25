using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab04
{
    public partial class MainForm : Form
    {
        ITool currentTool;
        Tools.PolygonsDrawing drawing;
        Context context;
        Bitmap bitmap;
        Matrix lastMatrix;
        Point startPoint;

        public MainForm(List<ITool> tools)
        {
            InitializeComponent();
            drawing = new Tools.PolygonsDrawing();
            context = new Context();
            bitmap = new Bitmap(mainPictureBox.Width, mainPictureBox.Height);
            currentTool = drawing;

            tools.Add(drawing);
            foreach(ITool tool in tools)
            {
                tool.Init(context);

                Button button = new Button
                {
                    Width = mainPanel.Height,
                    Height = mainPanel.Height,
                    Margin = new Padding(20, 0, 0, 0),
                    FlatStyle = FlatStyle.Popup,
                    BackgroundImage = tool.image,
                    BackgroundImageLayout = ImageLayout.Zoom,
                    BackColor = Color.FromArgb(0, 0, 0, 0),
                    TabStop = false
                };

                button.Click += (a, b) => { currentTool = tool; };
                mainPanel.Controls.Add(button);
            }
        }

        void DrawContext(Graphics g)
        {
            g.Clear(Color.White);
            context.Draw(g, lastMatrix);
        }

        private void mainPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                Graphics g = Graphics.FromImage(bitmap);
                DrawContext(g);
                if (currentTool == drawing)
                {
                    drawing.Down(new Point { X = e.X, Y = e.Y }, g);
                }else
                {
                    startPoint = new Point { X = e.X, Y = e.Y };
                }
                g.Dispose();
                mainPictureBox.Image = bitmap;
            }else
            if (e.Button == MouseButtons.Right && currentTool == drawing)
                drawing.Restart();
            
        }

        private void mainPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                Graphics g = Graphics.FromImage(bitmap);
                DrawContext(g);
                lastMatrix = currentTool.Draw(startPoint, new Point {X = e.X, Y = e.Y }, g);
                g.Dispose();
                mainPictureBox.Image = bitmap;
            }
            if (currentTool == drawing)
            {
                Graphics g = Graphics.FromImage(bitmap);
                DrawContext(g);
                drawing.Move(new Point { X = e.X, Y = e.Y }, g);
                g.Dispose();
                mainPictureBox.Image = bitmap;
            }
        }

        private void mainPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            context.Apply(lastMatrix);
            lastMatrix = Matrix.Ident();
            Graphics g = Graphics.FromImage(bitmap);
            DrawContext(g);
            g.Dispose();
            mainPictureBox.Image = bitmap;
        }
    }
}
