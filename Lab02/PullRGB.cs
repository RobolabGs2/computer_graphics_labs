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
                    new MonochromaticSolution.Monochromatic("R",
                        (format, src, dst) => dst[ColorChannel.R] = src[ColorChannel.R], Color.Red),
                    new MonochromaticSolution.Monochromatic("G",
                        (format, src, dst) => dst[ColorChannel.G] = src[ColorChannel.G], Color.Green),
                    new MonochromaticSolution.Monochromatic("B",
                        (format, src, dst) => dst[ColorChannel.B] = src[ColorChannel.B], Color.Blue)
                });
        }
    }
}
