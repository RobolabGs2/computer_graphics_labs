using System;
using System.Collections.Generic;
using System.Drawing;

namespace Lab02
{
    static class Grayscale
    {
        public static unsafe ISolution MakeSolution()
        {
            return new MonochromaticSolution("Оттенки серого",
                new List<MonochromaticSolution.Monochromatic>
                {
                    new MonochromaticSolution.Monochromatic("sRGB",
                        (format, src, dst) => dst[0] = dst[1] = dst[2] = sRGB(src[0], src[1], src[2]),
                        Color.DarkOrange),
                    new MonochromaticSolution.Monochromatic("NTSC RGB",
                        (format, src, dst) => dst[0] = dst[1] = dst[2] = NTSC_RGB(src[0], src[1], src[2]),
                        Color.YellowGreen),
                    new MonochromaticSolution.Monochromatic("Разность", (format, src, dst) =>
                    {
                        var (r, g, b) = (src[0], src[1], src[2]);
                        return dst[0] = dst[1] = dst[2] = (byte) (Math.Abs(sRGB(r, g, b) - NTSC_RGB(r, g, b)));
                    }, Color.BlueViolet)
                });
        }

        static byte NTSC_RGB(byte R, byte G, byte B)
        {
            return (byte) (0.3 * R + 0.59 * G + 0.11 * B);
        }

        static byte sRGB(byte R, byte G, byte B)
        {
            return (byte) (0.21 * R + 0.72 * G + 0.07 * B);
        }
    }
}