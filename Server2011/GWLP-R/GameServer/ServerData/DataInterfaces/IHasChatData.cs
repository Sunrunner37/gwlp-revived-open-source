using System.Collections.Generic;

namespace GameServer.ServerData.DataInterfaces
{
        public interface IHasChatData
        {
                Dictionary<string, bool> ChatCommands { get; set; }
                string ChatPrefix { get; set; }
                byte ChatColor { get; set; }
        }
}