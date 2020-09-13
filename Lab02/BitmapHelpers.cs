using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab02
{
    public static class BitmapHelpers
    {
        public static Bitmap Fit(this Bitmap bitmap, int width, int height)
        {
            var scale = Math.Max(bitmap.Width * 1.0 / width, bitmap.Height * 1.0 / height);
            var result = new Bitmap(width, height);
            Graphics.FromImage(result).DrawImage(bitmap,
                new Rectangle(0, 0, (int) (bitmap.Width / scale), (int) (bitmap.Height / scale)));
            return result;
        }
        public static Bitmap Fit(this Bitmap bitmap, Size size)
        {
            return bitmap.Fit(size.Width, size.Height);
        }
    }
}