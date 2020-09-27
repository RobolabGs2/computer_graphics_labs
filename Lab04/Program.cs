﻿using System;
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
                new Tools.Rectangle(),
                new Tools.Devision(),
                new Tools.SelectAll(),
                new Tools.Remove(),
            }));
        }
    }
}
