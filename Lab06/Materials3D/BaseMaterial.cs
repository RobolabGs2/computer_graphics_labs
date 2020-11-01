using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab06.Materials3D
{
    public abstract class BaseMaterial
    {
        public virtual Color Color { get; } = Constants.borderColore;

        public virtual Color this[(double X, double Y) p] => Color;
    }
}
