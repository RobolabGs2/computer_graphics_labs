using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace Lab05.LSystem
{
    class State
    {
        Stack<(Point, double)> stack = new Stack<(Point, double)>();
        Point point;
        double direction;
        Pen pen = new Pen(Color.Black);
        double rotateError = 0;
        double scaleError = 0;

        public State(Point start)
        {
            point = start;
        }

        public void SetLine(Color color, int width)
        {
            pen.Dispose();
            pen = new Pen(color, width); 
            pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
        }

        public void Push()
        {
            stack.Push((point, direction));
        }

        public void Pop()
        {
            (point, direction) = stack.Pop();
        }

        public void Move(Graphics g, double distance)
        {
            distance = distance / scaleError;
            double errorDirection = direction + rotateError;
            Point to = point + distance * new Point { X = Math.Cos(errorDirection), Y = Math.Sin(errorDirection) };
            g.DrawLine(pen, point.ToPointF, to.ToPointF);
            point = to;
        }

        public void Rotate(double angle)
        {
            direction += angle / 180 * Math.PI;
        }

        public void SetRotateError(double error)
        {
            rotateError = error / 180 * Math.PI;
        }
        public void SetScaleError(double error)
        {
            scaleError = error;
        }
    }
}
