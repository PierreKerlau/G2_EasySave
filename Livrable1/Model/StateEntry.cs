namespace Livrable1.Model
{
    public class StateEntry
    {
        public string BackupName { get; set; }
        public DateTime LastActionTimestamp { get; set; }
        public string CurrentAction { get; set; }

        public override string ToString()
        {
            return $"{LastActionTimestamp:yyyy-MM-dd HH:mm:ss} - {BackupName}: {CurrentAction}";
        }
    }
} 