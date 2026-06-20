using NSC.Winlator.Services;

namespace NSC.Winlator.Infrastructure
{
    public static class AppBootstrap
    {
        // Service instances (simple DI)
        public static ModScanner? ModScanner { get; private set; }
        public static ModConfigService? ModConfigService { get; private set; }
        public static ProfileManager? ProfileManager { get; private set; }
        public static BackupService? BackupService { get; private set; }
        public static ConflictDetector? ConflictDetector { get; private set; }
        public static LoadOrderManager? LoadOrderManager { get; private set; }
        public static GameSettingsService? GameSettingsService { get; private set; }
        public static LaunchService? LaunchService { get; private set; }
        public static ModInstaller? ModInstaller { get; private set; }
        public static CompilerService? CompilerService { get; private set; }

        public static string ApplicationFolder { get; private set; } = string.Empty;
        public static string ModsFolder { get; private set; } = string.Empty;
        public static string ProfilesFolder { get; private set; } = string.Empty;
        public static string BackupsFolder { get; private set; } = string.Empty;
        public static string ConfigFolder { get; private set; } = string.Empty;
        public static string ToolsFolder { get; private set; } = string.Empty;
        public static string LogsFolder { get; private set; } = string.Empty;

        public static void Initialize()
        {
            try
            {
                // Setup base paths
                ApplicationFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "NSC.Winlator"
                );

                ModsFolder = Path.Combine(ApplicationFolder, "Mods");
                ProfilesFolder = Path.Combine(ApplicationFolder, "Profiles");
                BackupsFolder = Path.Combine(ApplicationFolder, "Backups");
                ConfigFolder = Path.Combine(ApplicationFolder, "Config");
                ToolsFolder = Path.Combine(ApplicationFolder, "Tools");
                LogsFolder = Path.Combine(ApplicationFolder, "Logs");

                // Initialize storage
                StorageInitializer.InitializeDirectories(ApplicationFolder);

                // Initialize logging
                LoggerService.Initialize(LogsFolder);
                LoggerService.LogInfo("=== NSC Winlator Edition Starting ===");
                LoggerService.LogInfo($"Application Folder: {ApplicationFolder}");

                // Initialize services
                ModScanner = new ModScanner();
                ModConfigService = new ModConfigService();
                ProfileManager = new ProfileManager();
                ProfileManager.Initialize(ProfilesFolder);

                BackupService = new BackupService();
                BackupService.Initialize(BackupsFolder);

                ConflictDetector = new ConflictDetector();

                LoadOrderManager = new LoadOrderManager();
                LoadOrderManager.Initialize(ConfigFolder);

                GameSettingsService = new GameSettingsService();
                GameSettingsService.Initialize(ConfigFolder);

                LaunchService = new LaunchService();
                ModInstaller = new ModInstaller();

                CompilerService = new CompilerService()
CompilerService.Initialize(ToolsFolder);

                LoggerService.LogSuccess("All services initialized successfully");
            }
            catch (Exception ex)
            {
                LoggerService.LogError("Failed to initialize application", ex);
                MessageBox.Show($"Failed to initialize application: {ex.Message}", "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void Shutdown()
        {
            LoggerService.LogInfo("=== NSC Winlator Edition Shutting Down ===");
            LoggerService.Dispose();
        }
    }
}
