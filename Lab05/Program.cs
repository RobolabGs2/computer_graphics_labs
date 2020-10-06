using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab05
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
            Application.Run(new MainForm(new []
            {
                new StubSolution("L-системы"), 
                new StubSolution("Midpoint displacement"), 
                new StubSolution("Кубические сплайны Безье"), 
            }));
        }
    }
}
