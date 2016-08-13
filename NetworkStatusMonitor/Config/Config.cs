using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using IniParser;
using IniParser.Model;

namespace GNARLI
{


    public class Config<TSection, TValues> where TSection : struct, IComparable where TValues : struct, IComparable
    {
        private readonly string _configPath;
        public List<IConfigValue<TSection, TValues>> Settings;
        public List<IDynamicConfig> DynamicConfigs; 

        public Config(string path, List<IConfigValue<TSection, TValues>> defaults, List<IDynamicConfig> dynamicConfigs = null)
        {
            _configPath = path;
            Settings = defaults;
            if (dynamicConfigs == null) DynamicConfigs = new List<IDynamicConfig>();
            else DynamicConfigs = dynamicConfigs;
        }

        private bool HasSetting(TSection section, TValues setting)
        {
            return Settings.Any(x => x.GetSection().Equals(section) && x.GetSetting().Equals(setting));
        }

        private IConfigValue<TSection, TValues> GetSetting(TSection section, TValues setting)
        {
            if (!HasSetting(section, setting)) throw new InvalidDataException("Setting is Missing");
            return Settings.Find(x => x.GetSection().Equals(section) && x.GetSetting().Equals(setting));
        }

        public int GetIntSetting(TSection section, TValues setting)
        {
            return ((IntConfigValue<TSection, TValues>)GetSetting(section, setting)).Value;
        }

        public float GetFloatSetting(TSection section, TValues setting)
        {
            return ((FloatConfigValue<TSection, TValues>)GetSetting(section, setting)).Value;
        }

        public bool GetBoolSetting(TSection section, TValues setting)
        {
            return ((BoolConfigValue<TSection, TValues>)GetSetting(section, setting)).Value;
        }

        public void SetIntSetting(TSection section, TValues setting, int value)
        {
            ((IntConfigValue<TSection, TValues>)GetSetting(section, setting)).Value = value;
            SaveConfig();
        }

        public void SetFloatSetting(TSection section, TValues setting, float value)
        {
            ((FloatConfigValue<TSection, TValues>)GetSetting(section, setting)).Value = value;
            SaveConfig();
        }

        public void SetBoolSetting(TSection section, TValues setting, bool value)
        {
            ((BoolConfigValue<TSection, TValues>)GetSetting(section, setting)).Value = value;
            SaveConfig();
        }

        public void LoadConfig()
        {
            var parser = new FileIniDataParser();
            var data = new IniData();
            if (File.Exists(_configPath))
            {
                data = parser.ReadFile(_configPath);
                DynamicConfigs.ForEach(x => x.LoadData(data));
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
            DynamicConfigs.ForEach(x => x.SaveData(data));
            var parser = new FileIniDataParser();
            parser.WriteFile(_configPath, data);
        }

        public IDynamicConfig GetDynamicConfig(string groupName)
        {
          if(DynamicConfigs.All(x => x.GetGroupName() != groupName))  throw new InvalidDataException("GroupName is not present in the dynamic config list");
            return DynamicConfigs.Find(x => x.GetGroupName() == groupName);
        }
    }
}