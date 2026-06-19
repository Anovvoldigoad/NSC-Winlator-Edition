using System;
using System.IO;
using System.Threading.Tasks;
using LibCPK;

namespace NSC.Winlator.Services
{
    public class CompilerService
    {
        public async Task CompileModToCpk(string modFolder, string outputPath)
        {
            try
            {
                LoggerService.LogInfo($"Compiling mod: {modFolder}");

                // Use LibCPK to create CPK from mod folder
                var cpkBuilder = new CpkBuilder();
                
                foreach (var file in Directory.GetFiles(modFolder, "*", SearchOption.AllDirectories))
                {
                    string relativePath = Path.GetRelativePath(modFolder, file);
                    cpkBuilder.AddFile(file, relativePath);
                }

                cpkBuilder.Build(outputPath);
                LoggerService.LogSuccess($"CPK created: {outputPath}");
            }
            catch (Exception ex)
            {
                LoggerService.LogError($"Compilation failed: {ex.Message}", ex);
                throw;
            }
        }
    }
}
