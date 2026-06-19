using System;
using System.IO;
using System.Threading.Tasks;

namespace NSC.Winlator.Services
{
    public class CompilerService
    {
        private readonly YacpkWrapper _yacpkWrapper;

        public CompilerService(string toolsFolder)
        {
            _yacpkWrapper = new YacpkWrapper(toolsFolder);
        }

        /// <summary>
        /// Compile mod to CPK format
        /// </summary>
        public async Task CompileModToCpk(string modFolder, string outputPath)
        {
            try
            {
                LoggerService.LogInfo($"Compiling mod: {modFolder}");
                
                string result = await _yacpkWrapper.CompileMod(modFolder, outputPath);
                
                LoggerService.LogInfo($"Compilation complete: {outputPath}");
                LoggerService.LogInfo(result);
            }
            catch (Exception ex)
            {
                LoggerService.LogError($"Compilation failed: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Extract CPK file
        /// </summary>
        public async Task ExtractCpk(string cpkFile, string outputFolder)
        {
            try
            {
                LoggerService.LogInfo($"Extracting CPK: {cpkFile}");
                
                string result = await _yacpkWrapper.ExtractCpk(cpkFile, outputFolder);
                
                LoggerService.LogInfo($"Extraction complete: {outputFolder}");
                LoggerService.LogInfo(result);
            }
            catch (Exception ex)
            {
                LoggerService.LogError($"Extraction failed: {ex.Message}", ex);
                throw;
            }
        }
    }
}
