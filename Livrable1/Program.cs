using System;
using Livrable1.Controlleur;
using Livrable1.Vue;
class Program
{
    static void Main(string[] args)
    {
        CtrlSauvegarde sauvegarde = new CtrlSauvegarde();
        bool quitter = false;

        while (!quitter)
        {
            VueConsole.AfficherMenu();
            ConsoleKeyInfo choix = Console.ReadKey();
            Console.Clear();

            switch (choix.KeyChar)
            {
                case '1':
                    sauvegarde.AjouterTravail();
                    break;
                case '2':
                    sauvegarde.ExecuterSauvegarde();
                    break;
                case '3':
                    sauvegarde.RecupererSauvegarde();
                    break;
                case '4':
                    sauvegarde.ChoisirLangue();
                    break;
                case '5':
                    sauvegarde.VoirLogs();
                    break;
                case '6':
                    quitter = true;
                    VueConsole.AfficherMenuQuitter();
                    break;
                default:
                    Console.WriteLine("Choix non valide, veuillez réessayer.");
                    break;
            }
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
            Console.Clear();
        }
    }
}