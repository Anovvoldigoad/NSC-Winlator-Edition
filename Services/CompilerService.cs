using System;
using System.IO;
using System.Threading.Tasks;
using LibCPK;

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
                LoggerService.LogInfo($"Compiling mod: {modFolder}");

                var cpkBuilder = new CpkBuilder();
                foreach (var file in Directory.GetFiles(modFolder, "*", SearchOption.AllDirectories))
                {
                    if (Path.GetFileName(file) == "mod_config.ini") continue;
                    
                    string relativePath = Path.GetRelativePath(modFolder, file);
                    cpkBuilder.AddFile(file, relativePath);
                }

                cpkBuilder.Build(outputPath);
                LoggerService.LogSuccess($"CPK compiled: {outputPath}");

                // APPLY to game
                if (!string.IsNullOrEmpty(_gameFolder))
                {
                    ApplyCompiledMod(outputPath);
                }
            }
            catch (Exception ex)
            {
                LoggerService.LogError($"Compilation failed: {ex.Message}", ex);
                throw;
            }
        }

        private void ApplyCompiledMod(string compiledCpkPath)
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

                // Replace with compiled mod
                File.Copy(compiledCpkPath, gameDataPath, overwrite: true);
                LoggerService.LogSuccess($"Applied: {gameDataPath}");
            }
            catch (Exception ex)
            {
                LoggerService.LogError($"Apply failed: {ex.Message}", ex);
                throw;
            }
        }
    }
}
