using System;
using Livrable1.Controller;
using Livrable1.View;
class Program
{
    static void Main(string[] args)
    {
        CtrlBackup sauvegarde = new CtrlBackup();
        bool quitter = false;

        while (!quitter)
        {
            ViewConsole.ShowMenu();
            ConsoleKeyInfo choix = Console.ReadKey();
            Console.Clear();

            switch (choix.KeyChar)
            {
                case '1':
                    sauvegarde.AddBackup();
                    break;
                case '2':
                    sauvegarde.ExecuteBackup();
                    break;
                case '3':
                    sauvegarde.RecoverBackup();
                    break;
                case '4':
                    sauvegarde.ChoiceLanguage();
                    break;
                case '5':
                    quitter = true;
                    ViewConsole.ShowMenuLeave();
                    break;
                default:
                    ViewConsole.ShowLogo();
                    Console.WriteLine("Choix non valide, veuillez réessayer.");
                    break;
            }
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
            Console.Clear();
        }
    }
}