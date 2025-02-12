using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livrable1.Controller;

namespace Livrable1.View
{
    internal class ViewAddBackup
    {
        public static void AddBackup()
        {
            ViewConsole.ShowLogo();
            Console.WriteLine(LanguageManager.GetText("menu_add") + "\n");
        }

        public static string ChooseLogFormat()
        {
            string format;
            do
            {
                Console.WriteLine(LanguageManager.GetText("log_format"));
                Console.WriteLine("1. JSON");
                Console.WriteLine("2. XML");
                Console.Write(LanguageManager.GetText("format_choice"));
                string choice = Console.ReadLine().Trim();

                if (choice == "1")
                    format = "json";
                else if (choice == "2")
                    format = "xml";
                else
                {
                    Console.WriteLine("Choix invalide. Veuillez réessayer.");
                    format = "";
                }
            } while (string.IsNullOrEmpty(format));

            return format;
        }
    }
}
