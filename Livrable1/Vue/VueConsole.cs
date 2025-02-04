using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Livrable1.Vue
{
    public class VueConsole
    {
        public static void AfficherMenu()
        {
            AfficherLogo();
            Console.WriteLine("===== Menu Sauvegarde =====");
            Console.WriteLine("[1] Ajouter un travail de sauvegarde");
            Console.WriteLine("[2] Exécuter un travail de sauvegarde");
            Console.WriteLine("[3] Choisir une langue");
            Console.WriteLine("[4] Voir les journaux de sauvegardes");
            Console.WriteLine("[5] Quitter");
            Console.WriteLine("=============================");
            Console.Write("Votre choix : ");
        }

        public static void AfficherMenuQuitter()
        {
            AfficherLogo();
            Console.WriteLine("Merci d'avoir utilisé le gestionnaire de bibliothèque.");
        }

        public static void AfficherLogo()
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
