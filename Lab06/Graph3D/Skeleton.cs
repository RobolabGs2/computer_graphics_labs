using Lab06.Base3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab06.Graph3D
{
    class Skeleton : IDrawing
    {
        Context context;
        Graphics graphics;
        Pen selectedPen;
        Pen basePen;
        Matrix cameraMatric;

        public Skeleton(Context context)
        {
            selectedPen = new Pen(Constants.textColore, 2);
            basePen = new Pen(Constants.borderColore, 2);
            this.context = context;
        }

        public void Draw(Bitmap bitmap)
        {
            cameraMatric = context.DrawingMatrix();

            graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Constants.backColore);
            foreach (Entity e in context.world.entities)
                DrawEntity(e, basePen);
            graphics.Dispose();
        }

        void DrawEntity(Entity entity, Pen pen)
        {
            if (entity is Base3D.Point p)
                DrawPoint(p, pen);
            if (entity is Base3D.Spline s)
                DrawSpline(s, pen);
            if (entity is Base3D.Polytope pol)
                DrawPolytope(pol, pen);
        }

        void DrawPolytope(Base3D.Polytope p, Pen pen)
        {
            foreach (Polygon polygon in p.polygons)
                Drawpolygon(polygon, pen);
        }

        void Drawpolygon(Base3D.Polygon pol, Pen pen)
        {
            if (pol.points.Count < 3)
                return;

            var points = pol.points.Select(p => p * cameraMatric);
            var end = points.Aggregate((p1, p2) => { DrawLine(p1, p2, pen); return p2; });
            DrawLine(end, points.First(), pen);
        }

        void DrawPoint(Base3D.Point point, Pen pen)
        {
            Base3D.Point p = point * cameraMatric;
            if (!context.BeforeScreen(p.X))
                return;
            p = p.FlattenT();
            float r = 3;
            graphics.DrawEllipse(pen, 
                (float)(p.Y - r), (float)(p.Z - r), r * 2, r * 2);
        }

        void DrawSpline(Base3D.Spline s, Pen pen)
        {
            if (s.points.Count == 0)
                return;
            if (s.points.Count == 1)
            {
                DrawPoint(s.points[0], pen);
                return;
            }

            s.points.Select(p => p * cameraMatric).
                Aggregate((p1, p2) => { DrawLine(p1, p2, pen); return p2; });
        }

        void DrawLine(Base3D.Point p1, Base3D.Point p2, Pen pen)
        {
            if (!context.BeforeScreen(p1.X) || !context.BeforeScreen(p2.X))
                return;
            p1 = p1.Copy().FlattenT();
            p2 = p2.Copy().FlattenT();

            graphics.DrawLine(pen, (float)p1.Y, (float)p1.Z, (float)p2.Y, (float)p2.Z);
        }
    }
}
