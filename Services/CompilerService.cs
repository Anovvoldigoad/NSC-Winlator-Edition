using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using NSC.Winlator.Models;

namespace NSC.Winlator.Services
{
    public class CompilerService
    {
        private YacpkWrapper _yacpkWrapper = new();
        private string _toolsFolder = string.Empty;

        public event EventHandler<string>? CompileProgress;

        public void Initialize(string toolsFolder)
        {
            _toolsFolder = toolsFolder;
            string yacpkPath = Path.Combine(toolsFolder, "YACpkTool.exe");
            _yacpkWrapper.SetToolPath(yacpkPath);
        }

        public async Task<bool> CompileMods(List<ModInfo> enabledMods, string outputPath)
        {
            if (!_yacpkWrapper.ValidateTool())
            {
                LoggerService.LogError("YACpk tool is not available");
                return false;
            }

            if (!enabledMods.Any())
            {
                LoggerService.LogWarning("No enabled mods to compile");
                return false;
            }

            try
            {
                OnCompileProgress("Creating temporary workspace...");
                LoggerService.LogCompile("Starting mod compilation workflow");

                // Create temporary workspace
                string workspaceFolder = Path.Combine(Path.GetTempPath(), $"nsc_compile_{Guid.NewGuid()}");
                Directory.CreateDirectory(workspaceFolder);

                try
                {
                    // Merge mod files into workspace
                    OnCompileProgress("Merging mod files...");
                    if (!await MergeModFiles(enabledMods, workspaceFolder))
                    {
                        LoggerService.LogError("Failed to merge mod files");
                        return false;
                    }

                    // Compile using YACpk
                    OnCompileProgress("Compiling with YACpk...");
                    if (!_yacpkWrapper.ExecuteCompile(workspaceFolder, outputPath))
                    {
                        LoggerService.LogError("YACpk compilation failed");
                        return false;
                    }

                    OnCompileProgress("Compilation complete!");
                    LoggerService.LogSuccess("Mod compilation completed successfully");
                    return true;
                }
                finally
                {
                    // Cleanup workspace
                    if (Directory.Exists(workspaceFolder))
                    {
                        Directory.Delete(workspaceFolder, recursive: true);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerService.LogError("Compilation failed", ex);
                return false;
            }
        }

        private async Task<bool> MergeModFiles(List<ModInfo> enabledMods, string targetFolder)
        {
            try
            {
                foreach (var mod in enabledMods)
                {
                    OnCompileProgress($"Merging mod: {mod.Name}");
                    
                    if (!Directory.Exists(mod.ModFolder))
                    {
                        LoggerService.LogWarning($"Mod folder not found: {mod.ModFolder}");
                        continue;
                    }

                    // Copy mod files (excluding mod_config.ini)
                    await CopyModFilesAsync(mod.ModFolder, targetFolder);
                    
                    LoggerService.LogCompile($"Merged mod: {mod.Name}");
                }

                return true;
            }
            catch (Exception ex)
            {
                LoggerService.LogError("Failed to merge mod files", ex);
                return false;
            }
        }

        private async Task CopyModFilesAsync(string sourceFolder, string targetFolder)
        {
            try
            {
                foreach (string file in Directory.GetFiles(sourceFolder))
                {
                    // Skip mod_config.ini
                    if (Path.GetFileName(file) == "mod_config.ini")
                        continue;

                    string destFile = Path.Combine(targetFolder, Path.GetFileName(file));
                    File.Copy(file, destFile, overwrite: true);
                }

                foreach (string subDir in Directory.GetDirectories(sourceFolder))
                {
                    string dirName = Path.GetFileName(subDir);
                    string destDir = Path.Combine(targetFolder, dirName);
                    
                    if (!Directory.Exists(destDir))
                        Directory.CreateDirectory(destDir);

                    await CopyModFilesAsync(subDir, destDir);
                }
            }
            catch (Exception ex)
            {
                LoggerService.LogError($"Failed to copy mod files from {sourceFolder}", ex);
                throw;
            }

            await Task.CompletedTask;
        }

        protected virtual void OnCompileProgress(string message)
        {
            LoggerService.LogCompile(message);
            CompileProgress?.Invoke(this, message);
        }
    }
}
