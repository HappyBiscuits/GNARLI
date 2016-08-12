using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
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

        public List<IpAddressData> IpAddressData = new List<IpAddressData>()
        {
            new IpAddressData("Google DNS", IPAddress.Parse("8.8.8.8")),
            new IpAddressData("Level 3",IPAddress.Parse("4.2.2.2")),
            new IpAddressData("Open DNS",IPAddress.Parse("208.67.222.222")),
        }; 

        private void LoadIpAddressDatas(IniData data)
        {
            var loop = true;
            var i = 0;
            var addrs = new List<IpAddressData>();
            do
            {
                var dat = "IPAddress" + i;
                if (data.Sections.ContainsSection(dat))
                {
                    addrs.Add(GetIpAddressData(data[dat]));
                    i++;
                }
                else
                {
                    loop = false;
                }
            } while (loop);
            if (addrs.Count > 0)
            {
                IpAddressData = addrs;
            }
            

        }

        private void SaveIpAddressDatas(IniData data)
        {
            for (var i = 0; i < IpAddressData.Count; i++)
            {
                var dat = "IPAddress" + i;
                data.Sections.AddSection(dat);
                data[dat].AddKey("Name", IpAddressData[i].Name);
                data[dat].AddKey("Address", IpAddressData[i].Ip.ToString());
            }
        }
        private IpAddressData GetIpAddressData(KeyDataCollection data)
        {

            if (data.ContainsKey("Name") && data.ContainsKey("Address"))
            {
                IPAddress ip;
                if (IPAddress.TryParse(data["Address"], out ip))
                {
                    return new IpAddressData(data["Name"], ip);
                }
                else
                {
                    throw new InvalidDataException(data["Address"] + " is not a valid address");
                }

            }
            else
            {
                throw new InvalidDataException("Missing Ip Address Data");
            }

        }

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
                LoadIpAddressDatas(data);
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
            SaveIpAddressDatas(data);
            var parser = new FileIniDataParser();
            parser.WriteFile(_configPath, data);
        }
    }
}