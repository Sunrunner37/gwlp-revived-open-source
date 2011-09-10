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
                        CharAgentIDManager = new IDManager(1);
                        ActionQueue = new ConcurrentQueue<Action<Map>>();

                        Debug.WriteLine("Created new map");
                }

                public object this[Maps identType]
                {
                        get
                        {
                                object id;

                                identifierKeyEnumeration.TryGetValue(identType, out id);

                                return id;
                        }
                }

                public Dictionary<int, MapSpawn> Spawns { get; private set; }

                public Dictionary<int, NonPlayerChar> Npcs { get; private set; }

                public List<int> CharIDs { get; private set; }

                public IDManager CharAgentIDManager { get; private set; }

                public ConcurrentQueue<Action<Map>> ActionQueue { get; private set; }

                private readonly Dictionary<Maps, object> identifierKeyEnumeration;
                public IEnumerable<KeyValuePair<Maps, object>> IdentifierKeyEnumeration { get { return identifierKeyEnumeration; } }
        }
}
