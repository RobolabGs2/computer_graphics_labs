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
            return m;
        }
        public static Matrix Scale(Point scale)
        {
            Matrix m = Matrix.Ident();
            m[0, 0] = scale.X;
            m[1, 1] = scale.Y;
            m[2, 2] = scale.Z;
            return m;
        }
        public static Matrix XRotation(double angle)
        {
            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);
            return XRotation(sin, cos);
        }

        public static Matrix YRotation(double angle)
        {
            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);
            return YRotation(sin, cos);
        }

        public static Matrix ZRotation(double angle)
        {
            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);
            return ZRotation(sin, cos);
        }
        public static Matrix Rotation(Point vector, double angle)
        {
            double length1 = Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            double sin1 = vector.Y / length1;
            double cos1 = vector.X / length1;
            Matrix result = Matrix.Ident();
            if (length1 != 0)
                result = Matrix.ZRotation(-sin1, cos1);
            vector = vector * result;
            double length2 = Math.Sqrt(vector.X * vector.X + vector.Z * vector.Z);
            double sin2 = vector.X / length2;
            double cos2 = vector.Z / length2;
            result *= Matrix.YRotation(-sin2, cos2);
            result *= Matrix.ZRotation(angle);
            result *= Matrix.YRotation(sin2, cos2);
            if (length1 != 0)
                result *= Matrix.ZRotation(sin1, cos1);
            return result;
        }
        public static Matrix Scale(Point vector, double scale)
        {
            double length1 = Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            double sin1 = vector.Y / length1;
            double cos1 = vector.X / length1;
            Matrix result = Matrix.Ident();
            if (length1 != 0)
                result = Matrix.ZRotation(-sin1, cos1);
            vector = vector * result;
            double length2 = Math.Sqrt(vector.X * vector.X + vector.Z * vector.Z);
            double sin2 = vector.X / length2;
            double cos2 = vector.Z / length2;
            result *= Matrix.YRotation(-sin2, cos2);
            result *= Matrix.Scale(new Point {X = 1, Y = 1, Z = scale});
            result *= Matrix.YRotation(sin2, cos2);
            if (length1 != 0)
                result *= Matrix.ZRotation(sin1, cos1);
            return result;
        }

        public static Matrix XRotation(double sin, double cos)
        {
            Matrix m = Matrix.Ident();
            m[1, 1] = cos;
            m[1, 2] = sin;
            m[2, 1] = -sin;
            m[2, 2] = cos;
            return m;
        }

        public static Matrix YRotation(double sin, double cos)
        {
            Matrix m = Matrix.Ident();
            m[0, 0] = cos;
            m[0, 2] = -sin;
            m[2, 0] = sin;
            m[2, 2] = cos;
            return m;
        }

        public static Matrix ZRotation(double sin, double cos)
        {
            Matrix m = Matrix.Ident();
            m[0, 0] = cos;
            m[0, 1] = sin;
            m[1, 0] = -sin;
            m[1, 1] = cos;
            return m;
        }

        public Matrix Copy()
        {
            Matrix result = new Matrix();
            result.data = (double[,])data.Clone();
            return result;
        }

        public static Matrix Projection(double rX, double rY, double rZ)
        {
            Matrix m = Matrix.Ident();
            m[0, 3] = rX;
            m[1, 3] = rY;
            m[2, 3] = rZ;
            return m;
        }

        /// <summary>
        /// Умножение матриц. Не идеально, может работать не всегда
        /// (за разъяснением случаев когда не работает можно обращаться комне)
        /// </summary>
        public Matrix Invert()
        {
            Matrix res = Matrix.Ident();
            Matrix copy = Copy();
            for (int i = 0; i < volume; ++i)
            {
                double denum = copy[i, i];
                for (int j = 0; j < volume; ++j)
                    if (i != j)
                    {
                        double current = copy[j, i];
                        for (int k = 0; k < volume; ++k)
                        {

                            copy[j, k] -= copy[i, k] * current / denum;
                            res[j, k] -= res[i, k] * current / denum;

                        }
                    }
                for (int k = 0; k < volume; ++k)
                {
                    copy[i, k] = copy[i, k] / denum;
                    res[i, k] = res[i, k] / denum;
                }
            }
            return res;
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
            double T = p.X * m[0, 3] + p.Y * m[1, 3] + p.Z * m[2, 3] + m[3, 3];
            return new Point
            {
                X = (p.X * m[0, 0] + p.Y * m[1, 0] + p.Z * m[2, 0] + m[3, 0]) / T,
                Y = (p.X * m[0, 1] + p.Y * m[1, 1] + p.Z * m[2, 1] + m[3, 1]) / T,
                Z = (p.X * m[0, 2] + p.Y * m[1, 2] + p.Z * m[2, 2] + m[3, 2]) / T
            };
        }
    }
}
