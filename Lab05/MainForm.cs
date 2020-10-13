using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab05
{
    public partial class MainForm : Form
    {
        public MainForm(IEnumerable<ISolution> solutions, int startSolution = 0)
        {
            InitializeComponent();
            var tabs = new TabControl
            {
                Size = ClientSize,
                Dock = DockStyle.Fill,
            };
            tabs.TabPages.AddRange(solutions.Select((solution, i) =>
            {
                var tab = new TabPage($"{i + 1}. {solution.Name}")
                {
                    BackColor = Color.White,
                };
                tab.SizeChanged += (sender, args) =>
                {
                    if (tab.Size.Height != 0 && tab.Size.Width != 0)
                        solution.Size = tab.ClientSize;
                };
                tab.Controls.AddRange(solution.Controls);
                return tab;
            }).ToArray());
            Controls.Add(tabs);
            tabs.SelectedIndex = startSolution;
        }
    }
}