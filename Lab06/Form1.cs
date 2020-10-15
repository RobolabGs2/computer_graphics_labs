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


            context.camera.Move(Matrix.Move(new Base3D.Point { X = -100 }));
            for (int i = -3; i < 3; ++i)
                for (int j = -3; j < 3; ++j)
                    context.world.entities.Add(GenerateCube(new Base3D.Point { X = 600 * i, Y = 600 * j, Z = 0, T = 0 }));

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

            context.pictureBox.MouseMove += (o, s) =>
            {
                if (s.Button == MouseButtons.Right)
                {
                    var xDist = s.X - lastRotate.X;
                    var yDist = s.Y - lastRotate.Y;

                    if (xDist * xDist + yDist * yDist < 1000)
                    {
                        context.camera.leftAngle -= xDist * Math.PI / 300;
                        context.camera.downAngle += yDist * Math.PI / 300;
                        if (context.camera.downAngle <= -Math.PI / 2)
                            context.camera.downAngle = -Math.PI / 2;
                        if (context.camera.downAngle >= Math.PI / 2)
                            context.camera.downAngle = Math.PI / 2;
                        context.Redraw();
                    }
                    lastRotate.X = s.X;
                    lastRotate.Y = s.Y;
                }
                else
                if (s.Button == MouseButtons.Middle)
                {
                    var xDist = s.X - lastMove.X;
                    var yDist = s.Y - lastMove.Y;

                    if (xDist * xDist + yDist * yDist < 1000)
                    {
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
                    lastMove.X = s.X;
                    lastMove.Y = s.Y;
                }
            };

            context.pictureBox.MouseWheel += (o, s) =>
            {
                double d = s.Delta / 2;
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
            cube.Add(new Base3D.Point { X = 100, Y = 100, Z = 100 } + location);
            cube.Add(new Base3D.Point { X = -100, Y = 100, Z = 100 } + location);
            cube.Add(new Base3D.Point { X = -100, Y = -100, Z = 100 } + location);
            cube.Add(new Base3D.Point { X = 100, Y = -100, Z = 100 } + location);
            cube.Add(new Base3D.Point { X = 100, Y = 100, Z = -100 } + location);
            cube.Add(new Base3D.Point { X = -100, Y = 100, Z = -100 } + location);
            cube.Add(new Base3D.Point { X = -100, Y = -100, Z = -100 } + location);
            cube.Add(new Base3D.Point { X = 100, Y = -100, Z = -100 } + location);

            cube.Add(new Polygon(new Base3D.Point[] { cube.points[0], cube.points[1], cube.points[2], cube.points[3] }));
            cube.Add(new Polygon(new Base3D.Point[] { cube.points[4], cube.points[5], cube.points[6], cube.points[7] }));
            cube.Add(new Polygon(new Base3D.Point[] { cube.points[0], cube.points[1], cube.points[5], cube.points[4] }));
            cube.Add(new Polygon(new Base3D.Point[] { cube.points[2], cube.points[3], cube.points[7], cube.points[6] }));

            return cube;
        }
    }
}