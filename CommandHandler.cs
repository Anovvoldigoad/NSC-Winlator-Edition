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
        private string? gameFolder;

        public void Run()
        {
            while (true)
            {
                Console.Write("> ");
                string? input = Console.ReadLine();
                if (string.IsNullOrEmpty(input)) continue;

                string[] parts = input.Split(' ');
                string command = parts[0].ToLower();
                string[] args = parts.Skip(1).ToArray();

                try
                {
                    switch (command)
                    {
                        case "help":
                            ShowHelp();
                            break;
                        case "setgame":
                            SetGameFolder(args);
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
                        case "launch":
                            LaunchGame().Wait();
                            break;
                        case "compile":
                            CompileAndLaunch().Wait();
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
Commands:
  setgame <path>    - Set game folder
  list              - List installed mods
  install <path>    - Install mod
  remove <name>     - Remove mod
  launch            - Launch game (vanilla)
  compile           - Compile mods + launch
  exit              - Exit
");
        }

        private void SetGameFolder(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: setgame <path>");
                return;
            }
            gameFolder = string.Join(" ", args);
            if (Directory.Exists(gameFolder))
                Console.WriteLine($"✓ Game folder: {gameFolder}");
            else
                Console.WriteLine("✗ Not found");
        }

        private void ListMods()
        {
            try
            {
                var modDirs = Directory.GetDirectories(AppBootstrap.ModsFolder);
                Console.WriteLine($"\n{modDirs.Length} mod(s):\n");
                foreach (var dir in modDirs)
                    Console.WriteLine($"  • {Path.GetFileName(dir)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ {ex.Message}");
            }
        }

        private async Task InstallMod(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: install <path>");
                return;
            }

            try
            {
                await AppBootstrap.ModInstaller!.InstallMod(args[0], AppBootstrap.ModsFolder);
                Console.WriteLine("✓ Installed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ {ex.Message}");
            }
        }

        private void RemoveMod(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: remove <name>");
                return;
            }

            try
            {
                string modPath = Path.Combine(AppBootstrap.ModsFolder, string.Join(" ", args));
                if (Directory.Exists(modPath))
                {
                    Directory.Delete(modPath, true);
                    Console.WriteLine("✓ Removed");
                }
                else
                    Console.WriteLine("✗ Not found");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ {ex.Message}");
            }
        }

        private async Task LaunchGame()
        {
            if (string.IsNullOrEmpty(gameFolder))
            {
                Console.WriteLine("✗ setgame <path> first");
                return;
            }

            try
            {
                var exeFiles = Directory.GetFiles(gameFolder, "*.exe");
                if (exeFiles.Length == 0)
                {
                    Console.WriteLine("✗ No .exe found");
                    return;
                }

                Console.WriteLine("✓ Launching...");
                System.Diagnostics.Process.Start(exeFiles[0]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ {ex.Message}");
            }
            await Task.CompletedTask;
        }

        private async Task CompileAndLaunch()
        {
            if (string.IsNullOrEmpty(gameFolder))
            {
                Console.WriteLine("✗ setgame <path> first");
                return;
            }

            try
            {
                var modDirs = Directory.GetDirectories(AppBootstrap.ModsFolder);
                if (modDirs.Length == 0)
                {
                    Console.WriteLine("No mods. Launching vanilla...");
                    await LaunchGame();
                    return;
                }

                Console.WriteLine($"Compiling {modDirs.Length} mod(s)...");

                string tempOutput = Path.Combine(Path.GetTempPath(), "nsc_compiled.cpk");
                
                // Compile each mod
                foreach (var modDir in modDirs)
                {
                    string modName = Path.GetFileName(modDir);
                    Console.WriteLine($"  • {modName}");
                    
                    try
                    {
                        AppBootstrap.CompilerService!.SetGameFolder(gameFolder); await AppBootstrap.CompilerService!.CompileModToCpk(modDir, tempOutput);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"    ✗ {ex.Message}");
                    }
                }

                Console.WriteLine("✓ Compiled. Launching...");
                await LaunchGame();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ {ex.Message}");
            }
        }
    }
}
