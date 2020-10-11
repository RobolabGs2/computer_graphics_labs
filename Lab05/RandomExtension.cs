using System;
using System.Drawing;

namespace Lab05
{
    public static class RandomExtension
    {
        public static int Sign(this Random random)
        {
            return random.Next() % 2 == 0 ? -1 : 1;
        }

        public static bool Try(this Random random, double probability)
        {
            return probability > random.NextDouble();
        }

        public static float NextFloat(this Random random)
        {
            return (float) random.NextDouble();
        }

        public static float Next(this Random random, float max)
        {
            return (float)random.NextDouble()*max;
        }

        public static PointF Shift(this Random random, PointF p, float max)
        {
            return new PointF(
                (p.X + random.NextFloat() * max * random.Sign()),
                (p.Y + random.NextFloat() * max * random.Sign()));
        }
    }
}