using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading;

namespace GNARLI
{
    public class UptimeMonitorUi
    {
        private Config _config;
        private UptimeMonitor _monitor;
        public int SleepTime = 500;
        public bool Updating = false;
        private string loggingStatus = "ENABLED";

        public UptimeMonitorUi(Config config, UptimeMonitor monitor)
        {
            _config = config;
            _monitor = monitor;
        }

        public void Update()
        {
            Updating = true;
            do
            {
                while (!Console.KeyAvailable)
                {
                    MainScreen();
                    Thread.Sleep(SleepTime);
                }
                if (Console.ReadKey().Key == ConsoleKey.Enter) Updating = false;
            } while (Updating);
            Options();
        }

        public void MainScreen()
        {
            Console.Clear();
            Title();
            Status();
            PingStats();
            PingStatus();
            Fails();
            Settings();
            //var log = new UpTimeLog();
            //log.PopulateTest();
            //DrawGraph(log);
            //Console.ReadLine();
            Pause();
        }

        public void Options()
        {
            Console.Clear();
            Title();
            Status();
            Settings();
            Console.WriteLine(new string('-', 80));
            Console.WriteLine("# Main Menu");
            Console.WriteLine(new string('-', 80));
            Console.WriteLine("V Resume Viewing");
            Console.WriteLine("S Start/Stop Monitoring");
            Console.WriteLine("L Start/Stop Logging");
            //Console.WriteLine("C Clear Log");
            //Console.WriteLine("O Set Log Location");
            Console.WriteLine("A Add New Address");
            Console.WriteLine("R Remove Address");
            Console.WriteLine("T Set Timeout");
            Console.WriteLine("F Set Frequency");
            Console.WriteLine("E Set Sleep Period");
            Console.WriteLine("Q Quit");
            var key = Console.ReadKey();
            switch (key.Key)
            {
                case (ConsoleKey.V):
                    {
                        Update();
                        break;
                    }
                case (ConsoleKey.S):
                    {
                        if (_monitor.IsRunning()) _monitor.StopMonitor();
                        else new Thread(_monitor.Monitor).Start();
                        Options();
                        break;
                    }
                case (ConsoleKey.L):
                    {
                        ToggleLogging();
                        Options();
                        break;
                    }
                case (ConsoleKey.A):
                    {
                        AddNewAddress();
                        break;
                    }
                case (ConsoleKey.R):
                    {
                        RemoveAddress();
                        break;
                    }
                case (ConsoleKey.T):
                    {
                        Console.WriteLine();
                        Console.WriteLine(new string('-', 80));
                        Console.WriteLine("Enter New Timeout: ");
                        var dat = Console.ReadLine();
                        int val;
                        if (int.TryParse(dat, out val))
                        {
                            _config.SetIntSetting(ConfigSection.Monitor, ConfigSetting.TimeOut, val);
                            Options();
                        }
                        else
                        {
                            Console.WriteLine("Must be an Integer");
                            Console.WriteLine("Press Enter to Continue");
                            Console.ReadLine();
                            Options();
                        }

                        break;
                    }
                case (ConsoleKey.F):
                    {   
                        Console.WriteLine();
                        Console.WriteLine(new string('-', 80));
                        Console.WriteLine("Enter New Frequency: ");
                        var dat = Console.ReadLine();
                        float val;
                        if (float.TryParse(dat, out val))
                        {
                            _config.SetFloatSetting(ConfigSection.Monitor, ConfigSetting.Frequency, val);
                            Options();
                        }
                        else
                        {
                            Console.WriteLine("Must be a Float");
                            Console.WriteLine("Press Enter to Continue");
                            Console.ReadLine();
                            Options();
                        }

                        break;
                    }
                case (ConsoleKey.E):
                    {
                        Console.WriteLine();
                        Console.WriteLine(new string('-', 80));
                        Console.WriteLine("Enter New Sleep Period: ");
                        var dat = Console.ReadLine();
                        int val;
                        if (int.TryParse(dat, out val))
                        {
                            _config.SetIntSetting(ConfigSection.Monitor, ConfigSetting.SleepPeriod, val);
                            Options();
                        }
                        else
                        {
                            Console.WriteLine("Must be an Integer");
                            Console.WriteLine("Press Enter to Continue");
                            Console.ReadLine();
                            Options();
                        }

                        break;
                    }
            }


        }

        private void AddNewAddress()
        {
            Console.WriteLine();
            Console.WriteLine(new string('-', 80));
            Console.WriteLine("Leave Blank To Cancel");
            Console.WriteLine("Enter Address Name: ");
            var name = Console.ReadLine();
            if (name.Length == 0)
            {
                Options();
                return;
            }
            Console.WriteLine(name);
            Console.WriteLine("Enter IP Address: ");
            var address = Console.ReadLine();
            IPAddress addr;
            var success = IPAddress.TryParse(address, out addr);
            if (success)
            {
                var ipaddressData = new IpAddressData(name, addr);
                _monitor.AddAddresses.Add(ipaddressData);

                Update();
                return;
            }
            else
            {
                Console.WriteLine("\"" + address + "\" is  not a valid IP Address");
                Console.WriteLine("Press Enter To Continue");
                Console.ReadLine();
                Options();
                return;
            }


        }

        private void RemoveAddress()
        {
            Console.WriteLine(new string('-', 80));
            PingStatus();
            Console.WriteLine(new string('-', 80));
            Console.WriteLine("Select an Address to Remove (enter 0 to return");
            var line = Console.ReadLine();
            int val;
            if (int.TryParse(line, out val))
            {
                if (val > _monitor.IpAddresses.Count)
                {
                    Console.WriteLine(val + " is not a valid option.");
                    Console.WriteLine("Press Enter to Continue");
                    Console.ReadLine();
                    Options();
                    return;
                }
                else
                {
                    _monitor.RemoveAddresses.Add(_monitor.IpAddresses[val - 1]);
                    Update();
                    
                    return;
                }
            }
            else
            {
                Console.WriteLine(line + " is not a valid option.");
                Console.WriteLine("Press Enter to Continue");
                Console.ReadLine();
                Options();
                return;
            }
        }

        private void Title()
        {
            Console.WriteLine(new string('=', 80));
            var header = "Network Uptime Monitor v" + Assembly.GetExecutingAssembly().GetName().Version;
            Console.WriteLine(String.Format("{0," + ((80 / 2) + (header.Length / 2)) + "}", header));
            Console.WriteLine(new string('=', 80));

        }

        private void Status()
        {
            var header = "";
            Console.Write(String.Format("{0,25}", "Monitor: "));
            if (_monitor.IsRunning())
            {
                Console.ForegroundColor = ConsoleColor.Green;
                header = "RUNNING";
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                header = "STOPPED";
            }
            Console.Write(header);
            Console.ResetColor();

            DisplayLoggingStatus();
        }

        private void DisplayLoggingStatus()
        {
            Console.Write(String.Format("{0,25}", "Logging: "));
            Console.ForegroundColor = ConsoleColor.Red;

            if (loggingStatus == "ENABLED")
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }

            Console.WriteLine(loggingStatus);
            Console.ResetColor();
        }
   
        private void Settings()
        {
            Console.Write(string.Format("{0, 23}", "Timeout: " + _monitor.TimeOut + "ms"));
            Console.Write(String.Format("{0,23}", "Frequency: " + _monitor.Interval + "s"));
            Console.Write(String.Format("{0,23}", "Sleep: " + _monitor.SleepInterval + "ms"));
            Console.WriteLine();
        }

        private void PingStats()
        {
            Console.Write(string.Format("{0, 22}", "Success: "));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(_monitor.SuccessCount);
            Console.ResetColor();
            Console.Write(string.Format("{0, 22}", "Partial: "));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(_monitor.PartialCount);
            Console.ResetColor();
            Console.Write(string.Format("{0, 22}", "Fails: "));
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(_monitor.FailCount);
            Console.WriteLine();
            Console.ResetColor();
        }
        private void PingStatus()
        {
            Console.WriteLine(new string('-', 80));
            Console.Write(string.Format("{0, 18}", "Name"));
            Console.Write(string.Format("{0, 18}", "Address"));
            Console.Write(string.Format("{0, 9}", "Ping"));
            Console.Write(string.Format("{0, 9}", "Avg"));
            Console.WriteLine(string.Format("{0, 18}", "Status"));
            Console.WriteLine(new string('-', 80));
            var count = 1;
            foreach (var addr in _monitor.IpAddresses)
            {
                PingStatus(addr, count);
                count++;
            }
            //Console.WriteLine(new string('-', 80));
        }

        private void PingStatus(IpAddressData addr, int val)
        {
            var name = addr.Name;
            var ip = addr.Ip.ToString();
            var time = addr.Time + "ms";
            var avg = ((float) addr.TotalSuccessTime/(float) addr.SuccessCount).ToString("0") + "ms";
            var status = addr.Status.ToString();
            
            Console.Write(val + ".");
            Console.Write(string.Format("{0, 18}", name));
            Console.Write(string.Format("{0, 18}", ip));
            Console.Write(string.Format("{0, 9}", time));
            Console.Write(string.Format("{0, 9}", avg));
            if (addr.Status != IPStatus.Success) Console.ForegroundColor = ConsoleColor.Red;
            else Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(string.Format("{0, 18}", status));
            Console.ResetColor();
            Console.Write("\n");
        }

        private void Fails()
        {
            Console.WriteLine(new string('-', 80));
            var header = "Fails";
            Console.WriteLine(String.Format("{0," + ((80 / 2) + (header.Length / 2)) + "}", header));

            Console.WriteLine(new string('-', 80));
            Console.Write(string.Format("{0, 20}", "Start"));
            Console.Write(string.Format("{0, 20}", "End"));
            Console.WriteLine(string.Format("{0, 20}", "Duration"));
            Console.WriteLine(new string('-', 80));
            if (_monitor.ActiveFail != null)
            {
                Fails(_monitor.ActiveFail, true);
            }
            foreach (var fail in _monitor.PreviousFails.OrderByDescending(x => x.ReturnTime))
            {
                Fails(fail);
            }
            Console.WriteLine(new string('-', 80));
        }

        private void Fails(FailPeriod period, bool active = false)
        {
            var startTime = period.FailTime;
            var returnTime = DateTime.UtcNow;
            if (!active) returnTime = period.ReturnTime;
            Console.Write(string.Format("{0, 20}", startTime.ToString("dd/MM/yy HH:mm")));
            if (!active) Console.Write(string.Format("{0, 20}", returnTime.ToString("dd/MM/yy HH:mm")));
            else Console.Write(string.Format("{0, 20}", "In Progress"));
            Console.WriteLine(string.Format("{0, 20}", (returnTime - startTime).TotalSeconds.ToString("0")) + "s");
        }

        private void Pause()
        {
            Console.WriteLine(new string('-', 80));
            var header = "Press Enter for Options";
            Console.WriteLine(String.Format("{0," + ((80 / 2) + (header.Length / 2)) + "}", header));

        }

        private void ToggleLogging()
        {
            _config.SetBoolSetting(ConfigSection.Logging, ConfigSetting.LogPingReply, !_config.GetBoolSetting(ConfigSection.Logging, ConfigSetting.LogPingReply));
            _config.SetBoolSetting(ConfigSection.Logging, ConfigSetting.LogActiveFail, !_config.GetBoolSetting(ConfigSection.Logging, ConfigSetting.LogActiveFail));

            if(_config.GetBoolSetting(ConfigSection.Logging, ConfigSetting.LogPingReply))
            {
                loggingStatus = "ENABLED";
            }
            else
            {
                loggingStatus = "DISABLED";
            }
        }
    }
}