using NSC.Winlator.Models;

namespace NSC.Winlator.Services
{
    public class ModConfigService
    {
        private IniParser _iniParser = new();

        public ModInfo ReadModConfig(string modFolder)
        {
            string configPath = Path.Combine(modFolder, "mod_config.ini");
            
            var modInfo = new ModInfo
            {
                ModFolder = modFolder,
                LastModified = Directory.GetLastAccessTime(modFolder)
            };

            if (!File.Exists(configPath))
            {
                modInfo.Name = Path.GetFileName(modFolder);
                return modInfo;
            }

            try
            {
                _iniParser.Load(configPath);

                modInfo.Name = _iniParser.GetValue("ModManager", "ModName", Path.GetFileName(modFolder)) ?? modFolder;
                modInfo.Author = _iniParser.GetValue("ModManager", "Author", "Unknown") ?? "Unknown";
                modInfo.Version = _iniParser.GetValue("ModManager", "Version", "1.0") ?? "1.0";
                modInfo.Description = _iniParser.GetValue("ModManager", "Description", "") ?? "";
                
                string enableModStr = _iniParser.GetValue("ModManager", "EnableMod", "true") ?? "true";
                modInfo.Enabled = enableModStr.ToLower() == "true";

                // Check for icon
                string[] iconExtensions = { ".png", ".jpg", ".jpeg", ".bmp" };
                foreach (string ext in iconExtensions)
                {
                    string iconPath = Path.Combine(modFolder, $"icon{ext}");
                    if (File.Exists(iconPath))
                    {
                        modInfo.IconPath = iconPath;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerService.LogError($"Failed to read mod config from {configPath}", ex);
            }

            return modInfo;
        }

        public void WriteModConfig(ModInfo modInfo)
        {
            string configPath = Path.Combine(modInfo.ModFolder, "mod_config.ini");

            try
            {
                _iniParser.Load(configPath);

                _iniParser.SetValue("ModManager", "ModName", modInfo.Name);
                _iniParser.SetValue("ModManager", "Author", modInfo.Author);
                _iniParser.SetValue("ModManager", "Version", modInfo.Version);
                _iniParser.SetValue("ModManager", "Description", modInfo.Description);
                _iniParser.SetValue("ModManager", "EnableMod", modInfo.Enabled ? "true" : "false");

                _iniParser.Save();
                LoggerService.LogInfo($"Saved config for mod: {modInfo.Name}");
            }
            catch (Exception ex)
            {
                LoggerService.LogError($"Failed to write mod config to {configPath}", ex);
            }
        }
    }
}
