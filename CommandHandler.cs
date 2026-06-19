using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
                            InstallMod(args).Wait();
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
            try
            {
                if (!Directory.Exists(AppBootstrap.ModsFolder))
                {
                    Console.WriteLine("No mods folder found");
                    return;
                }

                var modDirs = Directory.GetDirectories(AppBootstrap.ModsFolder);
                if (modDirs.Length == 0)
                {
                    Console.WriteLine("No mods installed");
                    return;
                }

                Console.WriteLine($"\nFound {modDirs.Length} mod(s):\n");
                foreach (var dir in modDirs)
                {
                    var dirInfo = new DirectoryInfo(dir);
                    Console.WriteLine($"  • {dirInfo.Name}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error: {ex.Message}");
            }
        }

        private async Task InstallMod(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: install <path-to-mod.zip>");
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
                Console.WriteLine($"Installing mod from: {modPath}");
                
                if (AppBootstrap.ModInstaller == null)
                {
                    Console.WriteLine("✗ Mod installer not initialized");
                    return;
                }

                await AppBootstrap.ModInstaller.InstallMod(modPath, AppBootstrap.ModsFolder);
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
            try
            {
                string profilesFolder = AppBootstrap.ProfilesFolder;
                if (!Directory.Exists(profilesFolder))
                {
                    Console.WriteLine("No profiles folder found");
                    return;
                }

                var profileDirs = Directory.GetDirectories(profilesFolder);
                if (profileDirs.Length == 0)
                {
                    Console.WriteLine("No profiles created");
                    return;
                }

                Console.WriteLine($"\nFound {profileDirs.Length} profile(s):\n");
                foreach (var dir in profileDirs)
                {
                    var dirInfo = new DirectoryInfo(dir);
                    Console.WriteLine($"  • {dirInfo.Name}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error: {ex.Message}");
            }
        }

        private void CreateBackup()
        {
            Console.WriteLine("✓ Backup feature coming soon");
        }

        private void RestoreBackup(string[] args)
        {
            Console.WriteLine("✓ Restore feature coming soon");
        }

        private void LaunchGame()
        {
            Console.WriteLine("✓ Launch feature coming soon");
        }
    }
}
