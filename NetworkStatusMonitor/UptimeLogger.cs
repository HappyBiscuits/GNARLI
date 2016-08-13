using System.Net.NetworkInformation;
using ConfigINI;
using log4net;

namespace GNARLI
{
    public class UptimeLogger
    {
        private readonly ILog log;
        private readonly Config<ConfigSection, ConfigSetting> config;
        private bool logPingReply =>  this.config.GetBoolSetting(ConfigSection.Logging, ConfigSetting.LogPingReply);
        private bool logActiveFail => this.config.GetBoolSetting(ConfigSection.Logging, ConfigSetting.LogActiveFail);

        public UptimeLogger(ILog log, Config<ConfigSection, ConfigSetting> config)
        {
            this.log = log;
            this.config = config;
        }

        public void LogPingReply(PingReply pingReply)
        {
            if (logPingReply)
            {
                if (pingReply.Status == IPStatus.Success)
                    log.Info($"{pingReply.Status.ToString()} : {pingReply.Address.MapToIPv4()} : {pingReply.RoundtripTime}");

                else
                    log.Info($"{pingReply.Status.ToString()}");
            }
        }

        public void LogActiveFail(FailPeriod activeFail)
        {
            if (logActiveFail)
            {
                log.Info($"Active Fail Time: {activeFail.FailTime} : Return Time {activeFail.ReturnTime}");
            }
        }
    }
}
