using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

namespace Livrable1.logger
{
    public class LogEntry
    {
        public required string Name { get; set; }
        public required string FileSource { get; set; }
        public required string FileTarget { get; set; }
        public required long FileSize { get; set; }
        public required double FileTransferTime { get; set; }
        public required double CryptingTime { get; set; }
        public required string time { get; set; }

        public LogEntry()
        {
            time = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            CryptingTime = 0;
        }
    }

    public class StateEntry
    {
        public string BackupName { get; set; }
        public DateTime LastActionTimestamp { get; set; }
        public string CurrentAction { get; set; }
    }

    public class Logger
    {
        private readonly string _logDirectory;
        private readonly string _stateFilePath;

        public Logger(string logDirectory = @"../../../Logs")
        {
            _logDirectory = logDirectory;
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
            _stateFilePath = Path.Combine(_logDirectory, "backup_state.json");
        }

        public void LogBackupOperation(string jobName, string sourcePath, string destinationPath, long fileSize, long transferTime, long cryptingTime, bool useJson)
        {
            if (string.IsNullOrEmpty(jobName)) return;

            var logEntry = new LogEntry
            {
                Name = jobName,
                FileSource = sourcePath,
                FileTarget = destinationPath,
                FileSize = fileSize,
                FileTransferTime = transferTime / 1000.0,
                CryptingTime = cryptingTime > 0 ? cryptingTime / 1000.0 : 0.0,
                time = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
            };

            string extension = useJson ? "json" : "xml";
            string logFileName = $"log_{DateTime.Now:yyyy-MM-dd}.{extension}";
            string logFilePath = Path.Combine(_logDirectory, logFileName);

            if (useJson)
            {
                WriteJsonLog(logEntry, logFilePath);
            }
            else
            {
                WriteXmlLog(logEntry, logFilePath);
            }
        }

        private void WriteJsonLog(LogEntry logEntry, string logFilePath)
        {
            List<LogEntry> dailyLogs = new();
            if (File.Exists(logFilePath))
            {
                string existingContent = File.ReadAllText(logFilePath);
                dailyLogs = JsonSerializer.Deserialize<List<LogEntry>>(existingContent) ?? new List<LogEntry>();
            }

            dailyLogs.Add(logEntry);

            string jsonContent = JsonSerializer.Serialize(dailyLogs, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = null
            });

            File.WriteAllText(logFilePath, jsonContent);
        }

        private void WriteXmlLog(LogEntry logEntry, string logFilePath)
        {
            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(List<LogEntry>));
            List<LogEntry> dailyLogs = new();

            if (File.Exists(logFilePath))
            {
                using (var reader = new StreamReader(logFilePath))
                {
                    try
                    {
                        dailyLogs = (List<LogEntry>)xmlSerializer.Deserialize(reader);
                    }
                    catch
                    {
                        dailyLogs = new List<LogEntry>();
                    }
                }
            }

            dailyLogs.Add(logEntry);

            using (var writer = new StreamWriter(logFilePath))
            {
                xmlSerializer.Serialize(writer, dailyLogs);
            }
        }

        public void UpdateState(string backupName, string currentAction)
        {
            var stateEntry = new StateEntry
            {
                BackupName = backupName,
                LastActionTimestamp = DateTime.Now,
                CurrentAction = currentAction
            };

            Dictionary<string, StateEntry> states = new();
            if (File.Exists(_stateFilePath))
            {
                string existingContent = File.ReadAllText(_stateFilePath);
                states = JsonSerializer.Deserialize<Dictionary<string, StateEntry>>(existingContent)
                    ?? new Dictionary<string, StateEntry>();
            }

            states[backupName] = stateEntry;
            string jsonContent = JsonSerializer.Serialize(states, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_stateFilePath, jsonContent);
        }
    }
}
