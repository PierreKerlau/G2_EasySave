using System;
using System.Text;
using Livrable1.Controller;
using Livrable1.View;
class Program
{
    static void Main(string[] args)
    {
        LanguageManager.LoadLanguages();

        CtrlBackup controller = new CtrlBackup();
        LanguageManager languageManager = new LanguageManager();
        controller.StartSauvegarde();
        Console.OutputEncoding = Encoding.UTF8;
        CtrlBackup sauvegarde = new CtrlBackup();
        bool quitter = false;

        while (!quitter)
        {
            ViewConsole.ShowMenu();
            ConsoleKeyInfo choix = Console.ReadKey();
            Console.Clear();

            switch (choix.KeyChar)
            {
                case '1': sauvegarde.AddBackup(); break;
                case '2': sauvegarde.ExecuteBackup(); break;
                case '3': sauvegarde.RecoverBackup(); break;
                case '4': languageManager.ChoiceLanguage(); break;
                case '5': sauvegarde.ShowLogs(); break;
                case '6': quitter = true; ViewConsole.ShowMenuLeave(); break;
                default: Console.WriteLine(LanguageManager.GetText("invalid_choice")); break;
            }
            Console.Write("\n" + LanguageManager.GetText("press_any_key"));
            Console.ReadKey();
            Console.Clear();
        }
    }
}