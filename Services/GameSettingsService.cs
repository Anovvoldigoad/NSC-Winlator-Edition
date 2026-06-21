using System;
using System.Collections.Generic;
using NSC.Winlator.Models;

namespace NSC.Winlator.Services
{
    public class GameSettingsService
    {
        private string _settingsPath = string.Empty;
        private GameSettings _settings = new();
        private JsonSerializerSettings _jsonOptions = new() { WriteIndented = true };

        public void Initialize(string configFolder)
        {
            _settingsPath = Path.Combine(configFolder, "game.json");
            Load();
        }

        public void Load()
        {
            if (!File.Exists(_settingsPath))
            {
                _settings = new GameSettings();
                return;
            }

            try
            {
                string json = File.ReadAllText(_settingsPath);
                _settings = JsonConvert.Deserialize<GameSettings>(json) ?? new GameSettings();
                LoggerService.LogInfo("Loaded game settings");
            }
            catch (Exception ex)
            {
                LoggerService.LogError("Failed to load game settings", ex);
                _settings = new GameSettings();
            }
        }

        public void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_settingsPath) ?? string.Empty);
                string json = JsonConvert.Serialize(_settings, _jsonOptions);
                File.WriteAllText(_settingsPath, json);
                LoggerService.LogSuccess("Saved game settings");
            }
            catch (Exception ex)
            {
                LoggerService.LogError("Failed to save game settings", ex);
            }
        }

        public GameSettings GetSettings()
        {
            return _settings;
        }

        public void SetGameExecutable(string executablePath)
        {
            if (!File.Exists(executablePath))
            {
                LoggerService.LogWarning($"Game executable not found: {executablePath}");
                return;
            }

            _settings.GameExecutable = executablePath;
            _settings.GameDirectory = Path.GetDirectoryName(executablePath) ?? string.Empty;
            Save();
        }

        public void SetGameDirectory(string gameDirectory)
        {
            if (!Directory.Exists(gameDirectory))
            {
                LoggerService.LogWarning($"Game directory not found: {gameDirectory}");
                return;
            }

            _settings.GameDirectory = gameDirectory;
            Save();
        }

        public bool IsGameConfigured()
        {
            return !string.IsNullOrEmpty(_settings.GameExecutable) && 
                   File.Exists(_settings.GameExecutable) &&
                   !string.IsNullOrEmpty(_settings.GameDirectory) &&
                   Directory.Exists(_settings.GameDirectory);
        }
    }
}
