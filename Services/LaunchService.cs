using NSC.Winlator.Models;

namespace NSC.Winlator.Services
{
    public class LaunchService
    {
        public bool LaunchGame(GameSettings gameSettings)
        {
            if (!File.Exists(gameSettings.GameExecutable))
            {
                LoggerService.LogError($"Game executable not found: {gameSettings.GameExecutable}");
                return false;
            }

            try
            {
                LoggerService.LogLaunch($"Launching game: {gameSettings.GameExecutable}");

                ProcessStartInfo psi = new()
                {
                    FileName = gameSettings.GameExecutable,
                    WorkingDirectory = gameSettings.GameDirectory,
                    UseShellExecute = true,
                    CreateNoWindow = false
                };

                using (Process process = Process.Start(psi))
                {
                    if (process == null)
                    {
                        LoggerService.LogError("Failed to start game process");
                        return false;
                    }

                    LoggerService.LogSuccess($"Game launched successfully (PID: {process.Id})");
                    return true;
                }
            }
            catch (Exception ex)
            {
                LoggerService.LogError("Failed to launch game", ex);
                return false;
            }
        }

        public bool LaunchGameWithArguments(GameSettings gameSettings, string arguments)
        {
            if (!File.Exists(gameSettings.GameExecutable))
            {
                LoggerService.LogError($"Game executable not found: {gameSettings.GameExecutable}");
                return false;
            }

            try
            {
                LoggerService.LogLaunch($"Launching game with arguments: {arguments}");

                ProcessStartInfo psi = new()
                {
                    FileName = gameSettings.GameExecutable,
                    WorkingDirectory = gameSettings.GameDirectory,
                    Arguments = arguments,
                    UseShellExecute = true,
                    CreateNoWindow = false
                };

                using (Process process = Process.Start(psi))
                {
                    if (process == null)
                    {
                        LoggerService.LogError("Failed to start game process");
                        return false;
                    }

                    LoggerService.LogSuccess($"Game launched successfully (PID: {process.Id})");
                    return true;
                }
            }
            catch (Exception ex)
            {
                LoggerService.LogError("Failed to launch game", ex);
                return false;
            }
        }
    }
}
