﻿using Lab06.Base3D;
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
        CheckBox smoothing;
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

            //var changeRender = new Button
            //{
            //    Text = "Apply",
            //    ForeColor = Constants.textColore,
            //    Font = Constants.font,
            //    Dock = DockStyle.Top,
            //    AutoSize = true
            //};

            //panel.Controls.Add(changeRender);
            //changeRender.Click += (s, b) => SetBlinnDrawing();

            var phongButton = tab.AddButton(Properties.Resources.Phong, true);

            phongButton.ButtonClick += b =>
            {
                panel.Visible = true;
                context.drawing = new ZBuffer(context);
                SetPhongDrawing();
            };

            phongButton.ButtonDisable += b =>
            {
                panel.Visible = false;
            };

            var blinnButton = tab.AddButton(Properties.Resources.Blinn, true);

            blinnButton.ButtonClick += b =>
            {
                panel.Visible = true;
                context.drawing = new ZBuffer(context);
                SetBlinnDrawing();
            };

            blinnButton.ButtonDisable += b =>
            {
                panel.Visible = false;
            };

            var wardButton = tab.AddButton(Properties.Resources.Ward, true);

            wardButton.ButtonClick += b =>
            {
                panel.Visible = true;
                context.drawing = new ZBuffer(context);
                SetWardDrawing();
            };

            wardButton.ButtonDisable += b =>
            {
                panel.Visible = false;
            };

            var orenNayarButton = tab.AddButton(Properties.Resources.OrenNayar, true);

            orenNayarButton.ButtonClick += b =>
            {
                panel.Visible = true;
                context.drawing = new ZBuffer(context);
                SetOrenNayarDrawing();
            };

            orenNayarButton.ButtonDisable += b =>
            {
                panel.Visible = false;
            };

            var cookTorranceButton = tab.AddButton(Properties.Resources.CookTorrance, true);

            cookTorranceButton.ButtonClick += b =>
            {
                panel.Visible = true;
                context.drawing = new ZBuffer(context);
                SetCookTorranceDrawing();
            };

            cookTorranceButton.ButtonDisable += b =>
            {
                panel.Visible = false;
            };
        }

        void SetPhongDrawing()
        {
            try
            {
                if (context.drawing is ZBuffer zBuffer)
                {
                    zBuffer.pixelDrawing = new ZBuffer.PhongDrawing
                    {
                        ambient = double.Parse(ambient.Text) / 100,
                        diffuse = double.Parse(diffuse.Text) / 100,
                        specular = double.Parse(specular.Text) / 100,
                        power = double.Parse(power.Text),
                        smoothing = smoothing.Checked
                    };
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ошибка: \"{e.Message}\"", "Окошко-всплывашка", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            context.Redraw();
        }
        void SetBlinnDrawing()
        {
            try
            {
                if (context.drawing is ZBuffer zBuffer)
                {
                    zBuffer.pixelDrawing = new ZBuffer.BlinnDrawing
                    {
                        ambient = double.Parse(ambient.Text) / 100,
                        diffuse = double.Parse(diffuse.Text) / 100,
                        specular = double.Parse(specular.Text) / 100,
                        power = double.Parse(power.Text),
                        smoothing = smoothing.Checked
                    };
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ошибка: \"{e.Message}\"", "Окошко-всплывашка", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            context.Redraw();
        }
        void SetWardDrawing()
        {
            try
            {
                if (context.drawing is ZBuffer zBuffer)
                {
                    zBuffer.pixelDrawing = new ZBuffer.WardDrawing
                    {
                        ambient = double.Parse(ambient.Text) / 100,
                        diffuse = double.Parse(diffuse.Text) / 100,
                        specular = double.Parse(specular.Text) / 100,
                        power = double.Parse(power.Text),
                        smoothing = smoothing.Checked
                    };
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ошибка: \"{e.Message}\"", "Окошко-всплывашка", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            context.Redraw();
        }
        void SetOrenNayarDrawing()
        {
            try
            {
                if (context.drawing is ZBuffer zBuffer)
                {
                    zBuffer.pixelDrawing = new ZBuffer.OrenNayarDrawing
                    {
                        ambient = double.Parse(ambient.Text) / 100,
                        diffuse = double.Parse(diffuse.Text) / 100,
                        specular = double.Parse(specular.Text) / 100,
                        power = double.Parse(power.Text),
                        smoothing = smoothing.Checked
                    };
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ошибка: \"{e.Message}\"", "Окошко-всплывашка", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            context.Redraw();
        }
        void SetCookTorranceDrawing()
        {
            try
            {
                if (context.drawing is ZBuffer zBuffer)
                {
                    zBuffer.pixelDrawing = new ZBuffer.CookTorranceDrawing
                    {
                        ambient = double.Parse(ambient.Text) / 100,
                        diffuse = double.Parse(diffuse.Text) / 100,
                        specular = double.Parse(specular.Text) / 100,
                        power = double.Parse(power.Text),
                        smoothing = smoothing.Checked
                    };
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
