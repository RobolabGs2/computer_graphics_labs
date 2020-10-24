using Lab06.Base3D;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab06.Graph3D
{
    public class ZBuffer: IDrawing
    {
        Context context;
        Matrix cameraMatric;
        FastBitmap fastBitmap;
        double[,] buffer;
        double interval;
        int width;
        int height;

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
            interval = context.camera.interval * context.scale;
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
            var points = p.points.Select(point => point * cameraMatric).ToList();
            foreach (Polygon polygon in p.polygons)
                DrawPolygon(polygon, points, p.Matreial.Color);
        }

        Random r = new Random();
        void DrawPolygon(Base3D.Polygon pol, List<Base3D.Point> points, Color color)
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
            double cos = - prod.X / prod.Length();
            for (int i = 0; i < polyPoints.Count - 2; ++i)
                DrawTriangle(polyPoints[0], polyPoints[i + 1], polyPoints[i + 2], 
                    Color.FromArgb(0, 
                        (int)(cos * color.R), 
                        (int)(cos * color.G), 
                        (int)(cos * color.B)));
        }

        void TrySet(double x, double y, double z, Color color)
        {
            int ix = (int)x;
            int iy = (int)y;
            if (ix < 0 || iy < 0 || ix >= width || iy >= height)
                return;
            z = interval - z;
            if (z <= buffer[ix, iy])
                return;
            buffer[ix, iy] = z;
            fastBitmap.SetPixel(ix, iy, color);
        }

        void DrawTriangle(Base3D.Point p1, Base3D.Point p2, Base3D.Point p3, Color color)
        {
            if (p2.Z < p1.Z)
                (p1, p2) = (p2, p1);
            if (p3.Z < p1.Z)
                (p1, p3) = (p3, p1);
            if (p3.Z < p2.Z)
                (p2, p3) = (p3, p2);

            double startY = (p2.Z - p1.Z) * (p3.Y - p1.Y) / (p3.Z - p1.Z) + p1.Y;
            double startX = (p2.Z - p1.Z) * (p3.X - p1.X) / (p3.Z - p1.Z) + p1.X;
            double endY = p2.Y;
            double endX = p2.X;
            
            if (startY > endY)
            {
                (startY, endY) = (endY, startY);
                (startX, endX) = (endX, startX);
            }

            for (double i = p1.Z; i <= p2.Z; ++i)
            {
                double startJ = ((p2.Z - i) * p1.Y + (i - p1.Z) * startY) / (p2.Z - p1.Z);
                double endJ = ((p2.Z - i) * p1.Y + (i - p1.Z) * endY) / (p2.Z - p1.Z);
                double x1 = ((p2.Z - i) * p1.X + (i - p1.Z) * startX) / (p2.Z - p1.Z);
                double x2 = ((p2.Z - i) * p1.X + (i - p1.Z) * endX) / (p2.Z - p1.Z);

                for (double j = startJ; j < endJ; ++j)
                {
                    double x = ((endJ - j) * x1 + (j - startJ) * x2) / (endJ - startJ);
                    TrySet(j, i, x, color);
                }
            }

            for (double i = p2.Z; i <= p3.Z; ++i)
            {
                double startJ = ((p3.Z - i) * startY + (i - p2.Z) * p3.Y) / (p3.Z - p2.Z);
                double endJ = ((p3.Z - i) * endY + (i - p2.Z) * p3.Y) / (p3.Z - p2.Z);
                double x1 = ((p3.Z - i) * startX + (i - p2.Z) * p3.X) / (p3.Z - p2.Z);
                double x2 = ((p3.Z - i) * endX + (i - p2.Z) * p3.X) / (p3.Z - p2.Z);

                for (double j = startJ; j < endJ; ++j)
                {
                    double x = ((endJ - j) * x1 + (j - startJ) * x2) / (endJ - startJ);
                    TrySet(j, i, x, color);
                }
            }
        }
    }
}
