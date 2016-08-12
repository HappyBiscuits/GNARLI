using System.Net.NetworkInformation;
using log4net;

namespace GNARLI
{
    public class UptimeLogger
    {
        private readonly ILog log;
        public bool logStatus { get; set; }

        public UptimeLogger(ILog log)
        {
            this.log = log;
            this.logStatus = true;
        }

        public void Log(PingReply pingReply)
        {
            if (logStatus)
            {
                if (pingReply.Status == IPStatus.Success)
                    log.Info($"{pingReply.Status.ToString()} : {pingReply.Address.MapToIPv4()} : {pingReply.RoundtripTime}");

                else
                    log.Info($"{pingReply.Status.ToString()} : {pingReply.Address.MapToIPv4()}");
            }
        }
    }
}
