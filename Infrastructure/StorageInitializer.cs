namespace NSC.Winlator.Infrastructure
{
    public static class StorageInitializer
    {
        public static void InitializeDirectories(string baseFolder)
        {
            try
            {
                string[] requiredFolders = new[]
                {
                    Path.Combine(baseFolder, "Mods"),
                    Path.Combine(baseFolder, "Profiles"),
                    Path.Combine(baseFolder, "Logs"),
                    Path.Combine(baseFolder, "Backups"),
                    Path.Combine(baseFolder, "Cache"),
                    Path.Combine(baseFolder, "Config"),
                    Path.Combine(baseFolder, "Tools")
                };

                foreach (string folder in requiredFolders)
                {
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                        LoggerService.LogInfo($"Created directory: {folder}");
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerService.LogError("Failed to initialize directories", ex);
            }
        }
    }
}
