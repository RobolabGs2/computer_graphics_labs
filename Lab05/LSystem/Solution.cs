using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab05.LSystem
{
    class Solution : AbstractSolution
    {
        private readonly PictureBox canvas = new PictureBox
        {
            Dock = DockStyle.Left,
            BackColor = Color.Beige,
            BorderStyle = BorderStyle.FixedSingle,
        };

        private readonly Control panel;
        RichTextBox rulesText;
        TextBox startText;
        TextBox depthText;
        TextBox colorText;
        TextBox widthText;
        TextBox scaleText;
        TextBox rotationText;
        Point   point;

        public Solution() : base("L - системы")
        {
            panel = new FlowLayoutPanel{ Dock = DockStyle.Right, Width = 300};

            startText = CreateTextBox("аксиома:", "A");
            depthText = CreateTextBox("глубина:", "5");
            colorText = CreateTextBox("цвет:", "black");
            widthText = CreateTextBox("Толщина:", "2");
            scaleText = CreateTextBox("Делитель:", "1");
            rotationText = CreateTextBox("Поворот:", "60");

            rulesText = new RichTextBox
            {
                Text = "A => B-A-B\r\nB => A+B+A\r\n\r\nA = move 10\r\nB = move 10\r\n+ = rotate 60\r\n- = rotate -60",
                Font  = new Font("Consolas", 14),
                Width = panel.Width - 6
            };

            Button startButton = new Button
            {
                Text = "Нарисовать",
                Dock = DockStyle.Bottom,
                Height = 32,
                Width = panel.Width - 6
            };

            Button saveButton = new Button
            {
                Text = "Сохранить",
                Dock = DockStyle.Bottom,
                Height = 32,
                Width = panel.Width/2 - 6
            };

            saveButton.Click += (s, e) => Save(); 

            Button loadButton = new Button
            {
                Text = "Загрузить",
                Dock = DockStyle.Bottom,
                Height = 32,
                Width = panel.Width/2 - 6
            };

            loadButton.Click += (s, e) => Load();

            canvas.MouseClick += (s, e) => { point = new Point { X = e.X, Y = e.Y }; Start(); };
            startButton.Click += (s, e) => { Start(); };
            panel.Controls.Add(rulesText);
            panel.Controls.Add(startButton);
            panel.Controls.Add(saveButton);
            panel.Controls.Add(loadButton);
        }

        private TextBox CreateTextBox(string description, string defaultText)
        {
            Label label = new Label { Text = description, Width = 60 };

            TextBox result = new TextBox
            {
                Text = defaultText,
                Width = panel.Width - label.Width - 12,
                Font = new Font("Consolas", 14),
            };
            panel.Controls.Add(label);
            panel.Controls.Add(result);
            return result;
        }

        private void Start()
        {

            Turtle turtle = Parser.Parse(rulesText.Text.Split('\n'));
            Graphics g = canvas.CreateGraphics();
            int depth;

            try
            { 
                depth = Int32.Parse(depthText.Text);
            }
            catch
            {
                g.Dispose();
                return;
            }

            g.Clear(Color.Beige);
            turtle.Parse(g, startText.Text, depth, point, 
                Parser.Parsecolor(colorText.Text),
                Parser.ParsPair(widthText.Text),
                Parser.ParsPair(scaleText.Text),
                Parser.ParsPair(rotationText.Text));
        }

        private void Save()
        {
            Descriptor descriptor = new Descriptor
            {
                Point = point,
                Rules = rulesText.Text,
                Axiom = startText.Text,
                Depth = depthText.Text,
                Color = colorText.Text,
                Width = widthText.Text,
                Scale = scaleText.Text,
                Rotation = rotationText.Text
            };

            using (var dialog = new SaveFileDialog
            {
                Title = "Сохранение в файл",
                DefaultExt = ".fracjson",
                Filter = "Fractal in JSON (*.fracjson)|*.fracjson"
            })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllText(dialog.FileName, JsonSerializer.Serialize(descriptor));
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(
                            $"Не удалось сохранить в файл '${dialog.FileName}': ${e.Message}.", "Окошко-всплывашка",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void LoadFromFile(string filename)
        {
            Descriptor descriptor = JsonSerializer.Deserialize<Descriptor>(File.ReadAllText(filename));

            point = descriptor.Point;
            rulesText.Text = descriptor.Rules;
            startText.Text = descriptor.Axiom;
            depthText.Text = descriptor.Depth;
            colorText.Text = descriptor.Color;
            widthText.Text = descriptor.Width;
            scaleText.Text = descriptor.Scale;
            rotationText.Text = descriptor.Rotation;
        }

        private void Load()
        {
            using (var dialog = new OpenFileDialog
            {
                Title = "Загрузка из файла",
                DefaultExt = ".fracjson",
                Filter = "Polygons in JSON (*.fracjson)|*.fracjson"
            })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        LoadFromFile(dialog.FileName);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(
                            $"Не удалось загрузить файл '${dialog.FileName}': ${e.Message}.", "Окошко-всплывашка",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        public override Control[] Controls => new[] { canvas, panel };
        public override Size Size
        {
            set
            {
                canvas.Width = value.Width - panel.Width;
                rulesText.Height = panel.Height - 290;
            }
        }
    }
}
