using System;

namespace GameServer.ServerData.DataInterfaces
{
        public interface IHasHeartbeatData
        {
                DateTime LastHeartBeat { get; set; }
        }
}