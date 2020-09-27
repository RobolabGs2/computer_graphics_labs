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

                button.Click += (a, b) =>
                {
                    if (tool.Active())
                    {
                        currentTool = tool;
                    }
                    DrawContext(g => { });
                };
                mainPanel.Controls.Add(button);
            }
        }

        void DrawContext(Action<Graphics> action)
        {
            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.White);
            context.Draw(g, lastMatrix);
            action(g);
            g.Dispose();
            mainPictureBox.Image = bitmap;
        }

        private void mainPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                if (currentTool == drawing)
                    DrawContext(g => drawing.Down(new Point { X = e.X, Y = e.Y }, g));
                else
                    startPoint = new Point { X = e.X, Y = e.Y };
            }else
            if (e.Button == MouseButtons.Right && currentTool == drawing)
                drawing.Restart();
        }

        private void mainPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                DrawContext(g => lastMatrix = currentTool.Draw(startPoint, new Point { X = e.X, Y = e.Y }, g));
            if (currentTool == drawing)
                DrawContext(g => drawing.Move(new Point { X = e.X, Y = e.Y }, g));
        }

        private void mainPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                context.Apply(lastMatrix);
                lastMatrix = Matrix.Ident();
                DrawContext(g => { });
            } 
        }
    }
}
