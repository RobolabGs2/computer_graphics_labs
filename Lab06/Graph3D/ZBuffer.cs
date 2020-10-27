using Lab06.Base3D;
using Lab06.Materials3D;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Lab06.Graph3D
{
    public class ZBuffer: IDrawing
    {
        Context context;
        Matrix cameraMatric;
        Matrix rotationCamera;
        FastBitmap fastBitmap;
        double[,] buffer;
        double interval;
        int width;
        int height;

        public double phongAmbient = 0.1;
        public double phongDiffuse = 1;
        public double phongSpecular = 1;
        public double phongPower = 1000;
        public bool smoothing = true;

        public ZBuffer(Context context)
        {
            this.context = context;
        }

        public void Draw(Bitmap bitmap)
        {
            var graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Constants.backColore);
            graphics.Dispose();
            width = bitmap.Width;
            height = bitmap.Height;

            buffer = new double[bitmap.Width, bitmap.Height];
            fastBitmap = new FastBitmap(bitmap, System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            cameraMatric = context.DrawingMatrix();
            rotationCamera = context.camera.Rotation();
            interval = context.camera.interval * context.scale;
            foreach (Entity e in context.world.entities)
                DrawEntity(e);
            buffer = new double[bitmap.Width, bitmap.Height];
            foreach (Entity e in context.world.control)
                DrawEntity(e);
            fastBitmap.Dispose();
        }

        void DrawEntity(Entity entity, BaseMaterial material = null)
        {
            if (entity is Base3D.Polytope pol)
                DrawPolytope(pol, material == null ? pol.Matreial : material);
            if (entity is Base3D.Group group)
                DrawGroup(group);
        }

        void DrawGroup(Base3D.Group p)
        {
            foreach (Entity e in p.entities)
                DrawEntity(e, e.Matreial);
        }

        void DrawPolytope(Base3D.Polytope p, BaseMaterial material)
        {
            var points = p.points.Select(point => point * cameraMatric).ToList();
            var rotating = p.points.Select(point => point * rotationCamera).ToList();
            Base3D.Point zero = new Base3D.Point() * rotationCamera;
            var normals = p.normals.Select(point => point * rotationCamera - zero).ToList();
            foreach (Polygon polygon in p.polygons)
                DrawPolygon(polygon, points, rotating, normals, material.Color);
        }

        Random r = new Random();
        void DrawPolygon(Base3D.Polygon pol, List<Base3D.Point> points, List<Base3D.Point> rotated, List<Base3D.Point> normals, Color color)
        {
            if (pol.indexes.Count < 3)
                return;

            var polyPoints = pol.Points(points);
            if (polyPoints.Any(p => !context.BeforeScreen(p.X)))
                return;
            var v1 = polyPoints[1] - polyPoints[0];
            var v2 = polyPoints[2] - polyPoints[0];
            var prod = new Base3D.Point
            {
                X = v1.Y * v2.Z - v2.Y * v1.Z,
                Y = v1.Z * v2.X - v2.Z * v1.X,
                Z = v1.X * v2.Y - v2.X * v1.Y
            };

            if (prod.X > 0)
                return;
            if (prod.Length() == 0)
                return;

            Base3D.Point zero = new Base3D.Point() * cameraMatric;
            for (int i = 0; i < polyPoints.Count - 2; ++i)
            {
                Base3D.Point p1 = polyPoints[0];
                Base3D.Point p2 = polyPoints[i + 1];
                Base3D.Point p3 = polyPoints[i + 2];
                if (pol.normals == null || !smoothing)
                    DrawTriangle(p1.Y, p1.Z, p2.Y, p2.Z, p3.Y, p3.Z,
                        new Stuff { Z = interval - p1.X, Normal = prod },
                        new Stuff { Z = interval - p2.X, Normal = prod },
                        new Stuff { Z = interval - p3.X, Normal = prod }, color);
                else
                    DrawTriangle(p1.Y, p1.Z, p2.Y, p2.Z, p3.Y, p3.Z,
                        new Stuff { Z = interval - p1.X, Normal = normals[pol.normals[0]]  },
                        new Stuff { Z = interval - p2.X, Normal = normals[pol.normals[i + 1]] },
                        new Stuff { Z = interval - p3.X, Normal = normals[pol.normals[i + 2]] }, color);
            }
        }

        int colorScheme(double pow, double k, int C)
        {
            return Math.Min((int)(pow + C * k), 255);
        }

        void TrySet(double x, double y, Stuff s, Color color)
        {
            int ix = (int)Math.Round(x);
            int iy = (int)Math.Round(y);
            if (ix < 0 || iy < 0 || ix >= width || iy >= height)
                return;
            if (s.Z <= buffer[ix, iy])
                return;
            buffer[ix, iy] = s.Z;
            double cos = Math.Max(-s.Normal.X / s.Normal.Length(), 0);
            double pow = Math.Pow(cos, phongPower) * phongSpecular * 255;
            double k = (phongAmbient + phongDiffuse * cos);

            fastBitmap.SetPixel(ix, iy, Color.FromArgb(0,
                        colorScheme(pow, k, color.R),
                        colorScheme(pow, k, color.G),
                        colorScheme(pow, k, color.B)));
        }

        void DrawTriangle(double x1, double y1, double x2, double y2, double x3, double y3,
            Stuff s1, Stuff s2, Stuff s3, Color color)
        {
            if (y2 < y1)
                (x1, y1, x2, y2, s1, s2) = (x2, y2, x1, y1, s2, s1);
            if (y3 < y1)
                (x1, y1, x3, y3, s1, s3) = (x3, y3, x1, y1, s3, s1);
            if (y3 < y2)
                (x2, y2, x3, y3, s2, s3) = (x3, y3, x2, y2, s3, s2);

            double startX = (y2 - y1) * (x3 - x1) / (y3 - y1) + x1;
            double endX = x2;
            Stuff startStuff = (y2 - y1) * (s3 - s1) / (y3 - y1) + s1;
            Stuff endStuff = s2;
            
            if (startX > endX)
            {
                (startX, endX) = (endX, startX);
                (startStuff, endStuff) = (endStuff, startStuff);
            }

            for (double i = y1; i <= y2; i += 0.5)
            {
                double startJ = ((y2 - i) * x1 + (i - y1) * startX) / (y2 - y1);
                double endJ = ((y2 - i) * x1 + (i - y1) * endX) / (y2 - y1);
                Stuff stf1 = ((y2 - i) * s1 + (i - y1) * startStuff) / (y2 - y1);
                Stuff stf2 = ((y2 - i) * s1 + (i - y1) * endStuff) / (y2 - y1);

                for (double j = startJ; j < endJ; j += 0.5)
                {
                    Stuff x = ((endJ - j) * stf1 + (j - startJ) * stf2) / (endJ - startJ);
                    TrySet(j, i, x, color);
                }
            }

            for (double i = y2; i <= y3; i += 0.5)
            {
                double startJ = ((y3 - i) * startX + (i - y2) * x3) / (y3 - y2);
                double endJ = ((y3 - i) * endX + (i - y2) * x3) / (y3 - y2);
                Stuff stf1 = ((y3 - i) * startStuff + (i - y2) * s3) / (y3 - y2);
                Stuff stf2 = ((y3 - i) * endStuff + (i - y2) * s3) / (y3 - y2);

                for (double j = startJ; j < endJ; j += 0.5)
                {
                    Stuff x = ((endJ - j) * stf1 + (j - startJ) * stf2) / (endJ - startJ);
                    TrySet(j, i, x, color);
                }
            }
        }

        struct Stuff
        {
            public Base3D.Point Normal { get; set; }
            public double Z { get; set; }

            public static Stuff operator +(Stuff s1, Stuff s2)
            {
                return new Stuff
                {
                    Normal = s1.Normal + s2.Normal,
                    Z = s1.Z + s2.Z
                };
            }
            
            public static Stuff operator -(Stuff s1, Stuff s2)
            {
                return new Stuff
                {
                    Normal = s1.Normal - s2.Normal,
                    Z = s1.Z - s2.Z
                };
            }

            public static Stuff operator *(Stuff s, double c)
            {
                return new Stuff
                {
                    Normal = s.Normal * c,
                    Z = s.Z * c
                };
            }
            public static Stuff operator *(double c, Stuff s)
            {
                return s * c;
            }

            public static Stuff operator /(Stuff s, double c)
            {
                return new Stuff
                {
                    Normal = s.Normal / c,
                    Z = s.Z / c
                };
            }

        }
    }
}
