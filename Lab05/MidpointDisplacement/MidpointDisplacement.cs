using System;
using System.Collections.Generic;
using System.Drawing;

namespace Lab05.MidpointDisplacement
{
    public class Settings
    {
        public const string LabelH1 = "Высота левой точки";
        public const string LabelH2 = "Высота правой точки";
        public const string LabelH = "Общая высота (h)";
        [UI(LabelH)] public int Height = 1000;
        [UI("Общая длина (l)")] public int Length = 500;
        [UI("Зерно генерации")] public int RandomSeed = 0;
        [UI("Шероховатость (Roughness)")] public float R = 0.5F;
        [UI(LabelH1)] public int H1 = 500;
        [UI(LabelH2)] public int H2 = 500;
    }

    public class MidpointDisplacement
    {
        private readonly Random rnd;
        private readonly LinkedList<PointF> points;
        private readonly float R;

        public MidpointDisplacement(Settings settings)
        {
            rnd = new Random(settings.RandomSeed);
            points = new LinkedList<PointF>(
                new[]
                {
                    new PointF(0, settings.H1),
                    new PointF(settings.Length, settings.H2),
                });
            R = settings.R;
        }

        public void Iteration()
        {
            var start = points.First;
            while (start != points.Last)
            {
                var finish = start.Next;
                var l = start.Value.DistanceTo(finish.Value) * R;
                points.AddAfter(start,
                    new PointF((start.Value.X + finish.Value.X) / 2,
                        ((start.Value.Y + finish.Value.Y) / 2 + rnd.Next(l) * rnd.Sign())));
                start = finish;
            }
        }

        public void Draw(Graphics g, Pen pen)
        {
            for (var start = points.First; start != points.Last; start = start.Next)
            {
                g.DrawLine(pen, start.Value.X, start.Value.Y, start.Next.Value.X, start.Next.Value.Y);
            }
        }
    }
}