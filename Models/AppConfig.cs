namespace NSC.Winlator.Models
{
    public class AppConfig
    {
        public string ModsFolder { get; set; } = string.Empty;
        public string ProfilesFolder { get; set; } = string.Empty;
        public string BackupFolder { get; set; } = string.Empty;
        public string LogsFolder { get; set; } = string.Empty;
        public string? LastProfile { get; set; }
    }
}
