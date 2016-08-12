using System;
using System.Security.Policy;
using System.Threading;

namespace GNARLI
{


    class Program
    {


        static void Main(string[] args)
        {
            var config = new Config();
            config.LoadConfig();
            config.SaveConfig();
            var monitor = new UptimeMonitor(config);
            new Thread(monitor.Monitor).Start();
            var ui = new UptimeMonitorUi(config, monitor);
            new Thread(ui.Update).Start();


        }

    }
}