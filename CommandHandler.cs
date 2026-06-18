using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NSC.Winlator.Infrastructure;
using NSC.Winlator.Services;

namespace NSC.Winlator
{
    public class CommandHandler
    {
        public void Run()
        {
            while (true)
            {
                Console.Write("> ");
                string? input = Console.ReadLine();
                if (string.IsNullOrEmpty(input)) continue;

                string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                string command = parts[0].ToLower();
                string[] args = parts.Skip(1).ToArray();

                try
                {
                    switch (command)
                    {
                        case "help":
                            ShowHelp();
                            break;
                        case "list":
                            ListMods();
                            break;
                        case "install":
                            InstallMod(args);
                            break;
                        case "remove":
                            RemoveMod(args);
                            break;
                        case "profiles":
                            ListProfiles();
                            break;
                        case "backup":
                            CreateBackup();
                            break;
                        case "restore":
                            RestoreBackup(args);
                            break;
                        case "launch":
                            LaunchGame();
                            break;
                        case "exit":
                            return;
                        default:
                            Console.WriteLine("Unknown command. Type 'help'");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"✗ Error: {ex.Message}");
                }
                Console.WriteLine();
            }
        }

        private void ShowHelp()
        {
            Console.WriteLine(@"
Available Commands:
  list              - List all installed mods
  install <path>   - Install mod from ZIP file
  remove <name>    - Remove mod by name
  profiles          - List all profiles
  backup            - Create game backup
  restore <backup>  - Restore from backup
  launch            - Launch game with mods
  help              - Show this help
  exit              - Exit application
");
        }

        private void ListMods()
        {
            if (AppBootstrap.ModScanner == null)
            {
                Console.WriteLine("✗ Mod scanner not initialized");
                return;
            }

            var mods = AppBootstrap.ModScanner.ScanForMods(AppBootstrap.ModsFolder);
            if (!mods.Any())
            {
                Console.WriteLine("No mods installed");
                return;
            }

            Console.WriteLine($"\nFound {mods.Count} mod(s):\n");
            foreach (var mod in mods)
            {
                Console.WriteLine($"  • {mod.Name} v{mod.Version}");
                Console.WriteLine($"    Path: {mod.FolderPath}");
                Console.WriteLine($"    Enabled: {(mod.IsEnabled ? "Yes" : "No")}");
            }
        }

        private void InstallMod(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: install <path-to-mod-zip>");
                return;
            }

            string modPath = args[0];
            if (!File.Exists(modPath))
            {
                Console.WriteLine($"✗ File not found: {modPath}");
                return;
            }

            try
            {
                if (AppBootstrap.ModInstaller == null)
                {
                    Console.WriteLine("✗ Mod installer not initialized");
                    return;
                }

                Console.WriteLine($"Installing mod from: {modPath}");
                AppBootstrap.ModInstaller.InstallMod(modPath, AppBootstrap.ModsFolder).Wait();
                Console.WriteLine("✓ Mod installed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Installation failed: {ex.Message}");
            }
        }

        private void RemoveMod(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: remove <mod-name>");
                return;
            }

            string modName = string.Join(" ", args);
            Console.WriteLine($"Removing mod: {modName}");
            
            try
            {
                string modPath = Path.Combine(AppBootstrap.ModsFolder, modName);
                if (Directory.Exists(modPath))
                {
                    Directory.Delete(modPath, true);
                    Console.WriteLine("✓ Mod removed");
                }
                else
                {
                    Console.WriteLine("✗ Mod not found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error: {ex.Message}");
            }
        }

        private void ListProfiles()
        {
            if (AppBootstrap.ProfileManager == null)
            {
                Console.WriteLine("✗ Profile manager not initialized");
                return;
            }

            var profiles = AppBootstrap.ProfileManager.GetAllProfiles();
            if (!profiles.Any())
            {
                Console.WriteLine("No profiles created");
                return;
            }

            Console.WriteLine($"\nFound {profiles.Count} profile(s):\n");
            foreach (var profile in profiles)
            {
                Console.WriteLine($"  • {profile.Name}");
                Console.WriteLine($"    Mods: {profile.EnabledMods.Count}");
            }
        }

        private void CreateBackup()
        {
            try
            {
                if (AppBootstrap.BackupService == null)
                {
                    Console.WriteLine("✗ Backup service not initialized");
                    return;
                }

                Console.WriteLine("Creating backup...");
                AppBootstrap.BackupService.CreateBackup("Manual Backup").Wait();
                Console.WriteLine("✓ Backup created");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Backup failed: {ex.Message}");
            }
        }

        private void RestoreBackup(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: restore <backup-name>");
                return;
            }

            string backupName = string.Join(" ", args);
            Console.WriteLine($"Restoring from: {backupName}");
            Console.WriteLine("⚠ This will overwrite game files!");
        }

        private void LaunchGame()
        {
            try
            {
                if (AppBootstrap.LaunchService == null)
                {
                    Console.WriteLine("✗ Launch service not initialized");
                    return;
                }

                Console.WriteLine("Launching game...");
                AppBootstrap.LaunchService.LaunchGame().Wait();
                Console.WriteLine("✓ Game launched");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Launch failed: {ex.Message}");
            }
        }
    }
}
