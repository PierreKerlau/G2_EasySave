using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

namespace Livrable1.logger
{
    // Represents a single log entry for a backup operation
    public class LogEntry
    {
        // Name of the backup job
        public required string Name { get; set; }
        // Source file path
        public required string FileSource { get; set; }
        // Target file path where the backup is stored
        public required string FileTarget { get; set; }
        // Size of the file in bytes
        public required long FileSize { get; set; }
        // Time taken to transfer the file in seconds
        public required double FileTransferTime { get; set; }
        // Time taken to encrypt the file in seconds
        public required double CryptingTime { get; set; }
        // Timestamp of when the operation occurred
        public required string time { get; set; }

        // Constructor initializes timestamp and sets default encryption time
        public LogEntry()
        {
            time = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            CryptingTime = 0;
        }
    }

    // Represents the current state of a backup job
    public class StateEntry
    {
        // Name of the backup job
        public string BackupName { get; set; }
        // Timestamp of the last action performed
        public DateTime LastActionTimestamp { get; set; }
        // Current action being performed (e.g., "Copying", "Encrypting")
        public string CurrentAction { get; set; }
    }

    // Main logger class for handling backup operations logging
    public class Logger
    {
        // Directory where log files are stored
        private readonly string _logDirectory;
        // Path to the state file that tracks backup progress
        private readonly string _stateFilePath;

        // Constructor initializes logging directory
        public Logger(string logDirectory = @"../../../Logs")
        {
            _logDirectory = logDirectory;
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
            _stateFilePath = Path.Combine(_logDirectory, "backup_state.json");
        }

        // Logs a single backup operation with details about the transfer
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

        // Writes log entry to JSON file
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

        // Writes log entry to XML file
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

        // Updates the state of a backup job
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
