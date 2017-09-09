using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace HURTHax
{
    public static class BaseSettings
    {
        public const string DefaultSettingsName = "settings.bin";
        private static Settings _settings;

        public static Settings GetSettings => _settings ?? (_settings = FetchSettings());

        private static Settings FetchSettings()
        {
            Debug.Log("Trying to read settings..");
            try
            {
                using (var mem = new MemoryStream(File.ReadAllBytes(DefaultSettingsName)))
                {
                    var binary = new BinaryFormatter();
                    return binary.Deserialize(mem) as Settings;
                }
            }
            catch (Exception)
            {
                return GetDefault();
            }
        }

        private static Settings GetDefault()
        {
            return new Settings {ShowEspMenu = false, ShowAimbotMenu = false, IsDebug = false, Friends = new List<string>(), EspSettings = new EspSettings {DrawLootCrates = false, Range = 300f, DrawAnimals = false, DrawResouces = false, DrawPlayers = false, IsEnabled = false, DrawWrecks = false}, AimBotSettings = new AimBotSettings {IsEnabled = false, AimAtPlayers = false, AimAtAnimals = false, AimAtFriends = false}, AimbotMenuKeyCode = KeyCode.F6, EspMenuKeyCode = KeyCode.F5};
        }

        public static void SaveSettings()
        {
            using (var mem = new MemoryStream())
            {
                var binary = new BinaryFormatter();
                binary.Serialize(mem, _settings);
                File.WriteAllBytes(DefaultSettingsName, mem.ToArray());
            }
        }
    }


    [Serializable]
    public class Settings
    {
        public bool ShowAimbotMenu { get; set; }
        public bool ShowEspMenu { get; set; }
        public bool IsDebug { get; set; } = true;
        public EspSettings EspSettings { get; set; } = new EspSettings();
        public AimBotSettings AimBotSettings { get; set; } = new AimBotSettings();
        public List<string> Friends { get; set; } = new List<string>();
        public KeyCode AimbotMenuKeyCode { get; set; } = KeyCode.F6;
        public KeyCode EspMenuKeyCode { get; set; } = KeyCode.F5;
    }

    [Serializable]
    public class EspSettings
    {
        public bool DrawResouces { get; set; }
        public bool DrawAnimals { get; set; }
        public bool DrawOwnershipStakes { get; set; }
        public bool DrawWrecks { get; set; }
        public bool DrawLootCrates { get; set; }
        public bool DrawPlayers { get; set; }
        public bool IsEnabled { get; set; }
        public float Range { get; set; } = 300f;
    }

    [Serializable]
    public class AimBotSettings
    {
        public bool AimAtFriends { get; set; }
        public bool AimAtPlayers { get; set; }
        public bool AimAtAnimals { get; set; }
        public bool IsEnabled { get; set; }
    }
}