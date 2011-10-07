using System.Collections.Generic;

namespace GameServer.ServerData.DataInterfaces
{
        public interface IHasChatData
        {
                Dictionary<string, bool> ChatCommands { get; set; }
                bool ShowPrefix { get; set; }
                string ChatPrefix { get; set; }
                bool ShowColor { get; set; }
                byte ChatColor { get; set; }
        }
}