using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab06.Base3D
{
    public class Matrix
    {
        const int volume = 4;
        private double[,] data = new double[volume, volume];

        public double this[int i, int j]
        {
            get => data[i, j];
            set => data[i, j] = value;
        }

        private Matrix()
        { }

        public static Matrix Zero()
        {
            return new Matrix();
        }

        public static Matrix Ident()
        {
            Matrix result = new Matrix();
            for (int i = 0; i < volume; ++i)
                result[i, i] = 1;
            return result;
        }

        public static Matrix Move(Point delta)
        {
            Matrix m = Matrix.Ident();
            m[3, 0] = delta.X;
            m[3, 1] = delta.Y;
            m[3, 2] = delta.Z;
            m[3, 3] = delta.T;
            return m;
        }
        public static Matrix Scale(Point scale)
        {
            Matrix m = Matrix.Ident();
            m[0, 0] = scale.X;
            m[1, 1] = scale.Y;
            m[2, 2] = scale.Z;
            m[3, 3] = scale.T;
            return m;
        }
        public static Matrix XRotation(double angle)
        {
            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);
            Matrix m = Matrix.Ident();
            m[1, 1] = cos;
            m[1, 2] = sin;
            m[2, 1] = -sin;
            m[2, 2] = cos;
            return m;
        }
        
        public static Matrix YRotation(double angle)
        {
            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);
            Matrix m = Matrix.Ident();
            m[0, 0] = cos;
            m[0, 2] = -sin;
            m[2, 0] = sin;
            m[2, 2] = cos;
            return m;
        }

        public static Matrix ZRotation(double angle)
        {
            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);
            Matrix m = Matrix.Ident();
            m[0, 0] = cos;
            m[0, 1] = sin;
            m[1, 0] = -sin;
            m[1, 1] = cos;
            return m;
        }

        public static Matrix Projection(double rX, double rY, double rZ)
        {
            Matrix m = Matrix.Ident();
            m[0, 3] = rX;
            m[1, 3] = rY;
            m[2, 3] = rZ;
            return m;
        }

        public static Matrix InPoint(Point p, Matrix modification)
        {
            return Move(-p) * modification * Move(p);
        }

        public static Matrix operator *(Matrix A, Matrix B)
        {
            Matrix result = new Matrix();
            for (int i = 0; i < volume; ++i)
                for (int j = 0; j < volume; ++j)
                    for (int k = 0; k < volume; ++k)
                        result[i, j] += A[i, k] * B[k, j];
            return result;
        }

        public static Matrix operator +(Matrix A, Matrix B)
        {
            Matrix result = new Matrix();
            for (int i = 0; i < volume; ++i)
                for (int j = 0; j < volume; ++j)
                    result[i, j] = A[i, j] + B[i, j];
            return result;
        }

        public static Point operator *(Point p, Matrix m)
        {
            Point result = p.Copy();
            result.Apply(m);
            return result;
        }
    }
}
