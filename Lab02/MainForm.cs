using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Lab02
{
    public interface ISolution
    {
        // Название будет на кнопке
        string Name { get; }
        // Установка родительского контейнера
        void Init(Control container);
        // Обработка картинки
        void Show(Bitmap bitmap);
    }

    public partial class MainForm : Form
    {
        private Bitmap _originImage;
        private (Control, ISolution) _lastSolution;

        private void SetBitmap(Bitmap bitmap)
        {
            _originImage = bitmap;
            mainPictureDisplay.Image = bitmap;
            _lastSolution.Item2?.Show(bitmap);
        }

        public MainForm(IList<ISolution> solutions)
        {
            InitializeComponent();
            this.mainPictureDisplay.SizeMode = PictureBoxSizeMode.Zoom;
            foreach (var solution in solutions)
            {
                var container = new FlowLayoutPanel
                {
                    Visible = false,
                    Width = solutionPanel.Width,
                    Height = solutionPanel.Height,
                };
                this.solutionPanel.Controls.Add(container);
                solution.Init(container);
                var button = new Button
                {
                    Text = solution.Name,
                    Dock = DockStyle.Top,
                    Width = taskButtonsPanel.Width,
                    Height = chooseImageButton.Height
                };
                button.Click += (sender, args) =>
                {
                    if (_originImage == null)
                    {
                        MessageBox.Show("Нет картинки");
                        return;
                    }

                    if (_lastSolution.Item1 != null)
                    {
                        _lastSolution.Item1.Visible = false;
                    }
                    container.Visible = true;
                    solution.Show(_originImage);
                    _lastSolution = (container, solution);
                };
                taskButtonsPanel.Controls.Add(button);
            }
        }

        private void chooseImageButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    SetBitmap(new Bitmap(openFileDialog.FileName));
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.ToString());
                }
            }
        }
    }
}