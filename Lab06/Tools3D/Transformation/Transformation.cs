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

        private void RestartDirection()
        {
            xDirection = new Base3D.Point { X = 1 };
            yDirection = new Base3D.Point { Y = 1 };
            zDirection = new Base3D.Point { Z = 1 };
            location = context.SeletedPivot();
        }

        public void InitRotation()
        {
            RestartDirection();
            xView = Rotator(Color.DarkRed);
            yView = Rotator(Color.DarkBlue);
            zView = Rotator(Color.DarkGreen);

            xView.Apply(Matrix.Move(location));
            yView.Apply(Matrix.ZRotation(Math.PI / 2) * Matrix.Move(location));
            zView.Apply(Matrix.YRotation(-Math.PI / 2) * Matrix.Move(location));

            context.world.control.Add(xView);
            context.world.control.Add(yView);
            context.world.control.Add(zView);
            context.Redraw();

            Invert = Matrix.Ident();
            context.pictureBox.MouseMove += Rotate;
            context.pictureBox.MouseUp += MouseUp;
        }

        public void InitScale()
        {
            RestartDirection();
            xView = Scelator(Color.DarkRed);
            yView = Scelator(Color.DarkBlue);
            zView = Scelator(Color.DarkGreen);

            xView.Apply(Matrix.Move(location));
            yView.Apply(Matrix.ZRotation(Math.PI / 2) * Matrix.Move(location));
            zView.Apply(Matrix.YRotation(-Math.PI / 2) * Matrix.Move(location));

            context.world.control.Add(xView);
            context.world.control.Add(yView);
            context.world.control.Add(zView);
            context.Redraw();

            Invert = Matrix.Ident();
            context.pictureBox.MouseMove += Scale;
            context.pictureBox.MouseUp += MouseUp;
        }

        public void InitMove()
        {
            RestartDirection();
            xView = Arrow(Color.DarkRed);
            yView = Arrow(Color.DarkBlue);
            zView = Arrow(Color.DarkGreen);

            xView.Apply(Matrix.Move(location));
            yView.Apply(Matrix.ZRotation(Math.PI / 2) * Matrix.Move(location));
            zView.Apply(Matrix.YRotation(-Math.PI / 2) * Matrix.Move(location));

            context.world.control.Add(xView);
            context.world.control.Add(yView);
            context.world.control.Add(zView);
            context.Redraw();

            Invert = Matrix.Ident();
            context.pictureBox.MouseMove += Move;
            context.pictureBox.MouseUp += MouseUp;
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
            var triangle = Circle();
            triangle.Apply(Matrix.XRotation(Math.PI / 2));
            rotator.Add(triangle);

            Spline spline = new Spline();
            spline.Matreial = new SolidMaterial();
            spline.Add(new Base3D.Point { });
            spline.Add(new Base3D.Point { X = 2 });
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
            triangle.polygons.Add(new Polygon(new Base3D.Point[] { triangle.points[0], triangle.points[1], triangle.points[2] }));
            triangle.polygons.Add(new Polygon(new Base3D.Point[] { triangle.points[0], triangle.points[2], triangle.points[1] }));
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
            triangle.polygons.Add(new Polygon(new Base3D.Point[] { triangle.points[0], triangle.points[1], triangle.points[2], triangle.points[3] }));
            triangle.polygons.Add(new Polygon(new Base3D.Point[] { triangle.points[3], triangle.points[2], triangle.points[1], triangle.points[0] }));
            return triangle;
        }
        private Polytope Circle()
        {
            Polytope triangle = new Polytope();
            triangle.Matreial = new SolidMaterial();

            triangle.Add(new Base3D.Point { X = 2 });
            triangle.Add(new Base3D.Point { X = 3, Y = 0.5 });
            triangle.Add(new Base3D.Point { X = 3, Y = -0.5 });
            triangle.polygons.Add(new Polygon(new Base3D.Point[] { triangle.points[0], triangle.points[1], triangle.points[2] }));
            triangle.polygons.Add(new Polygon(new Base3D.Point[] { triangle.points[0], triangle.points[2], triangle.points[1] }));
            return triangle;
        }

        private double Distance(MouseEventArgs e, Matrix vMatrix)
        {
            Base3D.Point p0 = (new Base3D.Point { Y = e.X, Z = e.Y } * context.InvertDrawingMatrix()).FlattenT();
            Base3D.Point p1 = (new Base3D.Point { Y = e.X, Z = e.Y, X = 1 } * context.InvertDrawingMatrix()).FlattenT();
            Matrix deMove = Matrix.Move(-location);
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

        private bool NowMoving(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return false;
            if (xMoving || yMoving || zMoving)
                return true;

            if (context.ScreenPointInGroup(e.X, e.Y, xView))
            {
                lastDistance = Distance(e, Matrix.Ident());
                xMoving = true;
            }
            else if (context.ScreenPointInGroup(e.X, e.Y, yView))
            {
                lastDistance = Distance(e, Matrix.ZRotation(-Math.PI / 2));
                yMoving = true;
            }
            else if (context.ScreenPointInGroup(e.X, e.Y, zView))
            {
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
                location.Apply(movingMatrix);
                lastDistance = Distance(e, Matrix.Ident());
            }
            else
            if (yMoving)
            {
                double dist = Distance(e, Matrix.ZRotation(-Math.PI / 2));
                movingMatrix = Matrix.Move(yDirection.vectorMult((dist - lastDistance)));
                location.Apply(movingMatrix);
                lastDistance = Distance(e, Matrix.ZRotation(-Math.PI / 2));
            }
            else
            if (zMoving)
            {
                double dist = Distance(e, Matrix.YRotation(Math.PI / 2));
                movingMatrix = Matrix.Move(zDirection.vectorMult((dist - lastDistance)));
                location.Apply(movingMatrix);
                lastDistance = Distance(e, Matrix.YRotation(Math.PI / 2));
            }
            else
                return;
            xView.Apply(movingMatrix);
            yView.Apply(movingMatrix);
            zView.Apply(movingMatrix);
            if (!OnlyGizmo)
                context.world.SelectedApply(movingMatrix);
            context.Redraw();
        }

        private void Rotate(object s, MouseEventArgs e)
        {
            if (!NowMoving(e))
                return;
        }

        private void Scale(object s, MouseEventArgs e)
        {
            if (!NowMoving(e))
                return;
            Matrix movingMatrix = Matrix.Ident();
            if (xMoving)
            {
                double dist = Distance(e, Matrix.Ident());
                movingMatrix = Matrix.Move(-location) * Matrix.Scale(
                    new Base3D.Point { X = 1, Y = 1, Z = 1}.vectorAdd(
                    xDirection.vectorMult((dist - lastDistance)))) *
                    Matrix.Move(location);
                location.Apply(movingMatrix);
                lastDistance = Distance(e, Matrix.Ident());
            }
            else
            if (yMoving)
            {
                double dist = Distance(e, Matrix.ZRotation(-Math.PI / 2));
                movingMatrix = Matrix.Move(-location) * Matrix.Scale(
                    new Base3D.Point { X = 1, Y = 1, Z = 1 }.vectorAdd(
                    yDirection.vectorMult((dist - lastDistance)))) *
                    Matrix.Move(location);
                location.Apply(movingMatrix);
                lastDistance = Distance(e, Matrix.ZRotation(-Math.PI / 2));
            }
            else
            if (zMoving)
            {
                double dist = Distance(e, Matrix.YRotation(Math.PI / 2));
                movingMatrix = Matrix.Move(-location) * Matrix.Scale(
                    new Base3D.Point { X = 1, Y = 1, Z = 1 }.vectorAdd(
                    zDirection.vectorMult((dist - lastDistance)))) * 
                    Matrix.Move(location);
                location.Apply(movingMatrix);
                lastDistance = Distance(e, Matrix.YRotation(Math.PI / 2));
            }
            else
                return;
            xView.Apply(movingMatrix);
            yView.Apply(movingMatrix);
            zView.Apply(movingMatrix);
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
