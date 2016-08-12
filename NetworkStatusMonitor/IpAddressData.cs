using System.Net;
using System.Net.NetworkInformation;

namespace GNARLI
{
    public class IpAddressData
    {
        public string Name;
        public IPAddress Ip;
        public IPStatus Status = IPStatus.Unknown;
        public int Time = 1000;
        public int TotalSuccessTime = 0;
        public int SuccessCount= 0;
        public bool Exception;

        public IpAddressData(string name, IPAddress ip)
        {
            Name = name;
            Ip = ip;
        }
    }
}