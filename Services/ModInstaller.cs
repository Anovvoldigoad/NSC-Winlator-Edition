using System;
using System.Collections.Generic;
using System.IO.Compression;
using SharpCompress.Archives;
using SharpCompress.Common;

namespace NSC.Winlator.Services
{
    public class ModInstaller
    {
        private static readonly string[] SupportedExtensions = { ".nsc", ".ensc", ".uns", ".unse", ".zip", ".7z", ".rar", ".nus4" };

        public bool IsModPackage(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            return SupportedExtensions.Contains(extension);
        }

        public async Task<bool> InstallMod(string packagePath, string modsFolder)
        {
            if (!File.Exists(packagePath))
            {
                LoggerService.LogError($"Package file not found: {packagePath}");
                return false;
            }

            string extension = Path.GetExtension(packagePath).ToLower();

            try
            {
                LoggerService.LogInstall($"Installing mod from: {packagePath}");

                string tempFolder = Path.Combine(Path.GetTempPath(), $"nsc_install_{Guid.NewGuid()}");
                Directory.CreateDirectory(tempFolder);

                // Extract based on format
                if (extension == ".zip")
                {
                    await ExtractZip(packagePath, tempFolder);
                }
                else if (extension == ".7z" || extension == ".rar" || extension == ".nsc" || extension == ".ensc" || extension == ".uns" || extension == ".unse" || extension == ".nus4")
                {
                    ExtractSharpCompress(packagePath, tempFolder);
                }
                else
                {
                    LoggerService.LogError($"Unsupported format: {extension}");
                    return false;
                }

                // Find mod_config.ini in extracted files
                string configPath = FindModConfig(tempFolder);
                if (configPath == null)
                {
                    LoggerService.LogError("No mod_config.ini found in package");
                    CleanupTemp(tempFolder);
                    return false;
                }

                // Determine mod folder name
                string modName = GetModName(configPath);
                string targetModFolder = Path.Combine(modsFolder, modName);

                // Avoid conflicts
                if (Directory.Exists(targetModFolder))
                {
                    string backup = targetModFolder + "_backup_" + DateTime.Now.Ticks;
                    Directory.Move(targetModFolder, backup);
                    LoggerService.LogInfo($"Backed up existing mod to: {backup}");
                }

                // Move extracted content to mods folder
                string sourceFolder = Path.GetDirectoryName(configPath) ?? tempFolder;
                Directory.Move(sourceFolder, targetModFolder);

                LoggerService.LogSuccess($"Successfully installed mod: {modName}");
                CleanupTemp(tempFolder);
                return true;
            }
            catch (Exception ex)
            {
                LoggerService.LogError($"Failed to install mod: {packagePath}", ex);
                return false;
            }
        }

        public bool UninstallMod(string modFolder)
        {
            try
            {
                if (!Directory.Exists(modFolder))
                {
                    LoggerService.LogWarning($"Mod folder not found: {modFolder}");
                    return false;
                }

                // Backup before deletion
                string backupName = Path.GetFileName(modFolder) + "_uninstall_" + DateTime.Now.Ticks;
                string backupPath = Path.Combine(Path.GetTempPath(), backupName);
                Directory.Move(modFolder, backupPath);

                LoggerService.LogInfo($"Uninstalled mod from: {modFolder}");
                LoggerService.LogInfo($"Backup created at: {backupPath}");
                return true;
            }
            catch (Exception ex)
            {
                LoggerService.LogError($"Failed to uninstall mod: {modFolder}", ex);
                return false;
            }
        }

        private async Task ExtractZip(string zipPath, string extractPath)
        {
            using (ZipArchive archive = ZipFile.OpenRead(zipPath))
            {
                archive.ExtractToDirectory(extractPath, overwriteFiles: true);
            }
            await Task.CompletedTask;
        }

        private void ExtractSharpCompress(string archivePath, string extractPath)
        {
            using (var archive = ArchiveFactory.Open(archivePath))
            {
                foreach (var entry in archive.Entries.Where(e => !e.IsDirectory))
                {
                    entry.WriteToDirectory(extractPath, new ExtractionOptions { ExtractFullPath = true, Overwrite = OverwriteMode.Always });
                }
            }
        }

        private string? FindModConfig(string rootFolder)
        {
            try
            {
                var configFiles = Directory.GetFiles(rootFolder, "mod_config.ini", SearchOption.AllDirectories);
                return configFiles.FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        private string GetModName(string configPath)
        {
            try
            {
                var iniParser = new IniParser();
                iniParser.Load(configPath);
                string? modName = iniParser.GetValue("ModManager", "ModName");
                return modName ?? Path.GetFileNameWithoutExtension(Path.GetDirectoryName(configPath));
            }
            catch
            {
                return Path.GetFileNameWithoutExtension(Path.GetDirectoryName(configPath));
            }
        }

        private void CleanupTemp(string folder)
        {
            try
            {
                if (Directory.Exists(folder))
                    Directory.Delete(folder, recursive: true);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }
}
