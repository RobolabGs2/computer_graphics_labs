using Lab06.Base3D;
using Lab06.Graph3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lab06.AuxTools3D;
using System.Drawing.Printing;

namespace Lab06.Tools3D.Render
{
    class Render : IToolPage
    {
        TextBox ambient;
        TextBox diffuse;
        TextBox specular;
        TextBox power;
        CheckBox smoothing;
        Context context;

        public Bitmap Image => Properties.Resources.Render;

        public void Init(ToolTab tab, Context context)
        {
            this.context = context;
            
            Parser p = new Parser();
            var panelGraph = new Panel
            {
                Dock = DockStyle.Fill,
                Visible = false
            };
            var panelTable = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 7,
                Dock = DockStyle.Top,
                ForeColor = Constants.borderColore,
                AutoSize = true,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                Font = new Font("Consalas", 12),
                Visible = false
            };
            var floatinHor = new CheckBox
            { Text = "Floating horizon method", ForeColor = Constants.textColore, Font = Constants.font, Dock = DockStyle.Top, AutoSize = true };

            var funcText = new TextBox { Dock = DockStyle.Fill, Text = "(x * x + sin(y) *  y) / 5" };
            panelTable.Controls.AddRange(new Control[] { new Label { Text = "Z = " }, funcText });
            var XStart = new TextBox { Dock = DockStyle.Fill, Text = "-5" };
            panelTable.Controls.AddRange(new Control[] { new Label { Text = "X от" }, XStart });
            var XEnd = new TextBox { Dock = DockStyle.Fill, Text = "5" };
            panelTable.Controls.AddRange(new Control[] { new Label { Text = "X до" }, XEnd });
            var XStep = new TextBox { Dock = DockStyle.Fill, Text = "0.5" };
            panelTable.Controls.AddRange(new Control[] { new Label { Text = "X Шаг" }, XStep });
            var YStart = new TextBox { Dock = DockStyle.Fill, Text = "-5" };
            panelTable.Controls.AddRange(new Control[] { new Label { Text = "Y от" }, YStart });
            var YEnd = new TextBox { Dock = DockStyle.Fill, Text = "5" };
            panelTable.Controls.AddRange(new Control[] { new Label { Text = "Y до" }, YEnd });
           
            var apply = new Button
            {
                Text = "Apply",
                ForeColor = Constants.textColore,
                Font = Constants.font,
                Dock = DockStyle.Bottom,
                AutoSize = true,
                Visible = false
            };
            apply.Click += (s, b) =>
            {
                try
                {
                   context.drawing = new FloatingHorizont { node = p.Parse(funcText.Text),
                       xStart = double.Parse(XStart.Text),
                       xEnd = double.Parse(XEnd.Text),
                       xStep =  double.Parse(XStep.Text),
                       yStart = double.Parse(YStart.Text),
                       yEnd = double.Parse(YEnd.Text),
                       context = context};
                    context.Redraw();
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Ошибка: \"{e.Message}\"", "Окошко-всплывашка", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }; 

            floatinHor.CheckStateChanged += (sender, args) => { panelTable.Visible = floatinHor.Checked; apply.Visible = floatinHor.Checked; };
            tab.Settings.Controls.Add(panelGraph);
            panelGraph.Controls.Add(panelTable);
            panelGraph.Controls.Add(floatinHor);
            panelGraph.Controls.Add(apply);

            tab.AddButton(Properties.Resources.Skeleton, false).ButtonClick += b =>
            {
                panelGraph.Visible = false;
                context.drawing = new Skeleton(context, false);
                context.Redraw();
            };

            var skeleton = tab.AddButton(Properties.Resources.Cube, false);
            skeleton.ButtonClick += b =>
            {
                context.drawing = new Skeleton(context, true);
                context.Redraw();
                panelGraph.Visible = true;
                floatinHor.Checked = false;
            };

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Visible = false
            };

            tab.Settings.Controls.Add(panel);

            var textPanel = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 4,
                Dock = DockStyle.Top,
                ForeColor = Constants.borderColore,
                AutoSize = true,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                Font = new Font("Consalas", 12)
            };

            smoothing = new CheckBox
            {
                Text = "smoothing",
                Dock = DockStyle.Top,
                ForeColor = Constants.borderColore,
                Font = new Font("Consalas", 12),
                Checked = true
            };
            panel.Controls.Add(smoothing);

            ambient = new TextBox { Dock = DockStyle.Fill, Text = "10" };
            textPanel.Controls.AddRange(new Control[] { new Label { Text = "Ambient(%)" }, ambient });
            diffuse = new TextBox { Dock = DockStyle.Fill, Text = "100" };
            textPanel.Controls.AddRange(new Control[] { new Label { Text = "Diffuse(%)" }, diffuse });
            specular = new TextBox { Dock = DockStyle.Fill, Text = "100" };
            textPanel.Controls.AddRange(new Control[] { new Label { Text = "Specular(%)" }, specular });
            power = new TextBox { Dock = DockStyle.Fill, Text = "1000" };
            textPanel.Controls.AddRange(new Control[] { new Label { Text = "Power" }, power });
            panel.Controls.Add(textPanel);

            var changeRender = new Button
            {
                Text = "Apply",
                ForeColor = Constants.textColore,
                Font = Constants.font,
                Dock = DockStyle.Top,
                AutoSize = true
            };

            panel.Controls.Add(changeRender);
            changeRender.Click += (s, b) => SetDrawing();

            var renderButton = tab.AddButton(Properties.Resources.Render, true);

            renderButton.ButtonClick += b =>
            {
                panelGraph.Visible = false;
                floatinHor.Checked = false;
                panel.Visible = true;
                context.drawing = new ZBuffer(context);
                SetDrawing();
            };

            renderButton.ButtonDisable += b =>
            {
                panel.Visible = false;
            };

        }

        void SetDrawing()
        {
            try
            {
                if (context.drawing is ZBuffer zBuffer)
                {
                    zBuffer.phongAmbient = double.Parse(ambient.Text) / 100;
                    zBuffer.phongDiffuse = double.Parse(diffuse.Text) / 100;
                    zBuffer.phongSpecular = double.Parse(specular.Text) / 100;
                    zBuffer.phongPower = double.Parse(power.Text);
                    zBuffer.smoothing = smoothing.Checked;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ошибка: \"{e.Message}\"", "Окошко-всплывашка", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            context.Redraw();
        }

        
    }
}
