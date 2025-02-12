using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Livrable1.logger
{
    public class LogEntry
    {
        public string Name { get; set; }
        public string FileSource { get; set; }
        public string FileTarget { get; set; }
        public long FileSize { get; set; }
        public double FileTransferTime { get; set; }
        public string time { get; set; }

        public LogEntry()
        {
            time = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
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
        private string _logFormat;

        public Logger(string logDirectory = @"../../../Logs", string format = "json")
        {
            _logDirectory = logDirectory;
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
            _stateFilePath = Path.Combine(_logDirectory, "backup_state.json");
            _logFormat = format.ToLower();
        }

        public void LogBackupOperation(string jobName, string sourcePath, string destinationPath, long fileSize, long transferTime)
        {
            if (string.IsNullOrEmpty(jobName)) return;

            var logEntry = new LogEntry
            {
                Name = jobName,
                FileSource = sourcePath,
                FileTarget = destinationPath,
                FileSize = fileSize,
                FileTransferTime = transferTime / 1000.0,
            };

            string extension = _logFormat == "xml" ? "xml" : "json";
            string logFileName = $"log_{DateTime.Now:yyyy-MM-dd}.{extension}";
            string logFilePath = Path.Combine(_logDirectory, logFileName);

            List<LogEntry> dailyLogs = new();
            if (File.Exists(logFilePath))
            {
                if (_logFormat == "xml")
                {
                    var serializer = new XmlSerializer(typeof(List<LogEntry>));
                    using var reader = new StreamReader(logFilePath);
                    dailyLogs = (List<LogEntry>)serializer.Deserialize(reader);
                }
                else
                {
                    string existingContent = File.ReadAllText(logFilePath);
                    dailyLogs = JsonSerializer.Deserialize<List<LogEntry>>(existingContent) ?? new List<LogEntry>();
                }
            }

            dailyLogs = dailyLogs.Where(log => log.Name != null).ToList();
            dailyLogs.Add(logEntry);

            if (_logFormat == "xml")
            {
                var serializer = new XmlSerializer(typeof(List<LogEntry>));
                using var writer = new StreamWriter(logFilePath);
                serializer.Serialize(writer, dailyLogs);
            }
            else
            {
                string jsonContent = JsonSerializer.Serialize(dailyLogs, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = null
                });
                File.WriteAllText(logFilePath, jsonContent);
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
