﻿using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace Lab04
{
    public class Context
    {
        public HashSet<Polygon> Polygons { get; set; }
        public HashSet<Polygon> Selected { get; set; }
        public Pen Pen { get; set; }

        public bool Debug
        {
            get => Pen.EndCap == LineCap.Custom;
            set
            {
                if (value)
                    Pen.CustomEndCap = new AdjustableArrowCap(5, 5);
                else
                    Pen.EndCap = LineCap.Flat;
            }
        }

        private readonly Pen abbaPen = new Pen(Color.Gray)
        {
            DashStyle = DashStyle.Dash
        };
        private readonly Pen notSelectedPen = new Pen(Color.Black, 2);
        private readonly Font font = new Font("Consolas", 12);

        public Context()
        {
            Polygons = new HashSet<Polygon>();
            Selected = new HashSet<Polygon>();
            Pen = new Pen(Color.Red, 2);
        }

        public (Point p1, Point p2) SelectedABBA(Matrix m = null)
        {
            if (Selected.Count == 0)
                return (new Point(), new Point());
            return Point.ABBA(Selected.SelectMany(p => m == null ? p.Points : p.Points.Select(pt => pt * m)));
        }

        public void Add(Polygon p)
        {
            Polygons.Add(p);
        }

        public void Apply(Matrix matrix)
        {
            foreach (Polygon p in Selected)
                p.Apply(matrix);
        }
        public void Draw(Graphics g, Matrix matrix)
        {
            foreach (var p in Polygons.Where(p=>!Selected.Contains(p)))
            {
                p.Draw(g, notSelectedPen);
            }

            foreach (var p in Selected)
            {
                p.Draw(g, Pen, matrix);
                if (Debug)
                {
                    for (int i = 0; i < p.Points.Count; i++)
                    {
                        g.DrawString(i.ToString(), font, notSelectedPen.Brush, (p.Points[i] * matrix).ToPointF());
                    }
                }
            }

            var rect = SelectedABBA(matrix);
            if (rect.p1 != rect.p2)
            {
                var p1 = rect.p1.ToPointF();
                var p2 = (rect.p2 - rect.p1).ToPointF();
                g.DrawRectangle(abbaPen, p1.X, p1.Y, p2.X, p2.Y);
            }
        }
    }
}