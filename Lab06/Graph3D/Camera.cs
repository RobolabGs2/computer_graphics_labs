using Lab06.Base3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab06.Graph3D
{
    public class Camera
    {
        public double downAngle = 0;
        public double leftAngle = 0;
        public Point location = new Point();
        public double interval = 10;


        public Matrix Projection()
        {
            return
                Matrix.Move(-location) *
                Matrix.ZRotation(-leftAngle) *
                Matrix.YRotation(-downAngle) *
                Matrix.Projection(1 / interval, 0, 0);
        }

        public Matrix Rotation()
        {
            return
                Matrix.ZRotation(-leftAngle) *
                Matrix.YRotation(-downAngle);
        }

        public Matrix InvertProjection()
        {
            return
                Matrix.Projection(-1 / interval, 0, 0) *
                Matrix.YRotation(downAngle) *
                Matrix.ZRotation(leftAngle) *
                Matrix.Move(location);
        }

        public Point Direction()
        {
            return new Point { X = 1 } *
                Matrix.YRotation(downAngle) *
                Matrix.ZRotation(leftAngle);
        }

        public void Move(Matrix matrix)
        {
            location = location * matrix;
        }

        public bool BeforeScreen(double t)
        {
            return Math.Abs(t) > 1e-10 && t != double.NaN && t < interval;
        }
    }
}
