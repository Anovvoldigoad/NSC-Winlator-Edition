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
  setgame <path>      - Set game folder
  list                - List installed mods
  install <path>      - Install mod
  remove <name>       - Remove mod
  launch              - Launch game (vanilla)
  compile             - Compile mods + launch
  exit                - Exit
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
                Console.WriteLine($"✓ Game folder set: {gameFolder}");
            else
                Console.WriteLine("✗ Path not found");
        }

        private void ListMods()
        {
            try
            {
                var modDirs = Directory.GetDirectories(AppBootstrap.ModsFolder);
                Console.WriteLine($"\nFound {modDirs.Length} mod(s):\n");
                foreach (var dir in modDirs)
                {
                    Console.WriteLine($"  • {Path.GetFileName(dir)}");
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
                Console.WriteLine("Usage: install <path-to-mod.nsc>");
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
                await AppBootstrap.ModInstaller!.InstallMod(modPath, AppBootstrap.ModsFolder);
                Console.WriteLine("✓ Mod installed");
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

        private async Task LaunchGame()
        {
            if (string.IsNullOrEmpty(gameFolder))
            {
                Console.WriteLine("✗ Set game folder first: setgame <path>");
                return;
            }

            try
            {
                var exeFiles = Directory.GetFiles(gameFolder, "*.exe");
                if (exeFiles.Length == 0)
                {
                    Console.WriteLine($"✗ No .exe found in: {gameFolder}");
                    return;
                }

                Console.WriteLine($"Launching: {exeFiles[0]}");
                System.Diagnostics.Process.Start(exeFiles[0]);
                Console.WriteLine("✓ Game launched");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Launch failed: {ex.Message}");
            }
            await Task.CompletedTask;
        }

        private async Task CompileAndLaunch()
        {
            if (string.IsNullOrEmpty(gameFolder))
            {
                Console.WriteLine("✗ Set game folder first: setgame <path>");
                return;
            }

            try
            {
                var modDirs = Directory.GetDirectories(AppBootstrap.ModsFolder);
                if (modDirs.Length == 0)
                {
                    Console.WriteLine("No mods to compile. Launching vanilla...");
                    await LaunchGame();
                    return;
                }

                Console.WriteLine($"Compiling {modDirs.Length} mod(s)...");

                string tempOutput = Path.Combine(Path.GetTempPath(), "nsc_compiled.cpk");
                
                // Create workspace
                string workspace = Path.Combine(Path.GetTempPath(), $"nsc_compile_{Guid.NewGuid()}");
                Directory.CreateDirectory(workspace);

                try
                {
                    // Merge mod files
                    Console.WriteLine("Merging mod files...");
                    foreach (var modDir in modDirs)
                    {
                        string modName = Path.GetFileName(modDir);
                        Console.WriteLine($"  Merging: {modName}");
                        
                        // Copy mod files to workspace
                        foreach (var file in Directory.GetFiles(modDir, "*", SearchOption.AllDirectories))
                        {
                            if (Path.GetFileName(file) == "mod_config.ini") continue;
                            
                            string relativePath = Path.GetRelativePath(modDir, file);
                            string targetPath = Path.Combine(workspace, relativePath);
                            Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
                            File.Copy(file, targetPath, overwrite: true);
                        }
                    }

                    // Compile with YACpk
                    Console.WriteLine("Compiling with YACpk...");
                    if (AppBootstrap.CompilerService != null)
                    {
                        bool success = await AppBootstrap.CompilerService.CompileMods(
                            modDirs.Select(d => new Models.ModInfo { Name = Path.GetFileName(d), ModFolder = d, Enabled = true }).ToList(),
                            tempOutput
                        );

                        if (success)
                        {
                            Console.WriteLine("✓ Compilation complete");
                            await LaunchGame();
                        }
                        else
                        {
                            Console.WriteLine("✗ Compilation failed");
                        }
                    }
                }
                finally
                {
                    if (Directory.Exists(workspace))
                        Directory.Delete(workspace, true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error: {ex.Message}");
            }
        }
    }
}
