using System;
using LoginServer.Enums;

namespace LoginServer.ServerData.DataInterfaces
{
        public interface IHasSyncStatusData
        {
                SyncStatus Status { get; set; }

                DateTime LastStatusChange { get; set; }

                uint SyncCount { get; set; }
        }
}