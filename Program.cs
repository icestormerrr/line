using System;
using System.Windows.Forms;
using Line.Controllers;

namespace Line
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Создаем контроллер
            var controller = new MainController();
            
            // Создаем и запускаем главную форму
            Application.Run(new MainForm());
        }
    }
}
