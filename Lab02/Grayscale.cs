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
                        (format, src, dst) =>
                        {
                            var gray = sRGB(src[ColorChannel.R], src[ColorChannel.G], src[ColorChannel.B]);
                            dst[0] = dst[1] = dst[2] = gray;
                            return gray;
                        },
                        Color.DarkOrange),
                    new MonochromaticSolution.Monochromatic("NTSC RGB",
                        (format, src, dst) =>
                        {
                            var gray = NTSC_RGB(src[ColorChannel.R], src[ColorChannel.G], src[ColorChannel.B]);
                            dst[0] = dst[1] = dst[2] = gray;
                            return gray;
                        },
                        Color.YellowGreen),
                    new MonochromaticSolution.Monochromatic("Разность", (format, src, dst) =>
                    {
                        var (r, g, b) = (src[ColorChannel.R], src[ColorChannel.G], src[ColorChannel.B]);
                        var diff = (byte) (Math.Abs(sRGB(r, g, b) - NTSC_RGB(r, g, b)));
                        dst[0] = dst[1] = dst[2] = (byte)(255 - diff);
                        return diff;
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