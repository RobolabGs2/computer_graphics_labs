using Lab06.Base3D;
using Lab06.Graph3D;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab06
{
    public partial class MainForm : Form
    {
        Context context = new Context();
        PictureBox mainPicture;
        IDrawing drawing;
        Bitmap bitmap;

        public MainForm()
        {
            InitializeComponent();

            BackColor = Constants.backColore;
            splitList.BackColor = Constants.borderColore;

            mainPicture = new PictureBox
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                Width = splitTools.Panel1.Width,
                Height = splitTools.Panel1.Height,
            };
            //mainPicture.BackColor = Constants.backColore;

            splitTools.Panel1.Controls.Add(mainPicture);

            bitmap = new Bitmap(mainPicture.Width, mainPicture.Height);
            drawing = new Skeleton(context);

            mainPicture.SizeChanged += (o, s) => {
                if (mainPicture.Width <= 0 || mainPicture.Height <= 0)
                    return;
                Bitmap newBitmap = new Bitmap(mainPicture.Width, mainPicture.Height);
                (newBitmap, bitmap) = (bitmap, newBitmap);
                drawing.Draw(bitmap);
                mainPicture.Image = bitmap;
                newBitmap.Dispose();
            };

            System.Drawing.Point lastMose = new System.Drawing.Point(0, 0);
            mainPicture.MouseMove += (o, s) => {
                if (s.Button != MouseButtons.Left)
                    return;

                var xDist = s.X - lastMose.X;
                var yDist = s.Y - lastMose.Y;

                if (xDist * xDist + yDist * yDist < 1000)
                {
                    context.camera.leftAngle += xDist * Math.PI / 200;
                    context.camera.downAngle += yDist * Math.PI / 200;
                    if (context.camera.downAngle <= -Math.PI / 2)
                        context.camera.downAngle = -Math.PI / 2;
                    if (context.camera.downAngle >= Math.PI / 2)
                        context.camera.downAngle = Math.PI / 2;
                    drawing.Draw(bitmap);
                    mainPicture.Image = bitmap;
                }
                lastMose.X = s.X;
                lastMose.Y = s.Y;
            };


            context.camera.Move(Matrix.Move(new Base3D.Point { X = -100 }));
            context.camera.downAngle = Math.PI / 7;
            context.camera.leftAngle = Math.PI / 7;
            context.world.entities.Add(GenerateCube());

            drawing.Draw(bitmap);
            mainPicture.Image = bitmap;
        }

        Polytope GenerateCube()
        {

            Polytope cube = new Polytope();
            cube.Add(new Base3D.Point { X = 100, Y = 100, Z = 100 });
            cube.Add(new Base3D.Point { X = -100, Y = 100, Z = 100 });
            cube.Add(new Base3D.Point { X = -100, Y = -100, Z = 100 });
            cube.Add(new Base3D.Point { X = 100, Y = -100, Z = 100 });
            cube.Add(new Base3D.Point { X = 100, Y = 100, Z = -100 });
            cube.Add(new Base3D.Point { X = -100, Y = 100, Z = -100 });
            cube.Add(new Base3D.Point { X = -100, Y = -100, Z = -100 });
            cube.Add(new Base3D.Point { X = 100, Y = -100, Z = -100 });

            cube.Add(new Polygon(new Base3D.Point[] { cube.points[0], cube.points[1], cube.points[2], cube.points[3] }));
            cube.Add(new Polygon(new Base3D.Point[] { cube.points[4], cube.points[5], cube.points[6], cube.points[7] }));
            cube.Add(new Polygon(new Base3D.Point[] { cube.points[0], cube.points[1], cube.points[5], cube.points[4] }));
            cube.Add(new Polygon(new Base3D.Point[] { cube.points[2], cube.points[3], cube.points[7], cube.points[6] }));

            return cube;
        }
    }
}