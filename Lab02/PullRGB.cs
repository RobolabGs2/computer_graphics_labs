using System;
using System.Collections.Generic;
using System.Drawing;

namespace Lab02
{
    class PullRGB
    {
        public static unsafe ISolution MakeSolution()
        {
            return new MonochromaticSolution("Выделить RGB",
                new List<MonochromaticSolution.Monochromatic>
                {
                    new MonochromaticSolution.Monochromatic("Red",
                        (format, src, dst) => dst[2] = src[2],
                        Color.Red),
                    new MonochromaticSolution.Monochromatic("Green",
                        (format, src, dst) => dst[1] = src[1],
                        Color.Green),
                    new MonochromaticSolution.Monochromatic("Blue",
                        (format, src, dst) => dst[0] = src[0],
                        Color.Blue)
                });
        }
    }
}
