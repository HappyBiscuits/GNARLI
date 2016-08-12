using System.Collections.Generic;
using System.IO;
using System.Linq;
using IniParser;
using IniParser.Model;

namespace GNARLI
{
    public class Config
    {
        private string _configPath = "Config.ini";
        public List<IConfigValue> Settings = new List<IConfigValue>()
        {
            new FloatConfigValue(ConfigSection.Monitor, ConfigSetting.Frequency, 3f),
            new IntConfigValue(ConfigSection.Monitor, ConfigSetting.TimeOut, 4000),
            new IntConfigValue(ConfigSection.Monitor, ConfigSetting.SleepPeriod, 1000),

        };

        public int GetIntSetting(ConfigSection section, ConfigSetting setting)
        {
            if (!Settings.Any(x => x.GetSection() == section && x.GetSetting() == setting)) throw new InvalidDataException("Setting is Missing");
            return ((IntConfigValue) Settings.Find(x => x.GetSection() == section && x.GetSetting() == setting)).Value;
        }
        public float GetFloatSetting(ConfigSection section, ConfigSetting setting)
        {
            if (!Settings.Any(x => x.GetSection() == section && x.GetSetting() == setting)) throw new InvalidDataException("Setting is Missing");
            return ((FloatConfigValue)Settings.Find(x => x.GetSection() == section && x.GetSetting() == setting)).Value;
        }
        public void SetIntSetting(ConfigSection section, ConfigSetting setting, int value)
        {
            if (!Settings.Any(x => x.GetSection() == section && x.GetSetting() == setting)) throw new InvalidDataException("Setting is Missing");
            ((IntConfigValue) Settings.Find(x => x.GetSection() == section && x.GetSetting() == setting)).Value = value;
            SaveConfig();
        }
        public void SetFloatSetting(ConfigSection section, ConfigSetting setting, float value)
        {
            if (!Settings.Any(x => x.GetSection() == section && x.GetSetting() == setting)) throw new InvalidDataException("Setting is Missing");
            ((FloatConfigValue)Settings.Find(x => x.GetSection() == section && x.GetSetting() == setting)).Value = value;
            SaveConfig();
        }

        public void LoadConfig()
        {
            var parser = new FileIniDataParser();
            var data = new IniData();
            if (File.Exists(_configPath))
            {
                data = parser.ReadFile(_configPath);

            }
            foreach (var setting in Settings)
            {
                setting.ReadData(data);
            }

        }

        public void SaveConfig()
        {
            var data = new IniData();
            foreach (var setting in Settings)
            {
                setting.WriteData(data);
            }
            var parser = new FileIniDataParser();
            parser.WriteFile(_configPath, data);
        }
    }
}