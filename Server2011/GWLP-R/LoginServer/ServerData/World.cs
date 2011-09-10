using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ServerEngine.NetworkManagement;
using ServerEngine.Tools;

namespace LoginServer.ServerData
{
        public static class World
        {
                static World()
                {
                        clients = new MultiKeyDictionary<Idents.Clients, Client>();
                        gameServers = new MultiKeyDictionary<Idents.GameServers, GameServer>();
                }

                private static readonly MultiKeyDictionary<Idents.Clients, Client> clients;
                private static readonly MultiKeyDictionary<Idents.GameServers, GameServer> gameServers;

                public static Client GetClient(Idents.Clients identType, object identKey)
                {
                        Client result;

                        if (clients.TryGetValue(new KeyValuePair<Idents.Clients, object>(identType, identKey), out result))
                                return result;
                        return null;
                }

                public static GameServer GetGameServer(Idents.GameServers identType, object identKey)
                {
                        GameServer result;

                        if (gameServers.TryGetValue(new KeyValuePair<Idents.GameServers, object>(identType, identKey), out result))
                                return result;
                        return null;
                }

                public static void AddClient(Client client)
                {
                        clients.Add(client);
                }

                public static void AddGameServer(GameServer gameServer)
                {
                        gameServers.Add(gameServer);
                }

                public static void UpdateClient(Client oldClient, Client newClient)
                {
                        var identifier = oldClient.IdentifierKeyEnumeration.Intersect(newClient.IdentifierKeyEnumeration);
                        if (identifier.Count() > 0)
                        {
                                clients.Remove(identifier.First());
                                clients.Add(newClient);
                        }
                        else
                        {
                                // shouldnt throw an exception anymore
                                clients.Add(newClient);
                        }
                }

                public static void KickClient(Idents.Clients identType, object identKey)
                {
                        var client = GetClient(identType, identKey);
                        var netID = client[Idents.Clients.NetID];

                        clients.Remove(new KeyValuePair<Idents.Clients, object>(identType, identKey));

                        NetworkManager.Instance.RemoveClient((int)netID);

                        Debug.WriteLine("Client[{0}] kicked.", (int)netID);
                }

                public static void KickGameServer(Idents.GameServers identType, object identKey)
                {
                        var server = GetGameServer(identType, identKey);
                        var netID = server[Idents.GameServers.NetID];

                        gameServers.Remove(new KeyValuePair<Idents.GameServers, object>(identType, identKey));

                        NetworkManager.Instance.RemoveClient((int)netID);

                        Debug.WriteLine("GameServer[{0}] kicked.", (int)netID);
                }

                public static bool GetBestGameServer(int mapID, out GameServer gs)
                {
                        // if there are servers available:
                        if (gameServers.Values.Count() > 0)
                        {
                                var servers = from nc in gameServers.Values
                                              where (
                                                            (nc.Utilization < 100) &&
                                                            (nc.AvailableMaps.Contains((ushort) mapID)))
                                              select nc;

                                // select those which already have the map
                                if (servers.Count() > 0)
                                {
                                        servers.OrderBy(s => s.Utilization);
                                        gs = servers.First();
                                        return true;
                                }

                                // or select one wich does not have the map but a good utilization ratio
                                gameServers.Values.OrderBy(s => s.Utilization);
                                gs = gameServers.Values.First();
                                return false;
                        }

                        gs = null;
                        return false;
                }
        }
}
