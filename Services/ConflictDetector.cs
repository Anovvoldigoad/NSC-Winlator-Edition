using System;
using System.Collections.Generic;
using NSC.Winlator.Models;

namespace NSC.Winlator.Services
{
    public class ConflictReport
    {
        public string FilePath { get; set; } = string.Empty;
        public List<string> ConflictingMods { get; set; } = new();
    }

    public class ConflictDetector
    {
        private static readonly string[] MonitoredPaths = { "Resources", "Characters", "CPKs" };

        public List<ConflictReport> DetectConflicts(List<ModInfo> enabledMods)
        {
            var conflicts = new List<ConflictReport>();
            var fileMap = new Dictionary<string, List<string>>(); // file path -> mod names

            foreach (var mod in enabledMods.Where(m => m.Enabled))
            {
                try
                {
                    ScanModFiles(mod, fileMap);
                }
                catch (Exception ex)
                {
                    LoggerService.LogWarning($"Failed to scan mod {mod.Name} for conflicts");
                }
            }

            // Find conflicts
            foreach (var kvp in fileMap.Where(f => f.Value.Count > 1))
            {
                conflicts.Add(new ConflictReport
                {
                    FilePath = kvp.Key,
                    ConflictingMods = kvp.Value
                });
            }

            if (conflicts.Count > 0)
            {
                LoggerService.LogWarning($"Detected {conflicts.Count} file conflicts between mods");
            }

            return conflicts;
        }

        private void ScanModFiles(ModInfo mod, Dictionary<string, List<string>> fileMap)
        {
            foreach (string pathPattern in MonitoredPaths)
            {
                string fullPath = Path.Combine(mod.ModFolder, pathPattern);
                
                if (!Directory.Exists(fullPath))
                    continue;

                var files = Directory.GetFiles(fullPath, "*.*", SearchOption.AllDirectories);
                
                foreach (string file in files)
                {
                    // Create relative path
                    string relativePath = file.Replace(mod.ModFolder + "\\", "");

                    if (!fileMap.ContainsKey(relativePath))
                        fileMap[relativePath] = new List<string>();

                    if (!fileMap[relativePath].Contains(mod.Name))
                        fileMap[relativePath].Add(mod.Name);
                }
            }
        }

        public string GenerateConflictReport(List<ConflictReport> conflicts)
        {
            if (conflicts.Count == 0)
                return "No conflicts detected.";

            var report = new System.Text.StringBuilder();
            report.AppendLine($"Conflict Report - {conflicts.Count} conflicts found:");
            report.AppendLine();

            foreach (var conflict in conflicts.OrderBy(c => c.FilePath))
            {
                report.AppendLine($"File: {conflict.FilePath}");
                report.AppendLine($"  Conflicting Mods: {string.Join(", ", conflict.ConflictingMods)}");
                report.AppendLine();
            }

            return report.ToString();
        }
    }
}
