using System;
using System.Text;
using Livrable1.Controller;
using Livrable1.View;
class Program
{
    static void Main(string[] args)
    {
        //// Demander à l'utilisateur de choisir une langue
        //Console.WriteLine("Choose your language:");
        //Console.WriteLine("[1] English");
        //Console.WriteLine("[2] Français");
        //char languageChoice = Console.ReadKey().KeyChar;

        //if (languageChoice == '1')
        //{
        //    LanguageManager.SetLanguage("en");
        //}
        //else if (languageChoice == '2')
        //{
        //    LanguageManager.SetLanguage("fr");
        //}
        //else
        //{
        //    Console.WriteLine("Invalid choice, defaulting to English.");
        //    LanguageManager.SetLanguage("en"); // Langue par défaut
        //}

        // Charge les langues
        LanguageManager.LoadLanguages();

        CtrlBackup controller = new CtrlBackup();
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
                case '4': sauvegarde.ChoiceLanguage(); break;
                case '5': sauvegarde.ShowLogs(); break;
                case '6': quitter = true; ViewConsole.ShowMenuLeave(); break;
                default: Console.WriteLine(LanguageManager.GetText("invalid_choice")); break;
            }
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
            Console.Clear();
        }
    }
}