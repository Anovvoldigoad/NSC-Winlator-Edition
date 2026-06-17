namespace NSC.Winlator.Models
{
    public class ModInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ModFolder { get; set; } = string.Empty;
        public string? IconPath { get; set; }
        public bool Enabled { get; set; }
        public DateTime LastModified { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
