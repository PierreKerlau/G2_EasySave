using System;
using System.IO;
using System.Text.Json;
using Livrable1.Model;
using System.Collections.Generic;
using Livrable1.logger;

namespace Livrable1.View
{
    internal class ViewLogs
    {
        public static void ShowLogs()
        {
            ViewConsole.ShowLogo();
            string logDirectory = @"../../../Logs";
            Console.WriteLine($"Chemin du dossier des logs : {Path.GetFullPath(logDirectory)}");

            string todayLogFile = Path.Combine(logDirectory, $"log_{DateTime.Now:yyyy-MM-dd}.json");
            if (File.Exists(todayLogFile))
            {
                Console.WriteLine("\nLogs du jour :");
                Console.WriteLine("-------------");
                string logContent = File.ReadAllText(todayLogFile);
                var logs = JsonSerializer.Deserialize<List<LogEntry>>(logContent);

                if (logs != null && logs.Count > 0)
                {
                    foreach (var log in logs)
                    {
                        if (log.Name != null)
                        {
                            Console.WriteLine($"\nSauvegarde : {log.Name}");
                            Console.WriteLine($"Date/Heure : {log.time}");
                            Console.WriteLine($"Source : {log.FileSource}");
                            Console.WriteLine($"Destination : {log.FileTarget}");
                            Console.WriteLine($"Taille : {log.FileSize} octets");
                            Console.WriteLine($"Temps de transfert : {log.FileTransferTime:F3} secondes");
                            Console.WriteLine("----------------------------------------");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Aucune entrée de log valide pour aujourd'hui.");
                }
            }
            else
            {
                Console.WriteLine("\nAucun log pour aujourd'hui.");
            }
        }
    }
}
