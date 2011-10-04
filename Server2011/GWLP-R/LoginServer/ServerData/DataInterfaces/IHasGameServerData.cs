using System;
using ServerEngine.DataManagement;

namespace LoginServer.ServerData.DataInterfaces
{
        public interface IHasGameServerData
        {
                byte Utilization { get; set; }
                UInt16[] AvailableMaps { get; set; }
        }
}