using Lab06.Base3D;
using Lab06.Tools3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab06
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
            Application.Run(new MainForm(new List<IToolPage> {
                new Tools3D.AddItem.AddItem(),
                new Tools3D.Matrixes.Matrixes(),
                new Tools3D.Selection.Selection(),
                new Tools3D.Transformation.Transformation(),
                new Tools3D.Render.Render(),
            }));
        }
    }
}
