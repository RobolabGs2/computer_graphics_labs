using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab05.LSystem
{
    struct Descriptor
    {
        public Point Point { get; set; }
        public string Rules { get; set; }
        public string Axiom { get; set; }
        public string Depth { get; set; }
        public string Color { get; set; }
        public string Width { get; set; }
        public string Scale { get; set; }
        public string Rotation { get; set; }
    }
}
