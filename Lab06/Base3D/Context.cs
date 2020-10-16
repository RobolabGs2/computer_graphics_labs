using Lab06.Graph3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab06.Base3D
{
    public class Context
    {
        public Camera camera = new Camera();
        public World world = new World();
        public Matrix projection = Matrix.Projection(0.001, 0, 0);
        
        public IDrawing drawing;
        public PictureBox pictureBox;
        public Bitmap bitmap;

        public double scale = 100;
        public bool cutNegative = true;

        public Context(PictureBox pictureBox)
        {
            this.pictureBox = pictureBox;
            pictureBox.SizeChanged += (o, s) => Resize();
            bitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
        }
        
        public Matrix DrawingMatrix()
        {
            return
                camera.Projection() *
                Matrix.Scale(new Point { X = scale, Y = scale, Z = scale }) *
                projection *
                Matrix.Move(new Base3D.Point
                {
                    Y = -bitmap.Width / 2,
                    Z = -bitmap.Height / 2
                }) *
            Matrix.Scale(new Point { X = 1, Y = -1, Z = -1 });
        }

        public Matrix InvertDrawingMatrix()
        {
            return
                Matrix.Scale(new Point { X = 1, Y = -1, Z = -1 }) *
                Matrix.Move(new Base3D.Point
                {
                    Y = bitmap.Width / 2,
                    Z = bitmap.Height / 2
                }) *
                projection.Invert() *
                Matrix.Scale(new Point { X = 1 / scale, Y = 1 / scale, Z = 1 / scale }) *
                camera.InvertProjection();
        }


        /// <summary>
        /// Преобразует точку на экране в соответствующую точку на плоскости XY
        /// </summary>
        public (Base3D.Point p, bool front) ScreenToXY(int x, int y)
        {
            Point p1 = (new Point { Y = x, Z = y } * InvertDrawingMatrix()).FlattenT();
            Point p2 = (new Point { Y = x, Z = y, X = 1 } * InvertDrawingMatrix()).FlattenT();

            double denum = p2.Z - p1.Z;
            double numX = -(p2.X  - p1.X) * p2.Z;
            double numY = -(p2.Y - p1.Y) * p2.Z;

            return (new Point { X = numX / denum + p2.X, Y = numY / denum + p2.Y}, 
                Math.Sign(denum) * Math.Sign(camera.location.Z) < 0);
        }

        private void Resize()
        {
            if (pictureBox.Width <= 0 || pictureBox.Height <= 0)
                return;
            Bitmap bmp = new Bitmap(pictureBox.Width, pictureBox.Height);
            (bmp, bitmap) = (bitmap, bmp);
            Redraw();
            pictureBox.Image = bitmap;
            bmp?.Dispose();
        }

        public void Redraw()
        {
            drawing.Draw(bitmap);
            pictureBox.Image = bitmap;
        }

        public bool BeforeScreen(double depth)
        {
            return !cutNegative || depth > -1000;
        }
    }
}
