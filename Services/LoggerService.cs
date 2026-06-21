using System.IO;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace NSC.Winlator.Services
{
    public static class LoggerService
    {
        private static string _logsFolder = string.Empty;
        private static ConcurrentBag<string> _logBuffer = new();
        private static FileStream? _appLogStream;
        private static object _lockObject = new();

        public static event EventHandler<LogEventArgs>? LogAdded;

        public class LogEventArgs : EventArgs
        {
            public string Message { get; set; } = string.Empty;
            public LogLevel Level { get; set; }
        }

        public enum LogLevel
        {
            Info,
            Warning,
            Error,
            Success
        }

        public static void Initialize(string logsFolder)
        {
            _logsFolder = logsFolder;
            
            if (!Directory.Exists(logsFolder))
                Directory.CreateDirectory(logsFolder);

            try
            {
                string appLogPath = Path.Combine(logsFolder, "application.log");
                _appLogStream = new FileStream(appLogPath, FileMode.Append, FileAccess.Write, FileShare.Read);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize logger: {ex.Message}");
            }
        }

        public static void LogInfo(string message)
        {
            Log(message, LogLevel.Info, "application.log");
        }

        public static void LogWarning(string message)
        {
            Log(message, LogLevel.Warning, "application.log");
        }

        public static void LogError(string message, Exception? ex = null)
        {
            string fullMessage = ex != null ? $"{message}\n{ex}" : message;
            Log(fullMessage, LogLevel.Error, "application.log");
        }

        public static void LogSuccess(string message)
        {
            Log(message, LogLevel.Success, "application.log");
        }

        public static void LogCompile(string message)
        {
            Log(message, LogLevel.Info, "compile.log");
        }

        public static void LogInstall(string message)
        {
            Log(message, LogLevel.Info, "install.log");
        }

        public static void LogLaunch(string message)
        {
            Log(message, LogLevel.Info, "launch.log");
        }

        private static void Log(string message, LogLevel level, string logFile)
        {
            lock (_lockObject)
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string logMessage = $"[{timestamp}] [{level}] {message}";
                
                _logBuffer.Add(logMessage);
                LogAdded?.Invoke(null, new LogEventArgs { Message = message, Level = level });

                try
                {
                    string logPath = Path.Combine(_logsFolder, logFile);
                    File.AppendAllText(logPath, logMessage + Environment.NewLine);
                }
                catch
                {
                    // Silently fail if unable to write
                }
            }
        }

        public static List<string> GetRecentLogs(int count = 100)
        {
            return _logBuffer.Skip(Math.Max(0, _logBuffer.Count - count)).ToList();
        }

        public static void ClearBuffer()
        {
            _logBuffer = new ConcurrentBag<string>();
        }

        public static void Dispose()
        {
            _appLogStream?.Dispose();
        }
    }
}
