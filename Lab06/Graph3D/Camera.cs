using Lab06.Base3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab06.Graph3D
{
    class Camera
    {
        public double downAngle = 0;
        public double leftAngle = 0;
        Point location = new Point();

        public Matrix Location()
        {
            return Matrix.Move(-location) *
                 Matrix.ZRotation(-leftAngle) *
                Matrix.YRotation(-downAngle);
        }

        public void Move(Matrix matrix)
        {
            location = location * matrix;
        }
    }
}
