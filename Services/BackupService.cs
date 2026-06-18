using System.Threading.Tasks;
using System;
using System.Collections.Generic;
namespace NSC.Winlator.Services
{
    public class BackupService
    {
        private string _backupFolder = string.Empty;

        public void Initialize(string backupFolder)
        {
            _backupFolder = backupFolder;
            
            if (!Directory.Exists(backupFolder))
                Directory.CreateDirectory(backupFolder);
        }

        public async Task<bool> BackupGameFiles(string gamePath)
        {
            if (!Directory.Exists(gamePath))
            {
                LoggerService.LogError($"Game path not found: {gamePath}");
                return false;
            }

            try
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string backupPath = Path.Combine(_backupFolder, timestamp);

                LoggerService.LogInfo($"Starting backup of {gamePath} to {backupPath}");

                await CopyDirectoryAsync(gamePath, backupPath);

                LoggerService.LogSuccess($"Backup created: {backupPath}");
                return true;
            }
            catch (Exception ex)
            {
                LoggerService.LogError($"Backup failed: {gamePath}", ex);
                return false;
            }
        }

        public async Task<bool> RestoreBackup(string backupPath, string gamePath)
        {
            if (!Directory.Exists(backupPath))
            {
                LoggerService.LogError($"Backup not found: {backupPath}");
                return false;
            }

            try
            {
                LoggerService.LogInfo($"Starting restore from {backupPath} to {gamePath}");

                // Remove current game files
                if (Directory.Exists(gamePath))
                {
                    Directory.Delete(gamePath, recursive: true);
                }

                // Copy backup files
                await CopyDirectoryAsync(backupPath, gamePath);

                LoggerService.LogSuccess($"Game restored from backup: {backupPath}");
                return true;
            }
            catch (Exception ex)
            {
                LoggerService.LogError($"Restore failed: {backupPath}", ex);
                return false;
            }
        }

        public List<string> GetAvailableBackups()
        {
            try
            {
                if (!Directory.Exists(_backupFolder))
                    return new List<string>();

                return Directory.GetDirectories(_backupFolder)
                    .Select(d => Path.GetFileName(d))
                    .OrderByDescending(d => d)
                    .ToList();
            }
            catch (Exception ex)
            {
                LoggerService.LogError("Failed to retrieve backups", ex);
                return new List<string>();
            }
        }

        public void DeleteBackup(string backupName)
        {
            try
            {
                string backupPath = Path.Combine(_backupFolder, backupName);
                if (Directory.Exists(backupPath))
                {
                    Directory.Delete(backupPath, recursive: true);
                    LoggerService.LogInfo($"Deleted backup: {backupName}");
                }
            }
            catch (Exception ex)
            {
                LoggerService.LogError($"Failed to delete backup: {backupName}", ex);
            }
        }

        private async Task CopyDirectoryAsync(string sourceDir, string destDir)
        {
            if (!Directory.Exists(destDir))
                Directory.CreateDirectory(destDir);

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(destDir, Path.GetFileName(file));
                File.Copy(file, destFile, overwrite: true);
            }

            foreach (string subDir in Directory.GetDirectories(sourceDir))
            {
                string destSubDir = Path.Combine(destDir, Path.GetFileName(subDir));
                await CopyDirectoryAsync(subDir, destSubDir);
            }

            await Task.CompletedTask;
        }
    }
}
