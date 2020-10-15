using Lab06.Graph3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        public Context(PictureBox pictureBox)
        {
            this.pictureBox = pictureBox;
            pictureBox.SizeChanged += (o, s) => Resize();
            bitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
        }

        public Matrix drawingMatrix()
        {
            return camera.Location() * projection;
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
    }
}
