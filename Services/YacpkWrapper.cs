using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace NSC.Winlator.Services
{
    public class YacpkWrapper
    {
        private readonly string _yacpkPath;

        public YacpkWrapper(string toolsFolder)
        {
            _yacpkPath = Path.Combine(toolsFolder, "YACpkTool.exe");
            if (!File.Exists(_yacpkPath))
            {
                throw new FileNotFoundException($"YACpkTool.exe not found at {_yacpkPath}");
            }
        }

        /// <summary>
        /// Compile mod folder to CPK file
        /// </summary>
        public async Task<string> CompileMod(string inputFolder, string outputCpk)
        {
            if (!Directory.Exists(inputFolder))
                throw new DirectoryNotFoundException($"Input folder not found: {inputFolder}");

            return await RunYacpkCommand($"-P -i \"{inputFolder}\" -o \"{outputCpk}\"");
        }

        /// <summary>
        /// Extract CPK file to folder
        /// </summary>
        public async Task<string> ExtractCpk(string inputCpk, string outputFolder)
        {
            if (!File.Exists(inputCpk))
                throw new FileNotFoundException($"CPK file not found: {inputCpk}");

            return await RunYacpkCommand($"-X -i \"{inputCpk}\" -o \"{outputFolder}\"");
        }

        /// <summary>
        /// List CPK contents
        /// </summary>
        public async Task<string> ListCpkContents(string inputCpk)
        {
            if (!File.Exists(inputCpk))
                throw new FileNotFoundException($"CPK file not found: {inputCpk}");

            return await RunYacpkCommand($"-L -i \"{inputCpk}\"");
        }

        private async Task<string> RunYacpkCommand(string arguments)
        {
            var psi = new ProcessStartInfo
            {
                FileName = _yacpkPath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(psi))
            {
                if (process == null)
                    throw new InvalidOperationException("Failed to start YACpkTool process");

                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();

                await Task.Run(() => process.WaitForExit());

                if (process.ExitCode != 0)
                {
                    throw new InvalidOperationException(
                        $"YACpkTool failed with exit code {process.ExitCode}\n{error}");
                }

                return output;
            }
        }
    }
}
