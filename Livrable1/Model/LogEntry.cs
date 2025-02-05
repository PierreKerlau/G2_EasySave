namespace Livrable1.Model
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

        public override string ToString()
        {
            return $"{time} - {Name} [{Path.GetFileName(FileSource)}, {FileSize} octets] - {FileTransferTime}ms";
        }
    }
} 