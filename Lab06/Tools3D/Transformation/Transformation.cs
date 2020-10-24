﻿using Lab06.Base3D;
using Lab06.Materials3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab06.Tools3D.Transformation
{
    class Transformation : IToolPage
    {
        public Bitmap Image => Properties.Resources.Move;
        Gizmo gizmo;

        public void Init(ToolTab tab, Context context)
        {
            gizmo = new Gizmo(context);
            var moveButton = tab.AddButton(Properties.Resources.Move);
            moveButton.ButtonClick += b => gizmo.InitMove();
            moveButton.ButtonDisable += b => gizmo.Delete();

            var scaleButton = tab.AddButton(Properties.Resources.Scale);
            scaleButton.ButtonClick += b => gizmo.InitScale();
            scaleButton.ButtonDisable += b => gizmo.Delete();

            var rotateButton = tab.AddButton(Properties.Resources.Rotate);
            rotateButton.ButtonClick += b => gizmo.InitRotation();
            rotateButton.ButtonDisable += b => gizmo.Delete();

            var onlyGizmo = new CheckBox{Text = "Move gizmo", ForeColor = Constants.textColore, Font = Constants.font, Dock = DockStyle.Top};
            onlyGizmo.CheckStateChanged += (sender, args) =>
            {
                gizmo.OnlyGizmo = onlyGizmo.Checked;
            };
            tab.Settings.Controls.Add(onlyGizmo);
            var resetGizmo = new Button() { Text = "Reset gizmo", ForeColor = Constants.textColore, Font = Constants.font, Dock = DockStyle.Top, AutoSize = true};
            resetGizmo.Click += (sender, args) =>
            {
                // TODO
                gizmo.RestartDirection();
                gizmo.Delete();
                if (moveButton.ButtonEnable)
                {
                    gizmo.InitMove();
                }
                if (rotateButton.ButtonEnable)
                {
                    gizmo.InitRotation();
                }
                if (scaleButton.ButtonEnable)
                {
                    gizmo.InitScale();
                }
                context.Redraw();
            };
            tab.Settings.Controls.Add(resetGizmo);

            tab.TabSelected += () => gizmo.RestartDirection();
        }
    }

    class Gizmo
    {
        /// <summary>
        /// Если true, перемещается толькл гизмо 
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
        public Base3D.Point xDirection = new Base3D.Point { X = 1 };
        /// <summary>
        /// Это всегда единичный вектор вдоль лькальной оси Y
        /// </summary>
        public Base3D.Point yDirection = new Base3D.Point { Y = 1 };
        /// <summary>
        /// Это всегда единичный вектор вдоль лькальной оси Z
        /// </summary>
        public Base3D.Point zDirection = new Base3D.Point { X = 1 };

        Group xView;
        Group yView;
        Group zView;

        public Gizmo(Context context)
        {
            this.context = context;
        }

        public void RestartDirection()
        {
            xDirection = new Base3D.Point { X = 1 };
            yDirection = new Base3D.Point { Y = 1 };
            zDirection = new Base3D.Point { Z = 1 };
            location = context.SeletedPivot();
            Deformations = Matrix.Move(location);
            Invert = Matrix.Ident();
        }

        public void InitRotation()
        {
            xView = Rotator(Color.DarkRed);
            yView = Rotator(Color.DarkBlue);
            zView = Rotator(Color.DarkGreen);

            xView.Apply(Matrix.YRotation(-Math.PI / 2) * Matrix.ZRotation(Math.PI) * Matrix.XRotation(-Math.PI / 2) * Deformations);
            yView.Apply(Matrix.XRotation(Math.PI / 2) * Matrix.ZRotation(Math.PI) * Matrix.YRotation(Math.PI / 2) * Deformations);
            zView.Apply(Deformations);

            context.world.control.Add(xView);
            context.world.control.Add(yView);
            context.world.control.Add(zView);
            context.Redraw();

            context.pictureBox.MouseMove += Rotate;
            context.pictureBox.MouseUp += MouseUp;
            isRotation = true;
        }

        public void InitScale()
        {
            xView = Scelator(Color.DarkRed);
            yView = Scelator(Color.DarkBlue);
            zView = Scelator(Color.DarkGreen);

            xView.Apply(Deformations);
            yView.Apply(Matrix.ZRotation(Math.PI / 2) * Deformations);
            zView.Apply(Matrix.YRotation(-Math.PI / 2) * Deformations);

            context.world.control.Add(xView);
            context.world.control.Add(yView);
            context.world.control.Add(zView);
            context.Redraw();

            context.pictureBox.MouseMove += Scale;
            context.pictureBox.MouseUp += MouseUp;
            isRotation = false;
        }

        public void InitMove()
        {
            xView = Arrow(Color.DarkRed);
            yView = Arrow(Color.DarkBlue);
            zView = Arrow(Color.DarkGreen);

            xView.Apply(Deformations);
            yView.Apply(Matrix.ZRotation(Math.PI / 2) * Deformations);
            zView.Apply(Matrix.YRotation(-Math.PI / 2) * Deformations);

            context.world.control.Add(xView);
            context.world.control.Add(yView);
            context.world.control.Add(zView);
            context.Redraw();

            context.pictureBox.MouseMove += Move;
            context.pictureBox.MouseUp += MouseUp;
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
            spline.Add(new Base3D.Point { X = 2 });
            arrow.Add(spline);
            arrow.Apply(Matrix.Scale(new Base3D.Point { X = 0.3, Y = 0.3, Z = 0.3 }));

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
            spline.Add(new Base3D.Point { X = 2 });
            scelator.Add(spline);
            scelator.Apply(Matrix.Scale(new Base3D.Point { X = 0.3, Y = 0.3, Z = 0.3 }));

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
            spline.Add(new Base3D.Point { Z = 2 });
            rotator.Add(spline);

            rotator.Apply(Matrix.Scale(new Base3D.Point { X = 0.3, Y = 0.3, Z = 0.3 }));

            return rotator;
        }

        private Polytope Triangle()
        {
            Polytope triangle = new Polytope();
            triangle.Matreial = new SolidMaterial();

            triangle.Add(new Base3D.Point { X = 3 });
            triangle.Add(new Base3D.Point { X = 2, Y = 0.5 });
            triangle.Add(new Base3D.Point { X = 2, Y = -0.5 });
            triangle.polygons.Add(new Polygon(new int[] {0, 1, 2 }));
            triangle.polygons.Add(new Polygon(new int[] { 0, 2, 1 }));
            return triangle;
        }

        private Polytope Rectangle()
        {
            Polytope triangle = new Polytope();
            triangle.Matreial = new SolidMaterial();

            triangle.Add(new Base3D.Point { X = 2, Y = -0.5 });
            triangle.Add(new Base3D.Point { X = 2, Y = 0.5 });
            triangle.Add(new Base3D.Point { X = 3, Y = 0.5 });
            triangle.Add(new Base3D.Point { X = 3, Y = -0.5 });
            triangle.polygons.Add(new Polygon(new int[] { 0, 1, 2, 3 }));
            triangle.polygons.Add(new Polygon(new int[] { 3, 2, 1, 0 } ));
            return triangle;
        }
        private Polytope Circle()
        {
            Polytope triangle = new Polytope();
            triangle.Matreial = new SolidMaterial();

            triangle.Add(new Base3D.Point { X = 1, Y = 1 });
            triangle.Add(new Base3D.Point { X = 3, Y = 1});
            triangle.Add(new Base3D.Point { X = 1, Y = 3});
            triangle.polygons.Add(new Polygon(new int[] { 0, 1, 2 }));
            triangle.polygons.Add(new Polygon(new int[] { 0, 2, 1 }));
            return triangle;
        }

        private double Distance(MouseEventArgs e, Matrix vMatrix)
        {
            Base3D.Point p0 = (new Base3D.Point { Y = e.X, Z = e.Y } * context.InvertDrawingMatrix());
            Base3D.Point p1 = (new Base3D.Point { Y = e.X, Z = e.Y, X = 1 } * context.InvertDrawingMatrix());
            Matrix deMove = Matrix.Move(-location) * Invert;
            p0 = p0 * deMove * vMatrix;
            p1 = p1 * deMove * vMatrix;
            double A = p1.X - p0.X;
            double B = p1.Y - p0.Y;
            double C = p1.Z - p0.Z;
            double x = p0.X;
            double y = p0.Y;
            double z = p0.Z;
            return ((C * x - A * y) * (B * z - C  * y) + (B * x - A * y) * (C * y - B * z)) / 
                (B * C * (z + y) - B * B * z - C * C * y);
        }

        private (Base3D.Point p, double angle) RotationPoint(MouseEventArgs e, Matrix vMatrix)
        {
            Base3D.Point p1 = (new Base3D.Point { Y = e.X, Z = e.Y } * context.InvertDrawingMatrix());
            Base3D.Point p2 = (new Base3D.Point { Y = e.X, Z = e.Y, X = 1 } * context.InvertDrawingMatrix());
            Matrix deMove = Matrix.Move(-location) * Invert;
            p1 = p1 * deMove * vMatrix * Matrix.YRotation(Math.PI / 2);
            p2 = p2 * deMove * vMatrix * Matrix.YRotation(Math.PI / 2);

            double denum = p2.Z - p1.Z;
            double numX = -(p2.X - p1.X) * p2.Z;
            double numY = -(p2.Y - p1.Y) * p2.Z;
            Base3D.Point newPoint = new Base3D.Point { X = numX / denum + p2.X, Y = numY / denum + p2.Y };

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

        private void Move(object s, MouseEventArgs e)
        {
            if (!NowMoving(e))
                return;
            Matrix movingMatrix = Matrix.Ident();
            if (xMoving)
            {
                double dist = Distance(e, Matrix.Ident());
                movingMatrix = Matrix.Move(xDirection.vectorMult((dist - lastDistance)));
                location = location * movingMatrix;
                lastDistance = Distance(e, Matrix.Ident());
            }
            else
            if (yMoving)
            {
                double dist = Distance(e, Matrix.ZRotation(-Math.PI / 2));
                movingMatrix = Matrix.Move(yDirection.vectorMult((dist - lastDistance)));
                location = location * movingMatrix;
                lastDistance = Distance(e, Matrix.ZRotation(-Math.PI / 2));
            }
            else
            if (zMoving)
            {
                double dist = Distance(e, Matrix.YRotation(Math.PI / 2));
                movingMatrix = Matrix.Move(zDirection.vectorMult((dist - lastDistance)));
                location = location * movingMatrix;
                lastDistance = Distance(e, Matrix.YRotation(Math.PI / 2));
            }
            else
                return;
            xView.Apply(movingMatrix);
            yView.Apply(movingMatrix);
            zView.Apply(movingMatrix);
            Deformations = Deformations * movingMatrix;
            if (!OnlyGizmo)
                context.world.SelectedApply(movingMatrix);
            context.Redraw();
        }

        private void Scale(object s, MouseEventArgs e)
        {
            if (!NowMoving(e))
                return;
            Matrix movingMatrix = Matrix.Ident();
            if (xMoving)
            {
                double dist = Distance(e, Matrix.Ident());
                movingMatrix = Matrix.Move(-location) *
                    Matrix.Scale(xDirection, dist / lastDistance) *
                    Matrix.Move(location);
                lastDistance = Distance(e, Matrix.Ident());
            }
            else
            if (yMoving)
            {
                double dist = Distance(e, Matrix.ZRotation(-Math.PI / 2));
                movingMatrix = Matrix.Move(-location) *
                    Matrix.Scale(yDirection, dist / lastDistance) *
                    Matrix.Move(location);
                lastDistance = Distance(e, Matrix.ZRotation(-Math.PI / 2));
            }
            else
            if (zMoving)
            {
                double dist = Distance(e, Matrix.YRotation(Math.PI / 2));
                movingMatrix = Matrix.Move(-location) *
                    Matrix.Scale(zDirection, dist / lastDistance) *
                    Matrix.Move(location);
                lastDistance = Distance(e, Matrix.YRotation(Math.PI / 2));
            }
            else
                return;
            xView.Apply(movingMatrix);
            yView.Apply(movingMatrix);
            zView.Apply(movingMatrix);
            Deformations = Deformations * movingMatrix;
            if (!OnlyGizmo)
                context.world.SelectedApply(movingMatrix);
            context.Redraw();
        }

        private void Rotate(object s, MouseEventArgs e)
        {
            if (!NowMoving(e))
                return;

            Matrix movingMatrix = Matrix.Ident();
            Matrix rotateMatrix = Matrix.Ident();
            if (xMoving)
            {
                (var newPoint, double angle) = RotationPoint(e, Matrix.Ident());
                rotateMatrix = Matrix.Rotation(xDirection, angle);
                movingMatrix = Matrix.Move(-location) * rotateMatrix * Matrix.Move(location);
                Invert *= Matrix.Rotation(xDirection, -angle);

                (lastRotation, angle) = RotationPoint(e, Matrix.Ident());
            }
            else
            if (yMoving)
            {
                (var newPoint, double angle) = RotationPoint(e, Matrix.ZRotation(-Math.PI / 2));
                rotateMatrix = Matrix.Rotation(yDirection, angle);
                movingMatrix = Matrix.Move(-location) * rotateMatrix * Matrix.Move(location);
                Invert *= Matrix.Rotation(yDirection, -angle);

                (lastRotation, angle) = RotationPoint(e, Matrix.ZRotation(-Math.PI / 2));
            }
            else
            if (zMoving)
            {
                (var newPoint, double angle) = RotationPoint(e, Matrix.YRotation(Math.PI / 2));
                rotateMatrix = Matrix.Rotation(zDirection, angle);
                movingMatrix = Matrix.Move(-location) * rotateMatrix * Matrix.Move(location);
                Invert *= Matrix.Rotation(zDirection, -angle);

                (lastRotation, angle) = RotationPoint(e, Matrix.YRotation(Math.PI / 2));
            }
            else
                return;
            xView.Apply(movingMatrix);
            yView.Apply(movingMatrix);
            zView.Apply(movingMatrix);
            xDirection = xDirection * rotateMatrix;
            yDirection = yDirection * rotateMatrix;
            zDirection = zDirection * rotateMatrix;
            Deformations = Deformations * movingMatrix;
            if (!OnlyGizmo)
                context.world.SelectedApply(movingMatrix);
            context.Redraw();
        }

        private void MouseUp(object s, MouseEventArgs e)
        {
            xMoving = false;
            yMoving = false;
            zMoving = false;
        }

        public void Delete()
        {
            context.world.control.Remove(xView);
            context.world.control.Remove(yView);
            context.world.control.Remove(zView); 
            context.Redraw();

            context.pictureBox.MouseMove -= Move;
            context.pictureBox.MouseMove -= Scale;
            context.pictureBox.MouseMove -= Rotate;
            context.pictureBox.MouseUp -= MouseUp;
        }
    }
}
