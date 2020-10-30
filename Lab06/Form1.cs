using Lab06.Base3D;
using Lab06.Graph3D;
using Lab06.Materials3D;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab06
{
    public partial class MainForm : Form
    {
        Context context;

        public MainForm(List<IToolPage> tools)
        {
            Application.EnableVisualStyles();
            InitializeComponent();


            BackColor = Constants.backColore;
            splitList.BackColor = Constants.borderColore;
            splitList.Panel1Collapsed = true;
            PictureBox pictureBox = new PictureBox { Dock = DockStyle.Fill };
            splitTools.Panel1.Controls.Add(pictureBox);
            context = new Context(pictureBox);
            context.drawing = new Skeleton(context);


            context.camera.Move(Matrix.Move(new Base3D.Point { X = -1 }));

            List<ToolTab> tabList = tools.Select((tool, i) => new ToolTab { ImageIndex = i }).ToList();
            ToolTabs tabs = new ToolTabs(tabList, tools.Select(t => t.Image));
            foreach(var pair in tools.Zip(tabList, (tool, tab) => (tool, tab)))
                 pair.tool.Init(pair.tab, context);

            splitTools.Panel2.Controls.Add(tabs);

            AddMoveEvents();
            InitContext();
            context.Redraw();
        }


        void AddMoveEvents()
        {
            System.Drawing.Point lastRotate = new System.Drawing.Point(0, 0);
            System.Drawing.Point lastMove = new System.Drawing.Point(0, 0);
            bool lastRightCluck = false;
            bool lastMiddleCluck = false;

            KeyPreview = true;

            context.pictureBox.MouseUp += (o, s) =>
            {
                if (s.Button == MouseButtons.Right)
                    lastRightCluck = false;
                if (s.Button == MouseButtons.Middle)
                    lastMiddleCluck = false;
            };

            context.pictureBox.MouseMove += (o, s) =>
            {
                if (s.Button == MouseButtons.Right)
                {
                    double xDist = s.X - lastRotate.X;
                    double yDist = s.Y - lastRotate.Y;

                    if (lastRightCluck)
                    {
                        context.camera.leftAngle -= xDist * Math.PI / 500;
                        context.camera.downAngle += yDist * Math.PI / 500;
                        if (context.camera.downAngle <= -Math.PI / 2)
                            context.camera.downAngle = -Math.PI / 2;
                        if (context.camera.downAngle >= Math.PI / 2)
                            context.camera.downAngle = Math.PI / 2;
                        context.Redraw();
                    }
                    lastRightCluck = true;
                    lastRotate.X = s.X;
                    lastRotate.Y = s.Y;
                }
                else
                if (s.Button == MouseButtons.Middle)
                {
                    double xDist = s.X - lastMove.X;
                    double yDist = s.Y - lastMove.Y;

                    if (lastMiddleCluck)
                    {
                        xDist /= 30;  yDist /= 30;
                        double a = context.camera.leftAngle;
                        double b = context.camera.downAngle;
                        context.camera.Move(Matrix.Move(new Base3D.Point
                        {
                            X = yDist * Math.Sin(b) * Math.Cos(a) - xDist * Math.Sin(a),
                            Y = yDist * Math.Sin(b) * Math.Sin(a) + xDist * Math.Cos(a),
                            Z = yDist * Math.Cos(b)
                        }));
                        context.Redraw();
                    }
                    lastMiddleCluck = true;
                    lastMove.X = s.X;
                    lastMove.Y = s.Y;
                }
            };

            context.pictureBox.MouseWheel += (o, s) =>
            {
                if (Control.ModifierKeys == Keys.Control)
                {
                    context.camera.interval += s.Delta / 100.0;
                    if (context.camera.interval <= 0)
                        context.camera.interval = 1;
                }
                else
                if (Control.ModifierKeys == Keys.Shift)
                {
                    context.scale += s.Delta / 10.0;
                    if (context.scale <= 0)
                        context.scale = 1;
                }
                else
                {
                    double d = s.Delta / 40.0;
                    double a = context.camera.leftAngle;
                    double b = context.camera.downAngle;
                    context.camera.Move(Matrix.Move(new Base3D.Point
                    {
                        X = Math.Cos(b) * Math.Cos(a) * d,
                        Y = Math.Cos(b) * Math.Sin(a) * d,
                        Z = -Math.Sin(b) * d
                    }));
                }
                context.Redraw();
            };
        }

        void InitContext()
        {

            for (int i = -5; i <= 5; ++i)
            {
                Spline spline1 = new Spline();
                spline1.Add(new Base3D.Point { X = 5, Y = i, Z = 0 });
                spline1.Add(new Base3D.Point { X = -5, Y = i, Z = 0 });
                context.world.control.Add(spline1);
                Spline spline2 = new Spline();
                spline2.Add(new Base3D.Point { X = i, Y = 5, Z = 0 });
                spline2.Add(new Base3D.Point { X = i, Y = -5, Z = 0 });
                context.world.control.Add(spline2);
            }
            Spline xSpline = new Spline();
            xSpline.Matreial = new SolidMaterial(Color.DarkRed);
            xSpline.Add(new Base3D.Point { X = 0 });
            xSpline.Add(new Base3D.Point { X = 1 });
            context.world.control.Add(xSpline);

            Spline ySpline = new Spline();
            ySpline.Matreial = new SolidMaterial(Color.DarkBlue);
            ySpline.Add(new Base3D.Point { Y = 0 });
            ySpline.Add(new Base3D.Point { Y = 1 });
            context.world.control.Add(ySpline);

            Spline zSpline = new Spline();
            zSpline.Matreial = new SolidMaterial(Color.DarkGreen);
            zSpline.Add(new Base3D.Point { Z = 0 });
            zSpline.Add(new Base3D.Point { Z = 1 });
            context.world.control.Add(zSpline);
        }
    }
}