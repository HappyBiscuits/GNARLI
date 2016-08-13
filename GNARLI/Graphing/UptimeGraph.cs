using System;
using System.Linq;

namespace GNARLI.Graphing
{
   public class UptimeGraph : IUptimeGraph
    {
        public void DrawGraph(UpTimeLog log)
        {
            var graphPeriod = 300;
            var pings = log.Pings.Where(x => (DateTime.UtcNow - x.Date).TotalSeconds < graphPeriod).ToList();
            var split = graphPeriod / 75f;
            var max = pings.Where(x => x.Success).Max(x => x.Ping);
            var pingSplit = max / 20f;
            pings = pings.OrderByDescending(x => x.Success).ThenByDescending(x => x.Ping).ToList();

            for (var i = 0; i < pings.Count; i++)
            {
                Console.Write(pings[i].Ping.ToString("0000"));
                Console.Write("|");
                var pos = (int)((300 - (DateTime.UtcNow - pings[i].Date).TotalSeconds));
                Console.WriteLine(string.Format("{0, " + pos + "}", "@"));

            }
            Console.Write(new string(' ', 5));
            Console.WriteLine(new string('-', 75));

        }
    }
}
