using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Livrable1.View
{
    public class ViewConsole
    {
        public static void ShowMenu()
        {
            ShowLogo();
            Console.WriteLine("======== Backup menu ========\n");
            Console.WriteLine("[1] Add a backup job");
            Console.WriteLine("[2] Run a backup job");
            Console.WriteLine("[3] Recover a backup");
            Console.WriteLine("[4] Choose a language");
            Console.WriteLine("[5] View backup logs");
            Console.WriteLine("[6] Exit");
            Console.WriteLine("=============================");
            Console.Write("Your choice : ");
        }

        public static void ShowMenuLeave()
        {
            ShowLogo();
            Console.WriteLine("Thanks for using EasySave.");
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
