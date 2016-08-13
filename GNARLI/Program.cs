using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Policy;
using System.Threading;
using ConfigINI;
using IniParser.Model;

namespace GNARLI
{


    class Program
    {


        static void Main(string[] args)
        {
            var ipAddressConfig = new DynamicConfig<IpAddressData>("IpAddress", SetIpAddressData, GetIpAddressData,
                new List<IpAddressData>()
                {
                new IpAddressData("Google DNS", IPAddress.Parse("8.8.8.8")),
                new IpAddressData("Level 3", IPAddress.Parse("4.2.2.2")),
                new IpAddressData("Open DNS", IPAddress.Parse("208.67.222.222")),
                });

            var config = new Config<ConfigSection, ConfigSetting>("Config.ini", new List<IConfigValue<ConfigSection, ConfigSetting>>()
            {
            new FloatConfigValue<ConfigSection, ConfigSetting>(ConfigSection.Monitor, ConfigSetting.Frequency, 3f),
            new IntConfigValue<ConfigSection, ConfigSetting>(ConfigSection.Monitor, ConfigSetting.TimeOut, 4000),
            new IntConfigValue<ConfigSection, ConfigSetting>(ConfigSection.Monitor, ConfigSetting.SleepPeriod, 1000),
            new BoolConfigValue<ConfigSection, ConfigSetting>(ConfigSection.Logging, ConfigSetting.LogPingReply, true),
            new BoolConfigValue<ConfigSection, ConfigSetting>(ConfigSection.Logging, ConfigSetting.LogActiveFail, true),
            }, new List<IDynamicConfig>()
            {
                ipAddressConfig,
            });
            config.LoadConfig();
            config.SaveConfig();
            var monitor = new UptimeMonitor(config, ipAddressConfig);
            new Thread(monitor.Monitor).Start();
            var ui = new UptimeMonitorUi(config, monitor);
            new Thread(ui.Update).Start();


        }

        private static IpAddressData SetIpAddressData(KeyDataCollection data)
        {
            IPAddress ip;
            if (IPAddress.TryParse(data["Address"], out ip))
            {
                return new IpAddressData(data["Name"], ip);
            }
            throw new InvalidCastException("Not a valid IpAddress");
        }

        private static void GetIpAddressData(IpAddressData addr, KeyDataCollection data )
        {
            data.AddKey("Name", addr.Name);
            data.AddKey("Address", addr.Ip.ToString());
        }
    }

    
}