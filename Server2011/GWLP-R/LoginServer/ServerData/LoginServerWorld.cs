using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ServerEngine.DataManagement;
using ServerEngine.GuildWars.DataWrappers.Maps;

namespace LoginServer.ServerData
{
        public sealed class LoginServerWorld : World
        {
                /// <summary>
                ///   Singleton instance
                /// </summary>
                private static readonly LoginServerWorld instance = new LoginServerWorld();

                /// <summary>
                ///   Creates a new instance of the class
                /// </summary>
                private LoginServerWorld()
                {
                        worldData = new Dictionary<Type, MultiKeyDictionary<IEnumerable<IWrapper>>>();
                }

                /// <summary>
                ///   This property contains the singleton instance of the class
                /// </summary>
                public static LoginServerWorld Instance
                {
                        get { return instance; }
                }

                /// <summary>
                ///   Returns the most suitable game server if there is any.
                /// </summary>
                public bool GetBestGameServer(MapID mapID, out DataGameServer gs)
                {
                        // get the right dict
                        MultiKeyDictionary<IEnumerable<IWrapper>> tmpDict; 

                        try
                        {
                                tmpDict = worldData[typeof(DataGameServer)];
                        }
                        catch (Exception)
                        {
                                gs = null;
                                return false;
                        }

                        if (tmpDict.Values.Count() > 0)
                        {
                                var servers = from nc in tmpDict.Values
                                              where (
                                                (((DataGameServer)nc).Data.Utilization < 100) &&
                                                (((DataGameServer)nc).Data.AvailableMaps.Contains((ushort)mapID.Value)))
                                              select ((DataGameServer)nc);

                                // select those which already have the map
                                if (servers.Count() > 0)
                                {
                                        servers.OrderBy(s => s.Data.Utilization);
                                        gs = servers.First();
                                        return true;
                                }

                                // or select one wich does not have the map but a good utilization ratio
                                tmpDict.Values.OrderBy(s => ((DataGameServer)s).Data.Utilization);
                                gs = (DataGameServer)tmpDict.Values.First();
                                return false;
                        }

                        gs = null;
                        return false;
                }
        }
}
