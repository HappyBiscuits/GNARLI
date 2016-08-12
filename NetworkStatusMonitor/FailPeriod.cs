using System;

namespace NetworkStatusMonitor
{
    public class FailPeriod
    {
        public DateTime FailTime;
        public DateTime ReturnTime;

        public FailPeriod(DateTime failTime)
        {
            FailTime = failTime;
        }
    }
}