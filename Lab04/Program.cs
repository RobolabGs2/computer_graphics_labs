using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lab04.Tools;

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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(new List<ITool> {
                new Tools.Move(),
                new Tools.Rotate(),
                new Tools.Scale(),
                new Tools.Rectangle(),
                new Tools.Division(),
                new Tools.SelectAll(),
                new Tools.Remove(),
                new Tools.FixRotate(-Math.PI / 2, Properties.Resources.RightAngle),
                new Tools.Interseption(),
                new Tools.Arrow()
            }));
        }
    }
}
