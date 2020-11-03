using Lab06.Base3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Lab06.Materials3D;
using Point = Lab06.Base3D.Point;

namespace Lab06.Tools3D.AddItem
{
    class AddItem : IToolPage
    {
        Context context;
        OpenFileDialog openFileDialog = new OpenFileDialog();
        public Bitmap Image => Properties.Resources.Cube;

        string filePath;

        private static List<List<int>> matrix = new List<List<int>>
        {
            new List<int> {1, -3, 3, -1},
            new List<int> {0, 3, -6, 3},
            new List<int> {0, 0, 3, -3},
            new List<int> {0, 0, 0, 1}
        };

        public void Init(ToolTab tab, Context context)
        {
            this.context = context;
            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            openFileDialog.Filter = "obj files (*.obj)|*.obj";
            openFileDialog.RestoreDirectory = true;

            Item.AddButton(tab.AddButton(Properties.Resources.Point, true), GeneratePoint, context);
            Item.AddButton(tab.AddButton(Properties.Resources.Cube, true), GenerateCube, context);
            Item.AddButton(tab.AddButton(Properties.Resources.Octahedron, true), GenerateOcta, context);
            Item.AddButton(tab.AddButton(Properties.Resources.Tetrahedron, true), GenerateTetra, context);
            Item.AddButton(tab.AddButton(Properties.Resources.Icosahedron, true), GenerateIcosa, context);
            Item.AddButton(tab.AddButton(Properties.Resources.Dodecahedron, true), GenerateDodeca, context);
            tab.AddButton(Properties.Resources.SplineBezier, false).ButtonClick += (a) => GenerateBezier();
            MultiItem<Spline, Spline>.AddButton(tab.AddButton(Properties.Resources.Spline),
                () => new Spline(),
                (total, partial) => total.Add(partial.points.First()),
                GeneratePoint,
                context
            );

            var load = tab.AddButton(Properties.Resources.Load, true);
            load.ButtonClick += b => ChangeFile();
            Item.AddButton(load, GenerateObj, context);

            Polytope sun = Primitives.Octahedron(1); 
            sun.Matreial = new SolidMaterial(Color.Yellow);
            sun.Apply(Matrix.Move(context.world.Sun));
            var lastPos = context.world.Sun;
            var sunSettings = SunControl.Settings(point =>
            {
                var newPos = point*(context.camera.location.Length()/3);
                sun.Apply(Matrix.Move(newPos-lastPos));
                lastPos = newPos;
                context.world.Sun = point;
                context.Redraw();
            });
            tab.Settings.Controls.Add(sunSettings);
            var sunButton = tab.AddButton(Properties.Resources.Sun);
            sunButton.ButtonClick += button =>
            {
                sunSettings.Visible = true;
                context.world.entities.Add(sun);
                context.Redraw();
            };
            sunButton.ButtonDisable += button =>
            {
                sunSettings.Visible = false;
                context.world.entities.Remove(sun);
                context.Redraw();
            };
        }

        class SunControl
        {
            public static Control Settings(Action<Point> newPosition)
            {
                var sunSettings = new FlowLayoutPanel { Dock = DockStyle.Fill, Visible = false, FlowDirection = FlowDirection.LeftToRight};
                new SunControl(sunSettings).NewPosition += newPosition;
                return sunSettings;
            }
            
            private readonly PictureBox sunXY;
            public event Action<Point> NewPosition;
            private float Z = 0F;
            private float X = 1F;
            private float Y = 0F;
            private float xy => 1 - Z * Z;
            private readonly Pen orbitPen = new Pen(Color.Yellow, 1);
            private readonly Pen sunPen = new Pen(Color.Yellow, 10);
            private void Redraw()
            {
                var width = sunXY.Width;
                var d = width * 90F / 100F * xy;
                var r = d / 2F;
                var upperLeft = (width - d) / 2;
                var bitmap = new Bitmap(width, width);
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.DrawEllipse(orbitPen, upperLeft, upperLeft, d, d);
                    g.DrawEllipse(sunPen, -Y * r + width / 2F - 5, -X * r + width / 2F - 5, 10, 10);
                }
                sunXY.Image?.Dispose();
                sunXY.Image = bitmap;
            }

            public SunControl(Control parent)
            {

                sunXY = new PictureBox();
                var sunZ = new TrackBar
                {
                   Orientation = Orientation.Vertical, Minimum = -100, Maximum = 100, Value = 0,
                };
                parent.Controls.Add(sunXY);
                parent.Controls.Add(sunZ);
                parent.Resize += (sender, args) =>
                {
                    var width = Math.Min(parent.ClientSize.Width, parent.ClientSize.Height) - parent.Margin.Vertical - sunZ.Width - 8;
                    if (parent.Width <= 0)
                        return;
                    sunXY.Size = new Size(width, width);
                    sunZ.Height = width;
                    Redraw();
                };
                sunXY.MouseMove += (sender, args) =>
                {
                    if ((args.Button & MouseButtons.Left) == 0)
                        return;
                    var delta = new Point {X = args.X - sunXY.Width / 2, Y = args.Y - sunXY.Height / 2};
                    var r = (float) delta.Length();
                    Y = -(float) delta.X / r;
                    X = -(float) delta.Y / r;
                    Redraw();
                    NewPosition?.Invoke(new Point {X = X * xy, Y = Y * xy, Z = Z});
                };
                sunZ.ValueChanged += (sender, args) =>
                {
                    Z = sunZ.Value / 100F;
                    Redraw();
                    NewPosition?.Invoke(new Point {X = X * xy, Y = Y * xy, Z = Z});
                };
            }
        }

        void ChangeFile()
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                filePath = openFileDialog.FileName;
        }

        Entity GenerateObj()
        {
            try
            {
                return Obj.Parse(filePath);
            }
            catch(Exception e)
            {
                MessageBox.Show($"Не вышло загрузить файл: {e.Message}", "Окошко-всплывашка", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return new Group();
            }
        }

        Spline GeneratePoint()
        {
            Spline spline = new Spline();
            spline.Add(new Base3D.Point());
            return spline;
        }

        Polytope GenerateCube() => Primitives.Cube(2);

        Polytope GenerateTetra() => Primitives.Tetrahedron(2);


        Polytope GenerateOcta() => Primitives.Octahedron(2);


        Polytope GenerateIcosa()
        {
            return Primitives.Icosahedron();
        }

        Polytope GenerateDodeca()
        {
            Polytope icosa = GenerateIcosa();
            Polytope dodeca = new Polytope();

            foreach (var edge in icosa.polygons)
                dodeca.Add(new Point
                {
                    X = edge.Points(icosa).Sum(p => p.X) / 3, Y = edge.Points(icosa).Sum(p => p.Y) / 3,
                    Z = edge.Points(icosa).Sum(p => p.Z) / 3
                });

            dodeca.Add(new Polygon(new int[] {16, 12, 8, 4, 0}));
            for (int i = 0, j = 3; i < dodeca.points.Count; i += 4, j += 4)
            {
                dodeca.Add(new Polygon(new int[] {(i + 4) % 20, (i + 5) % 20, i + 3, i + 1, i}));
                dodeca.Add(new Polygon(new int[] {j, j - 1, (20 + j - 4) % 20 - 1, (20 + j - 4) % 20, i + 1}));
            }

            dodeca.Add(new Polygon(new int[] {2, 6, 10, 14, 18}));

            return dodeca;
        }

        private Point Bezier3(Point p0, Point p1, Point p2, Point p3, double t, double z)
        {
            List<Point> mt = new List<Point> {p0, p1, p2, p3};
            List<Point> res = new List<Point>();
            Point r = new Point();
            for (int i = 0; i < matrix.Count; i++)
            {
                double x = 0;
                double y = 0;
                for (int j = 0; j < matrix[i].Count; j++)
                {
                    x += mt[j].X * matrix[j][i];
                    y += mt[j].Y * matrix[j][i];
                }

                res.Add(new Point {X = x, Y = y, Z = z});
            }

            for (int i = 0; i < res.Count; i++)
            {
                r.X += res[i].X * Math.Pow(t, i);
                r.Y += res[i].Y * Math.Pow(t, i);
            }

            return r;
        }

        void GenerateBezier()
        {
            List<Spline> spline = new List<Spline>();
            foreach (var select in context.world.selected)
            {
                Spline s = select as Spline;
                if (s != null && s.points.Count > 3)
                {
                    var copypoints = new LinkedList<Point>();
                    int count = (s.points.Count + 1) / 2 - 1;
                    bool flag = s.points.Count % 2 != 0;
                    foreach (var point in s.points)
                    {
                        copypoints.AddLast(point);
                    }

                    LinkedListNode<Point> p0 = copypoints.First;
                    spline.Add(new Spline());
                    while (count != 0)
                    {
                        LinkedListNode<Point> p1, p2, p3;
                        p1 = p0.Next;
                        p2 = p1.Next;

                        if (flag && count == 1)
                            p3 = p2;
                        else if (!flag && count == 1)
                        {
                            p2 = p2.Next;
                            p3 = p2;
                        }
                        else if (!flag && count == 1)
                        {
                            p2 = p1;
                            p3 = p2.Next;
                            count++;
                            flag = true;
                        }
                        else p3 = p2.Next;

                        var p = new Point
                        {
                            X = (p2.Value.X + p3.Value.X) / 2, Y = (p2.Value.Y + p3.Value.Y) / 2,
                            Z = (p2.Value.Z + p3.Value.Z) / 2
                        }; //p'

                        Point q0 = p0.Value;
                        for (float t = (float) 0.12; t <= 1.0; t += (float) 0.02)
                        {
                            var q1 = Bezier3(p0.Value, p1.Value, p2.Value, p, t, p.Z);
                            spline.Last().Add(q0);
                            q0 = q1;
                        }

                        spline.Last().Add(q0);
                        p0 = copypoints.AddAfter(p2, p); //p'
                        count--;
                    }
                }
            }

            context.world.selected.Clear();
            foreach (var s in spline)
            {
                context.world.entities.Add(s);
                context.world.selected.Add(s);
            }

            context.Redraw();
        }
    }
}