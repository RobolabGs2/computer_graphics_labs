using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab04
{
    public class Matrix
    {
        const int volume = 3;
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
            Matrix result= new Matrix();
            for (int i = 0; i < volume; ++i)
                result[i, i] = 1;
            return result;
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
        public static Point operator *(Point P, Matrix M)
        {
            Point result = new Point{
                X = P.X * M[0, 0] + P.Y * M[1, 0] + M[2, 0],
                Y = P.X * M[0, 1] + P.Y * M[1, 1] + M[2, 1]
            };
            return result;
        }
    }
}
