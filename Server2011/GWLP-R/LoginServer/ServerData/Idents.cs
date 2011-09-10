using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoginServer.ServerData
{
        public static class Idents
        {
                public enum Clients
                {
                        NetID,
                        AccID,
                        CharID
                }

                public enum GameServers
                {
                        NetID,
                        IP,
                        Port
                }
        }
}
