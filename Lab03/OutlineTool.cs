using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Lab03
{
    static class OutlineSearcher
    {
        private enum Direction
        {
            Down,
            DownRight,
            Right,
            UpRight,
            Up,
            UpLeft,
            Left,
            DownLeft
        }

        private static readonly Size[] Directions =
        {
            new Size(0, +1),
            new Size(+1, +1),
            new Size(+1, 0),
            new Size(+1, -1),
            new Size(0, -1),
            new Size(-1, -1),
            new Size(-1, 0),
            new Size(-1, +1),
        };

        private static Point Shift(this Direction direction, Point origin)
        {
            return origin + Directions[(int) direction];
        }

        private static Direction Clockwise90Degree(this Direction direction)
        {
            var next = direction - 2;
            return next < Direction.Down ? next + Directions.Length : next;
        }

        private static Direction Inc(this Direction direction)
        {
            return direction == Direction.DownLeft ? Direction.Down : direction + 1;
        }

        private static Direction Reverse(this Direction direction)
        {
            return direction + 4 > Direction.DownLeft ? direction - 4 : direction + 4;
        }

        private static IEnumerable<(Direction, Point)> Neighbors(Point p, Direction start)
        {
            yield return (start, start.Shift(p));
            for (var i = start.Inc(); i != start; i = i.Inc())
            {
                yield return (i, i.Shift(p));
            }
        }

        private static (Direction, Point?) NextPoint(Color color, Point p, FastBitmap bitmap, Direction direction)
        {
            foreach (var (shift, nextPoint) in Neighbors(p, direction))
            {
                if (nextPoint.X < 0 || nextPoint.X >= bitmap.Width || nextPoint.Y < 0 || nextPoint.Y >= bitmap.Height)
                {
                    continue;
                }

                if (bitmap.GetPixel(nextPoint.X, nextPoint.Y) == color)
                {
                    return (shift, nextPoint);
                }
            }

            return (direction, null);
        }

        public class IncompleteOutlineException : Exception
        {
            public readonly LinkedList<Point> Outline;

            public IncompleteOutlineException(LinkedList<Point> outline) : base("Incomplete outline")
            {
                Outline = outline;
            }
        }

        public static LinkedList<Point> Outline(Point start, FastBitmap bitmap)
        {
            var outline = new LinkedList<Point>();
            var currentPoint = outline.AddLast(start);
            var color = bitmap.GetPixel(start.X, start.Y);
            var direction = Direction.Down;
            while (true)
            {
                Point? nextPoint;
                (direction, nextPoint) = NextPoint(color, currentPoint.Value, bitmap, direction.Clockwise90Degree());
                if (!nextPoint.HasValue)
                {
                    throw new IncompleteOutlineException(outline);
                }

                if (nextPoint == start)
                {
                    break;
                }

                currentPoint = outline.AddLast(nextPoint.Value);
            }

            return outline;
        }

        public static Point? StartOutline(int x, int y, FastBitmap bitmap)
        {
            for (var innerColor = bitmap.GetPixel(x, y); x != bitmap.Width; ++x)
            {
                if (bitmap.GetPixel(x, y) != innerColor)
                {
                    return new Point(x, y);
                }
            }

            return null;
        }
    }

    internal class OutlineTool : IDrawingTool
    {
        public string Name => "Search outline";
        private Color _color;

        public Color Color
        {
            set => _color = value;
        }

        private TextBox _logs;

        public void Init(Control container)
        {
            _logs = new TextBox
            {
                Height = container.Height - 4, Width = container.Width - 8, ScrollBars = ScrollBars.Vertical,
                Multiline = true, Font = new Font("Consolas", 8),
            };
            container.Controls.Add(_logs);
        }

        public void MouseDown(int x, int y, FastBitmap bitmap)
        {
            var start = OutlineSearcher.StartOutline(x, y, bitmap);
            if (!start.HasValue)
            {
                _logs.Text = "Граница за рамками битмапа";
                return;
            }

            try
            {
                var outline = OutlineSearcher.Outline(start.Value, bitmap);
                var logsBuilder = new StringBuilder();
                logsBuilder.AppendLine($"Точек в границе: {outline.Count}");
                logsBuilder.AppendLine($"{"X",6}|{"Y",6}");
                foreach (var point in outline)
                {
                    logsBuilder.AppendLine($"{point.X,6}|{point.Y,6}");
                    bitmap.SetPixel(point.X, point.Y, _color);
                }

                _logs.Text = logsBuilder.ToString();
            }
            catch (OutlineSearcher.IncompleteOutlineException e)
            {
                _logs.Text = "Граница не замкнута";
                foreach (var point in e.Outline)
                {
                    bitmap.SetPixel(point.X, point.Y, Color.OrangeRed);
                }
            }
        }

        public void MouseUp(int x, int y, FastBitmap bitmap)
        {
        }

        public void MouseMove(int x, int y, FastBitmap bitmap)
        {
        }

        public void Start(FastBitmap bitmap)
        {
            _logs.Clear();
        }
    }
}