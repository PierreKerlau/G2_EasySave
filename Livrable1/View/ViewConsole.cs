using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livrable1.Controller;

namespace Livrable1.View
{
    public class ViewConsole
    {
        public static void ShowMenu()
        {
            ShowLogo();
            Console.WriteLine($"======== {LanguageManager.GetText("menu_title")} ========\n");
            Console.WriteLine($"[1] {LanguageManager.GetText("menu_add")}");
            Console.WriteLine($"[2] {LanguageManager.GetText("menu_run")}");
            Console.WriteLine($"[3] {LanguageManager.GetText("menu_recover")}");
            Console.WriteLine($"[4] {LanguageManager.GetText("menu_language")}");
            Console.WriteLine($"[5] {LanguageManager.GetText("menu_leave")}\n");
            Console.WriteLine("=============================");
            Console.WriteLine(LanguageManager.GetText("menu_choice"));
        }

        public static void ShowMenuLeave()
        {
            ShowLogo();
            Console.WriteLine(LanguageManager.GetText("menu_thanks"));
        }

        public static void ShowLogo()
        {
            Console.WriteLine(" _____                ____                  ");
            Console.WriteLine("| ____|__ _ ___ _   _/ ___|  __ ___   _____ ");
            Console.WriteLine("|  _| / _` / __| | | \\___ \\ / _` \\ \\ / / _ \\");
            Console.WriteLine("| |__| (_| \\__ \\ |_| |___) | (_| |\\ V /  __/");
            Console.WriteLine("|_____\\__,_|___/\\__, |____/ \\__,_| \\_/ \\___|");
            Console.WriteLine("                |___/                       ");
            Console.WriteLine("");
        }
    }
}
