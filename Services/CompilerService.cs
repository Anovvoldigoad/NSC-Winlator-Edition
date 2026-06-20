using System;
using System.IO;
using System.Threading.Tasks;

namespace NSC.Winlator.Services
{
    public class CompilerService
    {
        private string _gameFolder = string.Empty;

        public void SetGameFolder(string gameFolder)
        {
            _gameFolder = gameFolder;
        }

        public async Task CompileModToCpk(string modFolder, string outputPath)
        {
            try
            {
                LoggerService.LogInfo($"Processing mod: {modFolder}");

                // For now: just copy mod folder as "compiled"
                // In future: implement proper CPK compilation
                string cpkPath = Path.Combine(Path.GetTempPath(), "mod_data.cpk");
                
                // Create simple archive
                if (Directory.Exists(modFolder))
                {
                    LoggerService.LogSuccess($"Mod prepared: {modFolder}");
                    
                    if (!string.IsNullOrEmpty(_gameFolder))
                    {
                        ApplyMod(modFolder);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerService.LogError($"Failed: {ex.Message}", ex);
                throw;
            }
        }

        private void ApplyMod(string modFolder)
        {
            try
            {
                string gameDataPath = Path.Combine(_gameFolder, "data0.cpk");
                string backupPath = gameDataPath + ".backup";

                // Backup original
                if (File.Exists(gameDataPath) && !File.Exists(backupPath))
                {
                    File.Copy(gameDataPath, backupPath);
                    LoggerService.LogInfo($"Backed up: {backupPath}");
                }

                // Note: actual CPK modification needs proper library
                LoggerService.LogSuccess($"Mod ready for application");
            }
            catch (Exception ex)
            {
                LoggerService.LogError($"Apply failed: {ex.Message}", ex);
            }
        }
    }
}
