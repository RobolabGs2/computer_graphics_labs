using Lab06.Graph3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab06.Materials3D
{
    class TextureMaterial: BaseMaterial
    {
        CSharpImageLibrary.MipMap texture;
        int step;
        
        public TextureMaterial(CSharpImageLibrary.ImageEngineImage bitmap)
        {
            texture = bitmap.MipMaps[0];
            step = (texture.Pixels.Length) / (texture.Width * texture.Height);
        }

        public override Color this[(double X, double Y) p] =>
            getPixrl(texture.Pixels, p.X, p.Y);



        public Color getPixrl(byte[] pixel, double dx, double dy)
        {
            var x = (int)(dx * texture.Width);
            var y = (int)((1 - dy) * texture.Height);
            int idx = step * (y * texture.Width + x);
            return Color.FromArgb(pixel[idx + ColorChannel.R], pixel[idx + ColorChannel.G], pixel[idx + ColorChannel.B]);
            
        }
    }
}
