using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ServerEngine.Tools;

namespace LoginServer.ServerData
{
        public class GameServer : IIdentifiable<Idents.GameServers>
        {
                public GameServer()
                {
                        identifierKeyEnumeration = new Dictionary<Idents.GameServers, object>();

                        Debug.WriteLine("Created new game server");
                }

                public GameServer(int netID, byte[] ip, int port)
                {
                        var tmp = new Dictionary<Idents.GameServers, object>();
                        tmp.Add(Idents.GameServers.NetID, netID);
                        tmp.Add(Idents.GameServers.IP, ip);
                        tmp.Add(Idents.GameServers.Port, port);

                        identifierKeyEnumeration = tmp;

                        Debug.WriteLine("Created new game server");
                }

                public object this[Idents.GameServers identType]
                {
                        get
                        {
                                object id;

                                identifierKeyEnumeration.TryGetValue(identType, out id);

                                return id;
                        }
                }

                public int Utilization { get; set; } // in percent

                public ushort[] AvailableMaps { get; set; }

                private readonly Dictionary<Idents.GameServers, object> identifierKeyEnumeration;
                public IEnumerable<KeyValuePair<Idents.GameServers, object>> IdentifierKeyEnumeration { get { return identifierKeyEnumeration; } }
        }
}
