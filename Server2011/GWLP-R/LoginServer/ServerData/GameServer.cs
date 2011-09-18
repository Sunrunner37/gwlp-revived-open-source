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
                private readonly object objLock = new object();

                private int utilization;
                private ushort[] availableMaps;
                private readonly Dictionary<Idents.GameServers, object> identifierKeyEnumeration;

                /// <summary>
                ///   Create a new instance of the class, without identifiers
                /// </summary>
                public GameServer()
                {
                        identifierKeyEnumeration = new Dictionary<Idents.GameServers, object>();

                        Debug.WriteLine("Created new game server");
                }

                /// <summary>
                ///   Create a new instance of the class
                /// </summary>
                public GameServer(int netID, byte[] ip, int port)
                {
                        var tmp = new Dictionary<Idents.GameServers, object>();
                        tmp.Add(Idents.GameServers.NetID, netID);
                        tmp.Add(Idents.GameServers.IP, ip);
                        tmp.Add(Idents.GameServers.Port, port);

                        identifierKeyEnumeration = tmp;

                        Debug.WriteLine("Created new game server");
                }

                /// <summary>
                ///   This indexer returns the identifier of the given type.
                /// </summary>
                /// <param name="identType"></param>
                /// <returns></returns>
                public object this[Idents.GameServers identType]
                {
                        get
                        {
                                lock (objLock)
                                {
                                        object id;

                                        identifierKeyEnumeration.TryGetValue(identType, out id);

                                        return id;
                                }
                        }
                }

                /// <summary>
                ///   This property contains the server utilization in percent
                /// </summary>
                public int Utilization
                {
                        get
                        {
                                lock (objLock)
                                {
                                        return utilization;
                                }
                        }
                        set
                        {
                                lock (objLock)
                                {
                                        utilization = value;
                                }
                        }
                }

                /// <summary>
                ///   This property contains this servers available maps as MapIDs
                /// </summary>
                public ushort[] AvailableMaps
                {
                        get
                        {
                                lock (objLock)
                                {
                                        return availableMaps;
                                }
                        }
                        set
                        {
                                lock (objLock)
                                {
                                        availableMaps = value;
                                }
                        }
                }


                /// <summary>
                ///   This property contains the identifier - key enumeration of the client.
                /// </summary>
                public IEnumerable<KeyValuePair<Idents.GameServers, object>> IdentifierKeyEnumeration
                {
                        get
                        {
                                lock (objLock)
                                {
                                        return identifierKeyEnumeration;
                                }
                        }
                }
        }
}
