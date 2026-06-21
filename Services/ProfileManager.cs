using System;
using System.Collections.Generic;
using NSC.Winlator.Models;

namespace NSC.Winlator.Services
{
    public class ProfileManager
    {
        private string _profilesFolder = string.Empty;
        private JsonSerializerSettings _jsonOptions = new() { Formatting = Newtonsoft.Json.Formatting.Indented };

        public void Initialize(string profilesFolder)
        {
            _profilesFolder = profilesFolder;
            
            if (!Directory.Exists(profilesFolder))
                Directory.CreateDirectory(profilesFolder);
        }

        public ProfileInfo LoadProfile(string profileName)
        {
            string profilePath = GetProfilePath(profileName);

            if (!File.Exists(profilePath))
            {
                LoggerService.LogWarning($"Profile not found: {profileName}");
                return new ProfileInfo { Name = profileName };
            }

            try
            {
                string json = File.ReadAllText(profilePath);
                ProfileInfo profile = JsonConvert.DeserializeObject<ProfileInfo>(json) ?? new ProfileInfo { Name = profileName };
                profile.Name = profileName;
                LoggerService.LogInfo($"Loaded profile: {profileName}");
                return profile;
            }
            catch (Exception ex)
            {
                LoggerService.LogError($"Failed to load profile: {profileName}", ex);
                return new ProfileInfo { Name = profileName };
            }
        }

        public void SaveProfile(ProfileInfo profile)
        {
            string profilePath = GetProfilePath(profile.Name);

            try
            {
                string json = JsonConvert.Serialize(profile, _jsonOptions);
                File.WriteAllText(profilePath, json);
                LoggerService.LogSuccess($"Saved profile: {profile.Name}");
            }
            catch (Exception ex)
            {
                LoggerService.LogError($"Failed to save profile: {profile.Name}", ex);
            }
        }

        public void DeleteProfile(string profileName)
        {
            string profilePath = GetProfilePath(profileName);

            try
            {
                if (File.Exists(profilePath))
                {
                    File.Delete(profilePath);
                    LoggerService.LogInfo($"Deleted profile: {profileName}");
                }
            }
            catch (Exception ex)
            {
                LoggerService.LogError($"Failed to delete profile: {profileName}", ex);
            }
        }

        public List<string> GetAllProfiles()
        {
            try
            {
                if (!Directory.Exists(_profilesFolder))
                    return new List<string>();

                return Directory.GetFiles(_profilesFolder, "*.json")
                    .Select(f => Path.GetFileNameWithoutExtension(f))
                    .ToList();
            }
            catch (Exception ex)
            {
                LoggerService.LogError("Failed to retrieve profiles", ex);
                return new List<string>();
            }
        }

        public bool ProfileExists(string profileName)
        {
            return File.Exists(GetProfilePath(profileName));
        }

        private string GetProfilePath(string profileName)
        {
            return Path.Combine(_profilesFolder, $"{profileName}.json");
        }
    }
}
