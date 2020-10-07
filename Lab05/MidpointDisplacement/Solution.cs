using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab05.MidpointDisplacement
{
    public class Solution: AbstractSolution
    {
        private readonly PictureBox canvas = new PictureBox
        {
            Dock = DockStyle.Left,
            BackColor = Color.Beige,
            BorderStyle = BorderStyle.FixedSingle,
        };

        private readonly Control panel = new Panel()
        {
            Dock = DockStyle.Right,
            Width = 200,
            Controls =
            {
                new Label {Text = "Test", Dock = DockStyle.Bottom, TextAlign = ContentAlignment.MiddleCenter},
                new Button {Text = "TestButton", Dock = DockStyle.Bottom, Height = 32},
                new Button {Text = "TestButton2", Dock = DockStyle.Bottom, Height = 32},
            }
        };

        public Solution() : base("Midpoint displacement")
        {
        }

        public override Control[] Controls => new [] {canvas, panel};
        public override Size Size
        {
            set => canvas.Width = value.Width - panel.Width;
        }
    }
}
