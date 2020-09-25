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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(new List<ITool> {
                new Tools.Move(),
                new Tools.JustPicture(Properties.Resources.Point),
                new Tools.JustPicture(Properties.Resources.Scale),
                new Tools.JustPicture(Properties.Resources.Rotate),
                new Tools.JustPicture(Properties.Resources.Rectangle),
                new Tools.JustPicture(Properties.Resources.Devision)
            }));
        }
    }
}
