using System;
using System.Collections.Generic;
using System.Text;

namespace NSC.Winlator.Services
{
    public class IniParser
    {
        private Dictionary<string, Dictionary<string, string>> _data = new();
        private string _filePath = string.Empty;

        public void Load(string filePath)
        {
            _filePath = filePath;
            _data.Clear();

            if (!File.Exists(filePath))
                return;

            try
            {
                string[] lines = File.ReadAllLines(filePath);
                string currentSection = "General";

                foreach (string line in lines)
                {
                    string trimmedLine = line.Trim();

                    if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith(";"))
                        continue;

                    if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                    {
                        currentSection = trimmedLine.Substring(1, trimmedLine.Length - 2);
                        if (!_data.ContainsKey(currentSection))
                            _data[currentSection] = new Dictionary<string, string>();
                    }
                    else if (trimmedLine.Contains("="))
                    {
                        int equalsIndex = trimmedLine.IndexOf("=");
                        string key = trimmedLine.Substring(0, equalsIndex).Trim();
                        string value = trimmedLine.Substring(equalsIndex + 1).Trim();

                        if (!_data.ContainsKey(currentSection))
                            _data[currentSection] = new Dictionary<string, string>();

                        _data[currentSection][key] = value;
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerService.LogError($"Failed to load INI file: {filePath}", ex);
            }
        }

        public string? GetValue(string section, string key, string? defaultValue = null)
        {
            if (_data.TryGetValue(section, out var sectionData))
            {
                if (sectionData.TryGetValue(key, out var value))
                    return value;
            }
            return defaultValue;
        }

        public void SetValue(string section, string key, string value)
        {
            if (!_data.ContainsKey(section))
                _data[section] = new Dictionary<string, string>();

            _data[section][key] = value;
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(_filePath))
                return;

            try
            {
                StringBuilder sb = new();

                foreach (var section in _data)
                {
                    sb.AppendLine($"[{section.Key}]");
                    foreach (var kvp in section.Value)
                    {
                        sb.AppendLine($"{kvp.Key}={kvp.Value}");
                    }
                    sb.AppendLine();
                }

                File.WriteAllText(_filePath, sb.ToString());
            }
            catch (Exception ex)
            {
                LoggerService.LogError($"Failed to save INI file: {_filePath}", ex);
            }
        }

        public bool Exists(string section, string key)
        {
            return _data.ContainsKey(section) && _data[section].ContainsKey(key);
        }
    }
}
