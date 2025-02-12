using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livrable1.Controller;

namespace Livrable1.View
{
    internal class ViewExecuteBackup
    {
        public static void ExecuteBackup()
        {
            ViewConsole.ShowLogo();
            Console.WriteLine(LanguageManager.GetText("menu_run") + "\n");
        }
    }
}
