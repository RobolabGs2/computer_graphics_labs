using Lab06.Base3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab06.Tools3D.Matrixes
{
    class Matrixes : IToolPage
    {
        Context context;
        public Bitmap Image => Properties.Resources.Matrix;

        public void Init(ToolTab tab, Context context)
        {
            this.context = context;
            tab.Settings.Controls.AddRange(new List<(string Name, double DownAngle, double LeftAngle)>
            {
                ("XY", Math.PI / 2, 0),
                ("XZ", 0, Math.PI / 2),
                ("YZ", 0, 0),
                ("Isometric", 0.6154797087, Math.PI+Math.PI / 4),
            }.Select(tuple =>
            {
                var (name, downAngle, leftAngle) = tuple;
                var button = new Button
                {
                    Text = name,
                    AutoSize = true,
                    Dock = DockStyle.Bottom,
                    ForeColor = Constants.textColore,
                    Font = Constants.font,
                };
                button.Click += (sender, args) =>
                {
                    context.camera.downAngle = downAngle;
                    context.camera.leftAngle = leftAngle;
                    context.Redraw();
                };
                return (Control)button;
            }).ToArray());
            tab.AddButton(Properties.Resources.Parallel, false).Click +=
                    (o, s) =>
                    {
                        context.camera.interval = 1e10;
                        context.Redraw();
                    };
            tab.AddButton(Properties.Resources.Projection, false).Click +=
                    (o, s) =>
                    {
                        context.camera.interval = 10;
                        context.Redraw();
                    };
        }
    }
}
