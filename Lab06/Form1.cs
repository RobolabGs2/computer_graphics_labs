using Lab06.Base3D;
using Lab06.Graph3D;
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
        Context context ;

        public MainForm(List<IToolPage> tools)
        {
            Application.EnableVisualStyles();
            InitializeComponent();


            BackColor = Constants.backColore;
            splitList.BackColor = Constants.borderColore;
            
            PictureBox pictureBox = new PictureBox { Dock = DockStyle.Fill };
            splitTools.Panel1.Controls.Add(pictureBox);
            context = new Context(pictureBox);
            context.drawing = new Skeleton(context);


            context.camera.Move(Matrix.Move(new Base3D.Point { X = -1 }));
            for (int i = -3; i < 3; ++i)
                for (int j = -3; j < 3; ++j)
                    context.world.entities.Add(GenerateCube(new Base3D.Point { X = 6 * i, Y = 6 * j, Z = (i + j == 2) ? 1 : 0, T = 0 }));



            List<ToolTab> tabList = tools.Select((tool, i) => new ToolTab { ImageIndex = i }).ToList();
            ToolTabs tabs = new ToolTabs(tabList, tools.Select(t => t.Image));
            foreach(var pair in tools.Zip(tabList, (tool, tab) => (tool, tab)))
                 pair.tool.Init(pair.tab, context);

            splitTools.Panel2.Controls.Add(tabs);

            AddMoveEvents();
            context.Redraw();
        }


        void AddMoveEvents()
        {
            System.Drawing.Point lastRotate = new System.Drawing.Point(0, 0);
            System.Drawing.Point lastMove = new System.Drawing.Point(0, 0);
            bool lastRightCluck = false;
            bool lastMiddleCluck = false;

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
                double d = s.Delta / 40.0;
                double a = context.camera.leftAngle;
                double b = context.camera.downAngle;
                context.camera.Move(Matrix.Move(new Base3D.Point { 
                    X = Math.Cos(b) * Math.Cos(a) * d,
                    Y = Math.Cos(b) * Math.Sin(a) * d,
                    Z = -Math.Sin(b) * d}));
                context.Redraw();
            };
        }

        Polytope GenerateCube(Base3D.Point location)
        {

            Polytope cube = new Polytope();
            cube.Add(new Base3D.Point { X = 1, Y = 1, Z = 1 } + location);
            cube.Add(new Base3D.Point { X = -1, Y = 1, Z = 1 } + location);
            cube.Add(new Base3D.Point { X = -1, Y = -1, Z = 1 } + location);
            cube.Add(new Base3D.Point { X = 1, Y = -1, Z = 1 } + location);
            cube.Add(new Base3D.Point { X = 1, Y = 1, Z = -1 } + location);
            cube.Add(new Base3D.Point { X = -1, Y = 1, Z = -1 } + location);
            cube.Add(new Base3D.Point { X = -1, Y = -1, Z = -1 } + location);
            cube.Add(new Base3D.Point { X = 1, Y = -1, Z = -1 } + location);

            cube.Add(new Polygon(new Base3D.Point[] { cube.points[0], cube.points[1], cube.points[2], cube.points[3] }));
            cube.Add(new Polygon(new Base3D.Point[] { cube.points[4], cube.points[5], cube.points[6], cube.points[7] }));
            cube.Add(new Polygon(new Base3D.Point[] { cube.points[0], cube.points[1], cube.points[5], cube.points[4] }));
            cube.Add(new Polygon(new Base3D.Point[] { cube.points[2], cube.points[3], cube.points[7], cube.points[6] }));

            return cube;
        }
    }
}