using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Lab02
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var mainForm = new MainForm(new List<ISolution>
            {
                Grayscale.MakeSolution(),
                new EmptySolution()
            });
            Application.Run(mainForm);
        }
    }
    // Заглушка
    class EmptySolution : ISolution
    {
        public EmptySolution(string name = "Empty")
        {
            Name = name;
        }

        public string Name { get; }

        public void Init(Control container)
        {
        }

        public void Show(FastBitmap bitmap)
        {
        }
    }
}