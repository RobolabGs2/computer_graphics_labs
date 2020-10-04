using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab04
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var args = Environment.GetCommandLineArgs();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(new List<ITool> {
                new Tools.Save(),
                new Tools.Load(args.Length > 1 ? args[1] : null),
                new Tools.Rectangle(),
                new Tools.InternalPoint(),
                new Tools.SelectAll(),
                new Tools.Remove(),
                new Tools.Move(),
                new Tools.Rotate(),
                new Tools.Scale(),
                new Tools.FixRotate(-Math.PI / 2, Properties.Resources.RightAngle),
                new Tools.Division(),
                new Tools.Collision(),
                new Tools.Intersection(),
                new Tools.Debug()
            }));
        }
    }
}
