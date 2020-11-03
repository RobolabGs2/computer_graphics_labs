using Lab06.Base3D;
using Lab06.Materials3D;
using Lab06.Tools3D.Graph;
using System;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
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

        Base3D.Point light;

        public ZBuffer(Context context)
        {
            this.context = context;
        }

        public void Draw(Bitmap bitmap)
        {
            var graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Constants.backColore);
            graphics.Dispose();
            if (width != bitmap.Width)
            {
                width = bitmap.Width;
                height = bitmap.Height;

                buffer = new double[bitmap.Width, bitmap.Height];
            }else
            {
                for (int i = 0; i < width; ++i)
                    for (int j = 0; j < height; ++j)
                        buffer[i, j] = 0;
            }

            fastBitmap = new FastBitmap(bitmap, System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            cameraMatric = context.DrawingMatrix();
            rotationCamera = context.camera.Rotation() * Matrix.Scale(new Base3D.Point { X = 1, Y = -1, Z = -1 });
            interval = context.camera.interval * context.scale;
            light = context.world.Sun * rotationCamera;

            foreach (Entity e in context.world.entities)
                DrawEntity(e);
            fastBitmap.Dispose();
        }

        void DrawEntity(Entity entity)
        {
            if (entity is Base3D.Polytope pol)
                DrawPolytope(pol);
            if (entity is Base3D.Group group)
                DrawGroup(group);
        }

        void DrawGroup(Base3D.Group p)
        {
            foreach (Entity e in p.entities)
                DrawEntity(e);
        }

        void DrawPolytope(Base3D.Polytope p)
        {
            var points = new List<Base3D.Point>(p.points.Count);
            for (int i = 0; i < p.points.Count; ++i)
                points.Add(p.points[i] * cameraMatric);

            var normals = new List<Base3D.Point>(p.normals.Count);
            for (int i = 0; i < p.normals.Count; ++i)
                normals.Add(p.normals[i] * rotationCamera);

            Parallel.ForEach(p.polygons, polygon => DrawPolygon(polygon, points, normals, p.textures, p.Matreial));
        }

        void DrawPolygon(Base3D.Polygon pol, List<Base3D.Point> points, 
            List<Base3D.Point> normals, List<(double X, double Y)> textures, BaseMaterial material)
        {
            if (pol.indexes.Length < 3)
                return;

            var polyPoints = pol.Points(points);
            for (int i = 0; i < polyPoints.Length; ++i)
                if (!context.BeforeScreen(polyPoints[i].X))
                    return;

            if (pol.normals == null || !smoothing)
            {
                Base3D.Point p1 = polyPoints[0];
                var v1 = polyPoints[1] - p1;
                var v2 = polyPoints[2] - p1;
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

                for (int i = 0; i < polyPoints.Length - 2; ++i)
                {
                    Base3D.Point p2 = polyPoints[i + 1];
                    Base3D.Point p3 = polyPoints[i + 2];
                    if (pol.texture == null)
                        CheckedDrawTriangle(p1.Y, p1.Z, p2.Y, p2.Z, p3.Y, p3.Z,
                            new Stuff { Z = interval - p1.X, Normal = prod },
                            new Stuff { Z = interval - p2.X, Normal = prod },
                            new Stuff { Z = interval - p3.X, Normal = prod }, material);
                    else
                        CheckedDrawTriangle(p1.Y, p1.Z, p2.Y, p2.Z, p3.Y, p3.Z,
                            new Stuff { Z = interval - p1.X, Normal = prod, texture = textures[pol.texture[0]] },
                            new Stuff { Z = interval - p2.X, Normal = prod, texture = textures[pol.texture[i + 1]] },
                            new Stuff { Z = interval - p3.X, Normal = prod, texture = textures[pol.texture[i + 2]] }, material);
                } 
            }else
            {
                Base3D.Point p1 = polyPoints[0];
                Base3D.Point p2 = polyPoints[1];
                Base3D.Point p3 = polyPoints[2];

                if (p1.Y * (p3.Z - p2.Z) + p2.Y * (p1.Z - p3.Z) + p3.Y * (p2.Z - p1.Z) <= 0)
                    return;

                Base3D.Point norm1 = normals[pol.normals[0]];
                Base3D.Point norm2 = normals[pol.normals[1]];
                Base3D.Point norm3 = normals[pol.normals[2]];

                int i = 0;
                int count = polyPoints.Length - 2;
                while (true)
                {
                    if (pol.texture == null)
                        CheckedDrawTriangle(p1.Y, p1.Z, p2.Y, p2.Z, p3.Y, p3.Z,
                            new Stuff { Z = interval - p1.X, Normal = norm1 },
                            new Stuff { Z = interval - p2.X, Normal = norm2 },
                            new Stuff { Z = interval - p3.X, Normal = norm3 }, material);
                    else
                        CheckedDrawTriangle(p1.Y, p1.Z, p2.Y, p2.Z, p3.Y, p3.Z,
                            new Stuff { Z = interval - p1.X, Normal = norm1, texture = textures[pol.texture[0]] },
                            new Stuff { Z = interval - p2.X, Normal = norm2, texture = textures[pol.texture[i + 1]] },
                            new Stuff { Z = interval - p3.X, Normal = norm3, texture = textures[pol.texture[i + 2]] }, material);

                    ++i;
                    if (i >= count)
                        break;
                    p2 = polyPoints[i + 1];
                    p3 = polyPoints[i + 2];
                    norm2 = normals[pol.normals[i + 1]];
                    norm3 = normals[pol.normals[i + 2]];
                }
            }
        }

        byte colorScheme(double pow, double k, int C)
        {
            return (byte)Math.Min((int)(pow + C * k), 255);
        }

        void TrySet(int x, int y, Stuff s, BaseMaterial material)
        {
            if (x < 0 || y < 0 || x >= width || y >= height)
                return;
            if (s.Z <= buffer[x, y])
                return;
            buffer[x, y] = s.Z;

            var norm = s.Normal.Normal();
            double cos = norm.DotProd(light);
            var flip_light = 2 * cos * norm.X - light.X;
           
            cos = Math.Max(-flip_light , 0);

            double pow = Math.Pow(cos, phongPower) * phongSpecular * 255;
            double k = (phongAmbient + phongDiffuse * cos);

            Color color = material[s.texture];

            fastBitmap.SetPixel(x, y,
                colorScheme(pow, k, color.R),
                colorScheme(pow, k, color.G),
                colorScheme(pow, k, color.B));
        }

        void CheckedDrawTriangle(double x1, double y1, double x2, double y2, double x3, double y3,
            Stuff s1, Stuff s2, Stuff s3, BaseMaterial material)
        {
            if ((x1 < 0 || x1 >= width || x2 < 0 || x2 >= width || x3 < 0 || x3 >= width ||
                y1 < 0 || y1 >= height || y2 < 0 || y2 >= height || y3 < 0 || y3 >= height) &&
                (x1 < 0 && x2 < 0 && x3 < 0 || x1 >= width && x2 >= width && x3 >= width ||
                y1 < 0 && y2 < 0 && y3 < 0 || y1 >= height && y2 >= height && y3 >= height))
                return;

            DrawIntTriangle((int)x1, (int)y1, (int)x2, (int)y2, (int)x3, (int)y3, s1, s2, s3, material);
        }

        void DrawIntTriangle(int x1, int y1, int x2, int y2, int x3, int y3, 
            Stuff s1, Stuff s2, Stuff s3, BaseMaterial material)
        {
            if (y2 < y1)
                (x1, y1, x2, y2, s1, s2) = (x2, y2, x1, y1, s2, s1);
            if (y3 < y1)
                (x1, y1, x3, y3, s1, s3) = (x3, y3, x1, y1, s3, s1);
            if (y3 < y2)
                (x2, y2, x3, y3, s2, s3) = (x3, y3, x2, y2, s3, s2);

            int leftDy = y2 - y1;
            int rightDy = y3 - y1;
            bool leftIsSecond = x1 * (y3 - y2) + x2 * (y1 - y3) + x3 * (y2 - y1) > 0;
            int leftDx = x2 - x1;
            int rightDx = x3 - x1;

            Stuff leftDStf = (s2 - s1) / leftDy;
            Stuff rightDStf = (s3 - s1) / rightDy;

            if (!leftIsSecond)
            {
                (leftDy, rightDy) = (rightDy, leftDy);
                (leftDx, rightDx) = (rightDx, leftDx);
                (leftDStf, rightDStf) = (rightDStf, leftDStf);
            }

            int leftErr = 0;
            int rightErr = 0;
            int y = y1;

            int leftX;
            int rightX;
            Stuff rightStf;
            Stuff leftStf;

            if (y1 == y2)
            {
                if (x1 < x2)
                {
                    leftX = x1;
                    rightX = x2;
                    leftStf = s1;
                    rightStf = s2;
                }
                else
                {
                    leftX = x2;
                    rightX = x1;
                    leftStf = s2;
                    rightStf = s1;
                }
            }
            else
            {
                leftX = x1;
                rightX = x1;
                rightStf = s1;
                leftStf = s1;
            }

            for (; y < y2; ++y)
            {
                Stuff stf = leftStf;
                Stuff deltaStf = (rightStf - leftStf) / (rightX - leftX);
                for (int x = leftX; x < rightX; ++x)
                {
                    TrySet(x, y, stf, material);
                    stf += deltaStf;
                }

                leftStf += leftDStf;
                rightStf += rightDStf;
                leftErr += leftDx;
                rightErr += rightDx;
                if (Math.Abs(leftErr) >= leftDy)
                {
                    leftX += leftErr / leftDy;
                    leftErr = leftErr % leftDy;
                }
                if (Math.Abs(rightErr) >= rightDy)
                {
                    rightX += rightErr / rightDy;
                    rightErr = rightErr % rightDy;
                }
            }

            if (leftIsSecond)
            {
                leftDx = x3 - x2;
                leftDy = y3 - y2;
                leftDStf = (s3 - s2) / leftDy;
                leftErr = 0;
            }
            else
            {
                rightDx = x3 - x2;
                rightDy = y3 - y2;
                rightDStf = (s3 - s2) / rightDy;
                rightErr = 0;
            }

            for (; y < y3; ++y)
            {
                Stuff stf = leftStf;
                Stuff deltaStf = (rightStf - leftStf) / (rightX - leftX);
                for (int x = leftX; x < rightX; ++x)
                {
                    TrySet(x, y, stf, material);
                    stf += deltaStf;
                }

                rightStf += rightDStf;
                leftStf += leftDStf;
                leftErr += leftDx;
                rightErr += rightDx;
                if (Math.Abs(leftErr) >= leftDy)
                {
                    leftX += leftErr / leftDy;
                    leftErr = leftErr % leftDy;
                }
                if (Math.Abs(rightErr) >= rightDy)
                {
                    rightX += rightErr / rightDy;
                    rightErr = rightErr % rightDy;
                }
            }
        }

        struct Stuff
        {
            public (double X, double Y) texture;
            public Base3D.Point Normal;
            public double Z;

            public static Stuff operator +(Stuff s1, Stuff s2)
            {
                return new Stuff
                {
                    Normal = s1.Normal + s2.Normal,
                    Z = s1.Z + s2.Z,
                    texture =  {
                        X = s1.texture.X + s2.texture.X,
                        Y = s1.texture.Y + s2.texture.Y,
                    }
                };
            }
            
            public static Stuff operator -(Stuff s1, Stuff s2)
            {
                return new Stuff
                {
                    Normal = s1.Normal - s2.Normal,
                    Z = s1.Z - s2.Z,
                    texture = {
                        X = s1.texture.X - s2.texture.X,
                        Y = s1.texture.Y - s2.texture.Y,
                    }
                };
            }

            public static Stuff operator *(Stuff s, double c)
            {
                return new Stuff
                {
                    Normal = s.Normal * c,
                    Z = s.Z * c,
                    texture = {
                        X = s.texture.X * c,
                        Y = s.texture.Y * c,
                    }
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
                    Z = s.Z / c,
                    texture = {
                        X = s.texture.X / c,
                        Y = s.texture.Y / c,
                    }
                };
            }

        }
    }
}
