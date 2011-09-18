using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GameServer.Enums;
using ServerEngine.Tools;

namespace GameServer.ServerData
{
        public class Map : IIdentifiable<Maps>
        {
                private readonly object objLock = new object();

                private Dictionary<int, MapSpawn> spawns;
                private Dictionary<int, NonPlayerChar> npcs;
                private List<int> charIDs;
                private IDManager charAgentIDManager;
                private ConcurrentQueue<Action<Map>> actionQueue;
                private readonly Dictionary<Maps, object> identifierKeyEnumeration;

                /// <summary>
                ///   Create a new instance of the class
                /// </summary>
                public Map(int mapID, int gameMapID, int gameFileID)
                {
                        var tmp = new Dictionary<Maps, object>();
                        tmp.Add(Maps.MapID, mapID);
                        tmp.Add(Maps.GameMapID, gameMapID);
                        tmp.Add(Maps.GameFileID, gameFileID);

                        identifierKeyEnumeration = tmp;

                        Spawns = new Dictionary<int, MapSpawn>();
                        Npcs = new Dictionary<int, NonPlayerChar>();
                        CharIDs = new List<int>();
                        CharAgentIDManager = new IDManager(1, 10000);
                        ActionQueue = new ConcurrentQueue<Action<Map>>();

                        Debug.WriteLine("Created new map");
                }

                /// <summary>
                ///   This indexer returns the identifier of the given type.
                /// </summary>
                /// <param name="identType"></param>
                /// <returns></returns>
                public object this[Maps identType]
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
                ///   This property contains possible spawn points for a map
                ///   THIS SHOULD BE CHANGED, TO HAVE NPCs AND PLAYERS DIVIDED FOR EACH SPAWN
                /// </summary>
                public Dictionary<int, MapSpawn> Spawns
                {
                        get
                        {
                                lock (objLock)
                                {
                                        return spawns;
                                }
                        }
                        private set
                        {
                                lock (objLock)
                                {
                                        spawns = value;
                                }
                        }
                }

                /// <summary>
                ///   This property contains the NPCs that are on this map
                /// </summary>
                public Dictionary<int, NonPlayerChar> Npcs
                {
                        get
                        {
                                lock (objLock)
                                {
                                        return npcs;
                                }
                        }
                        private set
                        {
                                lock (objLock)
                                {
                                        npcs = value;
                                }
                        }
                }

                /// <summary>
                ///   This property contains the CharIDs of the chars that are currently on this map
                /// </summary>
                public List<int> CharIDs
                {
                        get
                        {
                                lock (objLock)
                                {
                                        return charIDs;
                                }
                        }
                        private set
                        {
                                lock (objLock)
                                {
                                        charIDs = value;
                                }
                        }
                }

                /// <summary>
                ///   This property contains the ID manager for AgentIDs
                /// </summary>
                public IDManager CharAgentIDManager
                {
                        get
                        {
                                lock (objLock)
                                {
                                        return charAgentIDManager;
                                }
                        }
                        private set
                        {
                                lock (objLock)
                                {
                                        charAgentIDManager = value;
                                }
                        }
                }

                /// <summary>
                ///   This property contains the action queue
                /// </summary>
                public ConcurrentQueue<Action<Map>> ActionQueue
                {
                        get
                        {
                                lock (objLock)
                                {
                                        return actionQueue;
                                }
                        }
                        private set
                        {
                                lock (objLock)
                                {
                                        actionQueue = value;
                                }
                        }
                }

                /// <summary>
                ///   This property contains the identifier - key enumeration of the client.
                /// </summary>
                public IEnumerable<KeyValuePair<Maps, object>> IdentifierKeyEnumeration
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
