using Lab06.Base3D;
using Lab06.Materials3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Point = System.Drawing.Point;

namespace Lab06.Tools3D.Transformation
{
    class Transformation : IToolPage
    {
        public Bitmap Image => Properties.Resources.Move;
        Gizmo gizmo;

        public void Init(ToolTab tab, Context context)
        {
            gizmo = new Gizmo(context);
            foreach (var (icon, state) in new []
            {
                (Properties.Resources.Move, Gizmo.State.Move),
                (Properties.Resources.Scale, Gizmo.State.Scale),
                (Properties.Resources.Rotate, Gizmo.State.Rotate),
            })
            {
                var button = tab.AddButton(icon);
                button.ButtonClick += b => gizmo.CurrentState = state;
                button.ButtonDisable += b => gizmo.CurrentState = Gizmo.State.Unset;
            }
            
            var onlyGizmo = new CheckBox
                {Text = "Change gizmo", ForeColor = Constants.textColore, Font = Constants.font, Dock = DockStyle.Top};
            onlyGizmo.CheckStateChanged += (sender, args) => { gizmo.OnlyGizmo = onlyGizmo.Checked; };
            var resetGizmo = new Button()
            {
                Text = "Reset gizmo", ForeColor = Constants.textColore, Font = Constants.font, Dock = DockStyle.Top,
                AutoSize = true
            };
            resetGizmo.Click += (sender, args) =>
            {
                gizmo.RestartDirection();
                gizmo.CurrentState = gizmo.CurrentState;
            };
            var xyzSettings = new TableLayoutPanel
            {
                ColumnCount = 3, RowCount = 2, Dock = DockStyle.Top, ForeColor = Constants.textColore,
                AutoSize = true,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single
            };
            var axes = new[] {("X", Constants.xColor), ("Y", Constants.yColor), ("Z", Constants.zColor)};
            xyzSettings.Controls.AddRange(axes.Select(s => (Control) new Label
                {Text = s.Item1, TextAlign = ContentAlignment.TopCenter, Font = Constants.font}).ToArray());
            xyzSettings.SizeChanged += (sender, args) =>
            {
                for (int i = 0; i < xyzSettings.Controls.Count; i++)
                    xyzSettings.Controls[i].Width = xyzSettings.ClientSize.Width / 3 - 8;
            };
            var xyzInput = axes.Select(s =>
            {
                var textBox = new TextBox {Text = "", Font = Constants.font, ForeColor = s.Item2};
                gizmo.CurrentStateChanged += state =>
                {
                    switch (state)
                    {
                        case Gizmo.State.Unset:
                            textBox.Text = "-.-";
                            break;
                        case Gizmo.State.Move:
                            textBox.Text = "0.0";
                            break;
                        case Gizmo.State.Rotate:
                            textBox.Text = "0.0";
                            break;
                        case Gizmo.State.Scale:
                            textBox.Text = "1.0";
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(state), state, null);
                    }
                };
                return textBox;
            }).ToArray();
            xyzSettings.Controls.AddRange(xyzInput);
            var applyXYZ = new Button
            {
                Text = "Apply",
                ForeColor = Constants.textColore,
                Font = Constants.font,
                Dock = DockStyle.Top,
                AutoSize = true
            };
            gizmo.CurrentStateChanged += state =>
            {
                applyXYZ.Enabled = state != Gizmo.State.Unset;
                xyzSettings.Enabled = state != Gizmo.State.Unset;
            };
            applyXYZ.Click += (sender, args) =>
            {
                try
                {
                    gizmo.Transform(
                        double.Parse(xyzInput[0].Text, NumberStyles.Float, CultureInfo.InvariantCulture),
                        double.Parse(xyzInput[1].Text, NumberStyles.Float, CultureInfo.InvariantCulture),
                        double.Parse(xyzInput[2].Text, NumberStyles.Float, CultureInfo.InvariantCulture));
                    context.Redraw();
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Не удалось распарсить: {e.Message}", "Окошко-всплывашка", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            };
            tab.Settings.Controls.AddRange(new Control[] {applyXYZ, xyzSettings, onlyGizmo, resetGizmo}.Select(
                control =>
                {
                    control.Margin = new Padding(0, 8, 0, 8);
                    return control;
                }).ToArray());

            tab.TabSelected += () => gizmo.RestartDirection();
            tab.TabDeselected += () => gizmo.CurrentState = Gizmo.State.Unset;
            gizmo.CurrentState = Gizmo.State.Unset;
        }
    }

    class Gizmo
    {
        public enum State
        {
            Unset,
            Move,
            Rotate,
            Scale
        }

        public State CurrentState
        {
            get => currentState;
            set
            {
                if (currentState != State.Unset)
                {
                    context.world.control.Remove(xView);
                    context.world.control.Remove(yView);
                    context.world.control.Remove(zView);
                }

                if (currentState == State.Unset && value != State.Unset)
                {
                    context.pictureBox.MouseMove += MouseMoveHandler;
                    context.pictureBox.MouseUp += MouseUp;
                }

                switch (value)
                {
                    case State.Move:
                        InitMove();
                        break;
                    case State.Scale:
                        InitScale();
                        break;
                    case State.Rotate:
                        InitRotation();
                        break;
                    case State.Unset:
                        context.pictureBox.MouseMove -= MouseMoveHandler;
                        context.pictureBox.MouseUp -= MouseUp;
                        break;
                }

                if (State.Unset != value)
                {
                    context.world.control.Add(xView);
                    context.world.control.Add(yView);
                    context.world.control.Add(zView);
                }

                CurrentStateChanged(currentState = value);
                context.Redraw();
            }
        }

        public Action<State> CurrentStateChanged = (ignore) => { };

        private void MouseMoveHandler(object o, MouseEventArgs e)
        {
            switch (CurrentState)
            {
                case State.Unset:
                    break;
                case State.Move:
                    Move(o, e);
                    break;
                case State.Rotate:
                    Rotate(o, e);
                    break;
                case State.Scale:
                    Scale(o, e);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            context.Redraw();
        }

        public void Transform(double dx, double dy, double dz)
        {
            switch (CurrentState)
            {
                case State.Unset:
                    break;
                case State.Move:
                    Move(dx, dy, dz);
                    break;
                case State.Rotate:
                    Rotate(dx / 180 * Math.PI, dy / 180 * Math.PI, dz / 180 * Math.PI);
                    break;
                case State.Scale:
                    Scale(dx, dy, dz);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            context.Redraw();
        }

        private State currentState = State.Unset;

        /// <summary>
        /// Если true, перемещается только гизмо 
        /// </summary>
        public bool OnlyGizmo { get; set; } = false;

        Matrix Invert;
        Context context;
        double lastDistance = 0;
        bool xMoving = false;
        bool yMoving = false;
        bool zMoving = false;

        bool isRotation = false;
        Base3D.Point lastRotation;

        private Matrix Deformations = Matrix.Ident();
        public Base3D.Point location = new Base3D.Point { };

        /// <summary>
        /// Это всегда единичный вектор вдоль лькальной оси X
        /// </summary>
        public Base3D.Point xDirection = new Base3D.Point {X = 1};

        /// <summary>
        /// Это всегда единичный вектор вдоль лькальной оси Y
        /// </summary>
        public Base3D.Point yDirection = new Base3D.Point {Y = 1};

        /// <summary>
        /// Это всегда единичный вектор вдоль лькальной оси Z
        /// </summary>
        public Base3D.Point zDirection = new Base3D.Point {X = 1};

        Group xView;
        Group yView;
        Group zView;

        public Gizmo(Context context)
        {
            this.context = context;
        }

        public void RestartDirection()
        {
            xDirection = new Base3D.Point {X = 1};
            yDirection = new Base3D.Point {Y = 1};
            zDirection = new Base3D.Point {Z = 1};
            location = context.SeletedPivot();
            Deformations = Matrix.Move(location);
            Invert = Matrix.Ident();
        }

        public void InitRotation()
        {
            xView = Rotator(Constants.xColor);
            yView = Rotator(Constants.yColor);
            zView = Rotator(Constants.zColor);

            xView.Apply(Matrix.YRotation(-Math.PI / 2) * Matrix.ZRotation(Math.PI) * Matrix.XRotation(-Math.PI / 2) *
                        Deformations);
            yView.Apply(Matrix.XRotation(Math.PI / 2) * Matrix.ZRotation(Math.PI) * Matrix.YRotation(Math.PI / 2) *
                        Deformations);
            zView.Apply(Deformations);

            isRotation = true;
        }

        public void InitScale()
        {
            xView = Scelator(Constants.xColor);
            yView = Scelator(Constants.yColor);
            zView = Scelator(Constants.zColor);

            xView.Apply(Deformations);
            yView.Apply(Matrix.ZRotation(Math.PI / 2) * Deformations);
            zView.Apply(Matrix.YRotation(-Math.PI / 2) * Deformations);

            isRotation = false;
        }

        public void InitMove()
        {
            xView = Arrow(Constants.xColor);
            yView = Arrow(Constants.yColor);
            zView = Arrow(Constants.zColor);

            xView.Apply(Deformations);
            yView.Apply(Matrix.ZRotation(Math.PI / 2) * Deformations);
            zView.Apply(Matrix.YRotation(-Math.PI / 2) * Deformations);

            isRotation = false;
        }

        private Group Arrow(Color color)
        {
            Base3D.Group arrow = new Group();
            arrow.Matreial = new SolidMaterial(color);

            arrow.Add(Triangle());
            var triangle = Triangle();
            triangle.Apply(Matrix.XRotation(Math.PI / 2));
            arrow.Add(triangle);

            Spline spline = new Spline();
            spline.Matreial = new SolidMaterial();
            spline.Add(new Base3D.Point { });
            spline.Add(new Base3D.Point {X = 2});
            arrow.Add(spline);
            arrow.Apply(Matrix.Scale(new Base3D.Point {X = 0.3, Y = 0.3, Z = 0.3}));

            return arrow;
        }

        private Group Scelator(Color color)
        {
            Base3D.Group scelator = new Group();
            scelator.Matreial = new SolidMaterial(color);

            scelator.Add(Rectangle());
            var triangle = Rectangle();
            triangle.Apply(Matrix.XRotation(Math.PI / 2));
            scelator.Add(triangle);

            Spline spline = new Spline();
            spline.Matreial = new SolidMaterial();
            spline.Add(new Base3D.Point { });
            spline.Add(new Base3D.Point {X = 2});
            scelator.Add(spline);
            scelator.Apply(Matrix.Scale(new Base3D.Point {X = 0.3, Y = 0.3, Z = 0.3}));

            return scelator;
        }

        private Group Rotator(Color color)
        {
            Base3D.Group rotator = new Group();
            rotator.Matreial = new SolidMaterial(color);

            rotator.Add(Circle());

            Spline spline = new Spline();
            spline.Matreial = new SolidMaterial();
            spline.Add(new Base3D.Point { });
            spline.Add(new Base3D.Point {Z = 2});
            rotator.Add(spline);

            rotator.Apply(Matrix.Scale(new Base3D.Point {X = 0.3, Y = 0.3, Z = 0.3}));

            return rotator;
        }

        private Polytope Triangle()
        {
            Polytope triangle = new Polytope();
            triangle.Matreial = new SolidMaterial();

            triangle.Add(new Base3D.Point {X = 3});
            triangle.Add(new Base3D.Point {X = 2, Y = 0.5});
            triangle.Add(new Base3D.Point {X = 2, Y = -0.5});
            triangle.polygons.Add(new Polygon(new int[] {0, 1, 2}));
            triangle.polygons.Add(new Polygon(new int[] {0, 2, 1}));
            return triangle;
        }

        private Polytope Rectangle()
        {
            Polytope triangle = new Polytope();
            triangle.Matreial = new SolidMaterial();

            triangle.Add(new Base3D.Point {X = 2, Y = -0.5});
            triangle.Add(new Base3D.Point {X = 2, Y = 0.5});
            triangle.Add(new Base3D.Point {X = 3, Y = 0.5});
            triangle.Add(new Base3D.Point {X = 3, Y = -0.5});
            triangle.polygons.Add(new Polygon(new int[] {0, 1, 2, 3}));
            triangle.polygons.Add(new Polygon(new int[] {3, 2, 1, 0}));
            return triangle;
        }

        private Polytope Circle()
        {
            Polytope triangle = new Polytope();
            triangle.Matreial = new SolidMaterial();

            triangle.Add(new Base3D.Point {X = 1, Y = 1});
            triangle.Add(new Base3D.Point {X = 3, Y = 1});
            triangle.Add(new Base3D.Point {X = 1, Y = 3});
            triangle.polygons.Add(new Polygon(new int[] {0, 1, 2}));
            triangle.polygons.Add(new Polygon(new int[] {0, 2, 1}));
            return triangle;
        }

        private double Distance(MouseEventArgs e, Matrix vMatrix)
        {
            Base3D.Point p0 = (new Base3D.Point {Y = e.X, Z = e.Y} * context.InvertDrawingMatrix());
            Base3D.Point p1 = (new Base3D.Point {Y = e.X, Z = e.Y, X = 1} * context.InvertDrawingMatrix());
            Matrix deMove = Matrix.Move(-location) * Invert;
            p0 = p0 * deMove * vMatrix;
            p1 = p1 * deMove * vMatrix;
            double A = p1.X - p0.X;
            double B = p1.Y - p0.Y;
            double C = p1.Z - p0.Z;
            double x = p0.X;
            double y = p0.Y;
            double z = p0.Z;
            return ((C * x - A * y) * (B * z - C * y) + (B * x - A * y) * (C * y - B * z)) /
                   (B * C * (z + y) - B * B * z - C * C * y);
        }

        private (Base3D.Point p, double angle) RotationPoint(MouseEventArgs e, Matrix vMatrix)
        {
            Base3D.Point p1 = (new Base3D.Point {Y = e.X, Z = e.Y} * context.InvertDrawingMatrix());
            Base3D.Point p2 = (new Base3D.Point {Y = e.X, Z = e.Y, X = 1} * context.InvertDrawingMatrix());
            Matrix deMove = Matrix.Move(-location) * Invert;
            p1 = p1 * deMove * vMatrix * Matrix.YRotation(Math.PI / 2);
            p2 = p2 * deMove * vMatrix * Matrix.YRotation(Math.PI / 2);

            double denum = p2.Z - p1.Z;
            double numX = -(p2.X - p1.X) * p2.Z;
            double numY = -(p2.Y - p1.Y) * p2.Z;
            Base3D.Point newPoint = new Base3D.Point {X = numX / denum + p2.X, Y = numY / denum + p2.Y};

            double len = Math.Sqrt(newPoint.Y * newPoint.Y + newPoint.X * newPoint.X) *
                         Math.Sqrt(lastRotation.Y * lastRotation.Y + lastRotation.X * lastRotation.X);
            if (len == 0)
                return (newPoint, 0);
            double cos = (newPoint.Y * lastRotation.Y + newPoint.X * lastRotation.X) / len;
            double sin = (newPoint.Y * lastRotation.X - newPoint.X * lastRotation.Y) / len;
            Console.WriteLine($"cos = {cos,4:f4}, sin = {sin,4:f4}," +
                              $" {{{lastRotation.X,4:f4}, {lastRotation.Y,4:f4}, {lastRotation.Z,4:f4}}}" +
                              $" {{{newPoint.X,4:f4}, {newPoint.Y,4:f4}, {newPoint.Z,4:f4}}}");
            return (newPoint, -Math.Acos(cos) * Math.Sign(sin));
        }


        private bool NowMoving(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return false;
            if (xMoving || yMoving || zMoving)
                return true;

            if (context.ScreenPointInGroup(e.X, e.Y, xView))
            {
                if (isRotation)
                    lastRotation = RotationPoint(e, Matrix.Ident()).p;
                else
                    lastDistance = Distance(e, Matrix.Ident());
                xMoving = true;
            }
            else if (context.ScreenPointInGroup(e.X, e.Y, yView))
            {
                if (isRotation)
                    lastRotation = RotationPoint(e, Matrix.ZRotation(-Math.PI / 2)).p;
                else
                    lastDistance = Distance(e, Matrix.ZRotation(-Math.PI / 2));
                yMoving = true;
            }
            else if (context.ScreenPointInGroup(e.X, e.Y, zView))
            {
                if (isRotation)
                    lastRotation = RotationPoint(e, Matrix.YRotation(Math.PI / 2)).p;
                else
                    lastDistance = Distance(e, Matrix.YRotation(Math.PI / 2));
                zMoving = true;
            }

            return false;
        }

        private Matrix RotationForDistance()
        {
            if (yMoving)
                return Matrix.ZRotation(-Math.PI / 2);
            if (zMoving)
                return Matrix.YRotation(Math.PI / 2);
            return Matrix.Ident();
        }

        private void Move(object s, MouseEventArgs e)
        {
            if (!NowMoving(e))
                return;
            Matrix movingMatrix = Matrix.Ident();
            double dist = Distance(e, RotationForDistance());
            var dt = dist - lastDistance;
            Move(xMoving ? dt : 0, yMoving ? dt : 0, zMoving ? dt : 0);
            lastDistance = Distance(e, RotationForDistance());
        }

        public void Move(double distX, double distY, double distZ)
        {
            Matrix movingMatrix = Matrix.Move(xDirection.vectorMult(distX)) *
                                  Matrix.Move(yDirection.vectorMult(distY)) *
                                  Matrix.Move(zDirection.vectorMult(distZ));
            location *= movingMatrix;
            xView.Apply(movingMatrix);
            yView.Apply(movingMatrix);
            zView.Apply(movingMatrix);
            Deformations *= movingMatrix;
            if (!OnlyGizmo)
                context.world.SelectedApply(movingMatrix);
        }

        private void Scale(object s, MouseEventArgs e)
        {
            if (!NowMoving(e))
                return;
            Matrix movingMatrix = Matrix.Ident();
            double scale = Distance(e, RotationForDistance());
            var dt = scale / lastDistance;
            Scale(xMoving ? dt : 1, yMoving ? dt : 1, zMoving ? dt : 1);
            lastDistance = Distance(e, RotationForDistance());
        }

        public void Scale(double scaleX, double scaleY, double scaleZ)
        {
            Matrix movingMatrix = Matrix.Move(-location) *
                                  Matrix.Scale(xDirection, scaleX) *
                                  Matrix.Scale(yDirection, scaleY) *
                                  Matrix.Scale(zDirection, scaleZ) *
                                  Matrix.Move(location);
            location = location * movingMatrix;
            xView.Apply(movingMatrix);
            yView.Apply(movingMatrix);
            zView.Apply(movingMatrix);
            Deformations = Deformations * movingMatrix;
            if (!OnlyGizmo)
                context.world.SelectedApply(movingMatrix);
        }

        private void Rotate(object s, MouseEventArgs e)
        {
            if (!NowMoving(e))
                return;
            (var newPoint, double angle) = RotationPoint(e, RotationForDistance());
            Rotate(xMoving ? angle : 0, yMoving ? angle : 0, zMoving ? angle : 0);
            (lastRotation, angle) = RotationPoint(e, RotationForDistance());
        }

        private void Rotate(double angleX, double angleY, double angleZ)
        {
            Matrix rotateMatrix = Matrix.Rotation(xDirection, angleX) *
                                  Matrix.Rotation(yDirection, angleY) *
                                  Matrix.Rotation(zDirection, angleZ);

            Matrix movingMatrix = Matrix.Move(-location) *
                                  rotateMatrix *
                                  Matrix.Move(location);

            Invert *= Matrix.Rotation(xDirection, -angleX) *
                      Matrix.Rotation(yDirection, -angleY) *
                      Matrix.Rotation(yDirection, -angleZ);

            xView.Apply(movingMatrix);
            yView.Apply(movingMatrix);
            zView.Apply(movingMatrix);
            xDirection *= rotateMatrix;
            yDirection *= rotateMatrix;
            zDirection *= rotateMatrix;
            Deformations *= movingMatrix;
            if (!OnlyGizmo)
                context.world.SelectedApply(movingMatrix);
        }

        private void MouseUp(object s, MouseEventArgs e)
        {
            xMoving = false;
            yMoving = false;
            zMoving = false;
        }
    }
}