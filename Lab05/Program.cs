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
            var commandLineArgs = Environment.GetCommandLineArgs();
            // Через аргумент можно задать вкладку, открывающуюся по умолчанию
            var defaultSolution = commandLineArgs.Length < 2 ? 0 : int.Parse(commandLineArgs[1]); 
            Application.Run(new MainForm(new ISolution[]
            {
                new LSystem.Solution(),
                new MidpointDisplacement.Solution(), 
                new StubSolution("Кубические сплайны Безье"), 
            }, defaultSolution));
        }
    }
}
