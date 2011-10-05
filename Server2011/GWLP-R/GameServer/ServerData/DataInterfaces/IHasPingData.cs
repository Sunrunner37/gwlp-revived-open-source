using System;

namespace GameServer.ServerData.DataInterfaces
{
        public interface IHasPingData
        {
                DateTime PingTime { get; set; }
        }
}