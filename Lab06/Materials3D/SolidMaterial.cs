using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab06.Materials3D
{
    public class SolidMaterial: BaseMaterial
    {
        public override Color Color { get; } = Constants.textColore;
     
        public SolidMaterial()
        {
        }

        public SolidMaterial(Color color)
        {
            this.Color = color;
        }
    }
}
