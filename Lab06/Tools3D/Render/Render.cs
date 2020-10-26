using Lab06.Base3D;
using Lab06.Graph3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab06.Tools3D.Render
{
    class Render : IToolPage
    {
        TextBox ambient;
        TextBox diffuse;
        TextBox specular;
        TextBox power;
        Context context;

        public Bitmap Image => Properties.Resources.Render;

        public void Init(ToolTab tab, Context context)
        {
            this.context = context;
            tab.AddButton(Properties.Resources.Skeleton, false).ButtonClick += b =>
               {
                   context.drawing = new Skeleton(context, false);
                   context.Redraw();
               };
            tab.AddButton(Properties.Resources.Cube, false).ButtonClick += b =>
            {
                context.drawing = new Skeleton(context, true);
                context.Redraw();
            };

            var panel = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 4,
                Dock = DockStyle.Top,
                ForeColor = Constants.borderColore,
                AutoSize = true,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                Font = new Font("Consalas", 12),
                Visible = false
            };

            ambient = new TextBox { Dock = DockStyle.Fill, Text = "10" };
            panel.Controls.AddRange(new Control[] { new Label { Text = "Ambient(%)" }, ambient });
            diffuse = new TextBox { Dock = DockStyle.Fill, Text = "100" };
            panel.Controls.AddRange(new Control[] { new Label { Text = "Diffuse(%)" }, diffuse });
            specular = new TextBox { Dock = DockStyle.Fill, Text = "100" };
            panel.Controls.AddRange(new Control[] { new Label { Text = "Specular(%)" }, specular });
            power = new TextBox { Dock = DockStyle.Fill, Text = "1000" };
            panel.Controls.AddRange(new Control[] { new Label { Text = "Power" }, power });
            tab.Settings.Controls.Add(panel);

            var changeRender = new Button
            {
                Text = "Apply",
                ForeColor = Constants.textColore,
                Font = Constants.font,
                Dock = DockStyle.Top,
                AutoSize = true,
                Visible = false
            };

            tab.Settings.Controls.Add(changeRender);
            changeRender.Click += (s, b) => SetDrawing();

            var renderButton = tab.AddButton(Properties.Resources.Render, true);

            renderButton.ButtonClick += b =>
            {
                panel.Visible = true;
                changeRender.Visible = true;
                context.drawing = new ZBuffer(context);
                SetDrawing();
            };

            renderButton.ButtonDisable += b =>
            {
                panel.Visible = false;
                changeRender.Visible = false;
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
