#region

using System.Collections.Generic;
using Newtonsoft.Json.Linq;

#endregion

namespace Quobject.EngineIoClientDotNet.Client
{
    public class HandshakeData
    {
        public long PingInterval;
        public long PingTimeout;
        public string Sid;
        public List<string> Upgrades = new List<string>();

        public HandshakeData(string data)
            : this(JObject.Parse(data))
        {
        }

        public HandshakeData(JObject data)
        {
            var upgrades = data.GetValue("upgrades");

            foreach (var e in upgrades)
            {
                Upgrades.Add(e.ToString());
            }

            Sid = data.GetValue("sid").Value<string>();
            PingInterval = data.GetValue("pingInterval").Value<long>();
            PingTimeout = data.GetValue("pingTimeout").Value<long>();
        }
    }
}