using System;

namespace GameServer.ServerData.DataInterfaces
{
        public interface IHasHeartbeatData
        {
                bool EnabledHartBeat { get; set; }
                DateTime LastHeartBeat { get; set; }
        }
}