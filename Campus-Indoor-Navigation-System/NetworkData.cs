using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campus_Indoor_Navigation_System
{
    public class NetworkData
    {
        public int StausId { get; set; }
        public string? Ssid { get; set; }
        public string? SsidName { get { return string.IsNullOrWhiteSpace(Ssid) ? "Unknown" : Ssid; } }
        public int IpAddress { get; set; }
        public string? GatewayAddress { get; set; }
        public object? NativeObject { get; set; }
        public object? Bssid { get; set; }
        public object? Level { get; internal set; }
    }
}
