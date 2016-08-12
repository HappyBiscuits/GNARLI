using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace GNARLI
{
    public class UptimeSettings
    {
        
    }
    public class UpTimeLog
    {
        public IpAddressData Address;
        public List<PingResult> Pings = new List<PingResult>();

        public void PopulateTest()
        {
            var rand = new Random();
            for (var i = 0; i < 100; i++)
            {
                var date = DateTime.UtcNow.AddSeconds(-(5*(100 - i)));
                var ping = new PingResult()
                {
                    Date = date,
                    Ping = rand.Next(1000),
                    Status = IPStatus.Success,
                    Success = true
                    
                };
                Pings.Add(ping);
            }
        }
    }

    public class PingResult
    {
        public DateTime Date;
        public IPStatus Status;
        public bool Success;
        public int Ping;
    }

    public class UptimeMonitor
    {
        private Config _config;
        private bool _stop = true;
        private DateTime _lastTime;

        public FailPeriod ActiveFail;
        public List<FailPeriod> PreviousFails = new List<FailPeriod>();

        public int SuccessCount = 0;
        public int PartialCount = 0;
        public int FailCount = 0;
        public UptimeLogger uptimeLogger = new UptimeLogger(log4net.LogManager.GetLogger("uptimeLog"));

        public List<IpAddressData> IpAddresses => _config.IpAddressData;
        public List<IpAddressData> AddAddresses = new List<IpAddressData>();  
        public List<IpAddressData> RemoveAddresses = new List<IpAddressData>(); 

        public UptimeMonitor(Config config)
        {
            _config = config;
        }

        public float Interval => _config.GetFloatSetting(ConfigSection.Monitor, ConfigSetting.Frequency);        
        public int TimeOut => _config.GetIntSetting(ConfigSection.Monitor, ConfigSetting.TimeOut);
        public int SleepInterval => _config.GetIntSetting(ConfigSection.Monitor, ConfigSetting.SleepPeriod);


        public void Monitor()
        {
            _stop = false;
            do
            {
                if (AddAddresses.Any())
                {
                    IpAddresses.AddRange(AddAddresses);
                    _config.SaveConfig();
                    AddAddresses.Clear();
                }
                if (RemoveAddresses.Any())
                {
                    foreach (var addr in RemoveAddresses)
                    {
                        IpAddresses.Remove(addr);
                        _config.SaveConfig();
                    }
                    RemoveAddresses.Clear();
                }
                var time = DateTime.UtcNow;
                if (time - _lastTime > TimeSpan.FromSeconds(Interval))
                {
                    _lastTime = time;
                    CheckConnections();

                }


                Thread.Sleep(SleepInterval);

            } while (!_stop);
        }

        public void StopMonitor()
        {
            _stop = true;
        }

        public bool IsRunning()
        {
            return !_stop;
        }

        private void CheckConnections()
        {
            foreach (var address in IpAddresses)
            {
                CheckConnection(address);
            }
            AnalyseData();
        }

        private void CheckConnection(IpAddressData address)
        {
            var ping = new Ping();
            var reply = ping.Send(address.Ip, TimeOut);
            if (reply == null)
            {
                address.Exception = true;
            }
            else
            {
                address.Status = reply.Status;
                address.Time = (int)reply.RoundtripTime;
                address.Exception = false;
            }
            if (address.Status == IPStatus.Success)
            {
                address.TotalSuccessTime += address.Time;
                address.SuccessCount++;
            }

            Task.Factory.StartNew(() =>
            {
                uptimeLogger.LogPingReply(reply);
            });
        }

        private void AnalyseData()
        {
            if (IpAddresses.All(x => x.Exception || x.Status != IPStatus.Success))
            {
                FailCount ++;
                if (ActiveFail == null)
                {
                    ActiveFail = new FailPeriod(DateTime.UtcNow);
                }
            }
            else if (IpAddresses.Any(x => x.Exception || x.Status != IPStatus.Success))
            {
                PartialCount++;
                if (ActiveFail != null)
                {
                    ActiveFail.ReturnTime = DateTime.UtcNow;
                    PreviousFails.Add(ActiveFail);
                    ActiveFail = null;
                }
            }
            else
            {
                SuccessCount++;
                if (ActiveFail != null)
                {
                    ActiveFail.ReturnTime = DateTime.UtcNow;
                    PreviousFails.Add(ActiveFail);

                    uptimeLogger.LogActiveFail(ActiveFail);

                    ActiveFail = null;

            
                }
            }


        }
    }
}