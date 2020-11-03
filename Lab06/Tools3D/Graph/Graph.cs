using Lab06.Base3D;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lab06.AuxTools3D;

namespace Lab06.Tools3D.Graph
{
    public class Graph : IToolPage
    {
        public Bitmap Image => Properties.Resources.Graph;

        public void Init(ToolTab tab, Context context)
        {
            Parser p = new Parser();
            var panel = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 7,
                Dock = DockStyle.Top,
                ForeColor = Constants.borderColore,
                AutoSize = true,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                Font = new Font("Consalas", 12)
            };

            var funcText = new TextBox { Dock = DockStyle.Fill, Text = "(x * x + sin(y) *  y) / 5"};
            panel.Controls.AddRange(new Control[] { new Label { Text = "Z = " }, funcText });
            var XStart = new TextBox { Dock = DockStyle.Fill, Text = "-5" };
            panel.Controls.AddRange(new Control[] { new Label { Text = "X от" }, XStart });
            var XEnd = new TextBox { Dock = DockStyle.Fill, Text = "5" };
            panel.Controls.AddRange(new Control[] { new Label { Text = "X до" }, XEnd });
            var XStep = new TextBox { Dock = DockStyle.Fill, Text = "1" };
            panel.Controls.AddRange(new Control[] { new Label { Text = "X Шаг" }, XStep });
            var YStart = new TextBox { Dock = DockStyle.Fill, Text = "-5" };
            panel.Controls.AddRange(new Control[] { new Label { Text = "Y от" }, YStart });
            var YEnd = new TextBox { Dock = DockStyle.Fill, Text = "5" };
            panel.Controls.AddRange(new Control[] { new Label { Text = "Y до" }, YEnd });
            var YStep = new TextBox { Dock = DockStyle.Fill, Text = "1" };
            panel.Controls.AddRange(new Control[] { new Label { Text = "Y Шаг" }, YStep });
            tab.Settings.Controls.Add(panel);

            tab.AddButton(Properties.Resources.Graph, false).ButtonClick +=
                b =>
                {
                    try
                    {
                        context.world.entities.Add(MakeGraph(p.Parse(funcText.Text),
                            double.Parse(XStart.Text),
                            double.Parse(XEnd.Text),
                            double.Parse(XStep.Text),
                            double.Parse(YStart.Text),
                            double.Parse(YEnd.Text),
                            double.Parse(YStep.Text)));
                        context.Redraw();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show($"Ошибка: \"{e.Message}\"", "Окошко-всплывашка", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                };

        }

        Polytope MakeGraph(Node node, double xStart, double xEnd, double xStep, double yStart, double yEnd, double yStep)
        {
            var result = new Polytope();
            int xDelta = 0;
            int yDelta = 0;
            for (double i = xStart; i <= xEnd; i += xStep)
            {
                xDelta += 1;
                yDelta = 0;
                for (double j = yStart; j <= yEnd; j += yStep)
                {
                    yDelta += 1;
                    result.Add(new Base3D.Point { X = i, Y = j, Z = node.Get(i, j) });
                    result.AddNormal(new Base3D.Point { X = -node.Dx().Get(i, j), Y = -node.Dy().Get(i, j), Z = 1 }.Normal());
                    result.AddNormal(new Base3D.Point { X = node.Dx().Get(i, j), Y = node.Dy().Get(i, j), Z = -1 }.Normal());
                }
            }

            for (int i = 0; i < xDelta - 1; ++i)
                for (int j = 0; j < yDelta - 1; ++j)
                {
                    result.Add(new Polygon(new int[]{
                        j + i * yDelta,
                        j + (i + 1) * yDelta,
                        (j + 1) + (i + 1) * yDelta,
                        (j + 1) + i * yDelta},
                        new int[]{
                        2 * (j + i * yDelta),
                        2 * (j + (i + 1) * yDelta),
                        2 * ((j + 1) + (i + 1) * yDelta),
                        2 * ((j + 1) + i * yDelta)}
                    ));

                    result.Add(new Polygon(new int[]{
                        j + i * yDelta,
                        (j + 1) + i * yDelta,
                        (j + 1) + (i + 1) * yDelta,
                        j + (i + 1) * yDelta},
                        new int[]{
                        2 * (j + i * yDelta) + 1,
                        2 * ((j + 1) + i * yDelta) + 1,
                        2 * ((j + 1) + (i + 1) * yDelta) + 1,
                        2 * (j + (i + 1) * yDelta) + 1}
                    ));
                }

            return result;
        }
    }

}
