using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace NetworkStatusMonitor
{
    

    class Program
    {
        static void Main(string[] args)
        {
            var monitor = new UptimeMonitor();
            new Thread(monitor.Monitor).Start();
            var ui = new UptimeMonitorUi {Monitor = monitor};
            new Thread(ui.Update).Start();
              

        }
    }
}