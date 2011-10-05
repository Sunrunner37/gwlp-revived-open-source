using System;
using GameServer.Enums;

namespace GameServer.ServerData.DataInterfaces
{
        public interface IHasSyncStatusData
        {
                SyncStatus Status { get; set; }

                DateTime LastStatusChange { get; set; }

                uint SyncCount { get; set; }
        }
}