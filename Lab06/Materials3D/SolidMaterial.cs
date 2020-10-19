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
        public override Color Color { get; }
     
        public SolidMaterial()
        {
            Color = base.Color;
        }

        public SolidMaterial(Color color)
        {
            this.Color = color;
        }
    }
}
