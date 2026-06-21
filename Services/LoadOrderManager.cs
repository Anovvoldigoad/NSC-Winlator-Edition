using System;
using System.Collections.Generic;

namespace NSC.Winlator.Services
{
    public class LoadOrderManager
    {
        private string _loadOrderPath = string.Empty;
        private List<string> _loadOrder = new();
        private JsonSerializerSettings _jsonOptions = new() { Formatting = Newtonsoft.Json.Formatting.Indented };

        public void Initialize(string configFolder)
        {
            _loadOrderPath = Path.Combine(configFolder, "loadorder.json");
            Load();
        }

        public void Load()
        {
            _loadOrder.Clear();

            if (!File.Exists(_loadOrderPath))
            {
                LoggerService.LogInfo("Load order file not found, creating new");
                return;
            }

            try
            {
                string json = File.ReadAllText(_loadOrderPath);
                _loadOrder = JsonConvert.DeserializeObject<List<string>>(json) ?? new List<string>();
                LoggerService.LogInfo($"Loaded load order with {_loadOrder.Count} entries");
            }
            catch (Exception ex)
            {
                LoggerService.LogError("Failed to load load order", ex);
                _loadOrder = new List<string>();
            }
        }

        public void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_loadOrderPath) ?? string.Empty);
                string json = JsonConvert.SerializeObject(_loadOrder, _jsonOptions);
                File.WriteAllText(_loadOrderPath, json);
                LoggerService.LogInfo("Saved load order");
            }
            catch (Exception ex)
            {
                LoggerService.LogError("Failed to save load order", ex);
            }
        }

        public void SetLoadOrder(List<string> modNames)
        {
            _loadOrder = new List<string>(modNames);
            Save();
        }

        public List<string> GetLoadOrder()
        {
            return new List<string>(_loadOrder);
        }

        public void MoveUp(string modName)
        {
            int index = _loadOrder.IndexOf(modName);
            if (index > 0)
            {
                _loadOrder.RemoveAt(index);
                _loadOrder.Insert(index - 1, modName);
                Save();
                LoggerService.LogInfo($"Moved '{modName}' up in load order");
            }
        }

        public void MoveDown(string modName)
        {
            int index = _loadOrder.IndexOf(modName);
            if (index >= 0 && index < _loadOrder.Count - 1)
            {
                _loadOrder.RemoveAt(index);
                _loadOrder.Insert(index + 1, modName);
                Save();
                LoggerService.LogInfo($"Moved '{modName}' down in load order");
            }
        }

        public void AddMod(string modName)
        {
            if (!_loadOrder.Contains(modName))
            {
                _loadOrder.Add(modName);
                Save();
            }
        }

        public void RemoveMod(string modName)
        {
            if (_loadOrder.Contains(modName))
            {
                _loadOrder.Remove(modName);
                Save();
            }
        }

        public int GetPosition(string modName)
        {
            return _loadOrder.IndexOf(modName);
        }
    }
}
