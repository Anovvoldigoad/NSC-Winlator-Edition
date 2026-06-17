using System;
using System.Collections.Generic;
namespace NSC.Winlator.Services
{
    public class YacpkWrapper
    {
        private string _yacpkPath = string.Empty;

        public void SetToolPath(string toolPath)
        {
            _yacpkPath = toolPath;
            
            if (!File.Exists(toolPath))
            {
                LoggerService.LogWarning($"YACpk tool not found: {toolPath}");
            }
        }

        public bool ValidateTool()
        {
            if (string.IsNullOrEmpty(_yacpkPath))
            {
                LoggerService.LogWarning("YACpk tool path not set");
                return false;
            }

            if (!File.Exists(_yacpkPath))
            {
                LoggerService.LogError($"YACpk tool not found: {_yacpkPath}");
                return false;
            }

            LoggerService.LogInfo($"YACpk tool validated: {_yacpkPath}");
            return true;
        }

        public bool ExecuteCompile(string inputPath, string outputPath, string parameters = "")
        {
            if (!ValidateTool())
                return false;

            if (!Directory.Exists(inputPath))
            {
                LoggerService.LogError($"Input path not found: {inputPath}");
                return false;
            }

            try
            {
                LoggerService.LogCompile($"Starting YACpk compile: {inputPath}");

                ProcessStartInfo psi = new()
                {
                    FileName = _yacpkPath,
                    Arguments = $"\"{inputPath}\" \"{outputPath}\" {parameters}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    if (process == null)
                    {
                        LoggerService.LogError("Failed to start YACpk process");
                        return false;
                    }

                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (process.ExitCode == 0)
                    {
                        LoggerService.LogCompile($"YACpk compile completed successfully");
                        if (!string.IsNullOrEmpty(output))
                            LoggerService.LogCompile(output);
                        return true;
                    }
                    else
                    {
                        LoggerService.LogError($"YACpk compile failed with code {process.ExitCode}");
                        if (!string.IsNullOrEmpty(error))
                            LoggerService.LogError(error);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerService.LogError("Failed to execute YACpk compile", ex);
                return false;
            }
        }
    }
}
