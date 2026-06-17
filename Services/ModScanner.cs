using System;
using System.Collections.Generic;
using NSC.Winlator.Models;

namespace NSC.Winlator.Services
{
    public class ModScanner
    {
        private ModConfigService _configService = new();

        public List<ModInfo> ScanMods(string modsFolder)
        {
            List<ModInfo> mods = new();

            if (!Directory.Exists(modsFolder))
            {
                LoggerService.LogWarning($"Mods folder does not exist: {modsFolder}");
                return mods;
            }

            try
            {
                string[] directories = Directory.GetDirectories(modsFolder);

                foreach (string modFolder in directories)
                {
                    try
                    {
                        ModInfo modInfo = _configService.ReadModConfig(modFolder);
                        mods.Add(modInfo);
                    }
                    catch (Exception ex)
                    {
                        LoggerService.LogWarning($"Failed to scan mod folder: {modFolder}");
                    }
                }

                LoggerService.LogInfo($"Scanned {mods.Count} mods from {modsFolder}");
            }
            catch (Exception ex)
            {
                LoggerService.LogError($"Error scanning mods folder: {modsFolder}", ex);
            }

            return mods;
        }

        public List<ModInfo> ScanModsRecursive(string modsFolder)
        {
            List<ModInfo> mods = new();

            if (!Directory.Exists(modsFolder))
                return mods;

            try
            {
                var modConfigFiles = Directory.GetFiles(modsFolder, "mod_config.ini", SearchOption.AllDirectories);

                foreach (string configPath in modConfigFiles)
                {
                    try
                    {
                        string modFolder = Path.GetDirectoryName(configPath) ?? string.Empty;
                        ModInfo modInfo = _configService.ReadModConfig(modFolder);
                        mods.Add(modInfo);
                    }
                    catch (Exception ex)
                    {
                        LoggerService.LogWarning($"Failed to parse mod config: {configPath}");
                    }
                }

                LoggerService.LogInfo($"Recursively scanned {mods.Count} mods from {modsFolder}");
            }
            catch (Exception ex)
            {
                LoggerService.LogError($"Error recursively scanning mods: {modsFolder}", ex);
            }

            return mods;
        }
    }
}
