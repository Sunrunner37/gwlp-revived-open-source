using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GameServer.Commands;
using GameServer.DataBase;
using GameServer.Enums;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.DataManagement;
using ServerEngine.NetworkManagement;
using ServerEngine.GuildWars.Tools;

namespace GameServer.ServerData
{
        public static class World
        {
                static World()
                {
                        ChatCommandsDict = new Dictionary<string, Type>();

                        // HelpMe (most necessary)
                        ChatCommandsDict.Add("HelpMe", typeof(HelpMe));

                        // Custom commands, alphabetical order
                        ChatCommandsDict.Add("ChangeMap", typeof(ChangeMap));
                        ChatCommandsDict.Add("SetSpawn", typeof(SetSpawn));
                        ChatCommandsDict.Add("Test", typeof (Test));

                        // GW commands alphabetical order
                        ChatCommandsDict.Add("stuck", typeof(GWStuck));

                        charLocalIDs = new IDManager(1, 10000);
                }

                public Dictionary<string, Type> ChatCommandsDict { get; private set; }

                private Dictionary<Type, MultiKeyDictionary<IEnumerable<IWrapper>>> worldData;

                private IDManager charLocalIDs;

                public static int LoginSrvNetID { get; set;}

                public static DataClient GetClient(Clients identType, object identKey)
                {
                        DataClient result;

                        if (clients.TryGetValue(new KeyValuePair<Clients, object>(identType, identKey), out result))
                                return result;
                        return null;
                }

                public static DataCharacter GetCharacter(Chars identType, object identKey)
                {
                        DataCharacter result;

                        if (chars.TryGetValue(new KeyValuePair<Chars, object>(identType, identKey), out result))
                                return result;
                        return null;
                }

                public static Map GetMap(Maps identType, object identKey)
                {
                        Map result;

                        if (maps.TryGetValue(new KeyValuePair<Maps, object>(identType, identKey), out result))
                                return result;
                        return null;
                }

                public static void AddClient(DataClient client)
                {
                        clients.Add(client);
                }

                public static void AddChar(DataCharacter chara)
                {
                        chars.Add(chara);
                }

                public static void AddMap(Map map)
                {
                        maps.Add(map);
                }

                public static void UpdateClient(DataClient oldClient, DataClient newClient)
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

                public static void UpdateChar(DataCharacter oldChar, DataCharacter newChar)
                {
                        var identifier = oldChar.IdentifierKeyEnumeration.Intersect(newChar.IdentifierKeyEnumeration);
                        if (identifier.Count() > 0)
                        {
                                chars.Remove(identifier.First());
                                chars.Add(newChar);
                        }
                        else
                        {
                                // shouldnt throw an exception anymore
                                chars.Add(newChar);
                        }
                }

                public static void KickClient(Clients identType, object identKey)
                {
                        var client = GetClient(identType, identKey);
                        var map = GetMap(Maps.MapID, client.MapID);
                        var charID = (int)client[Clients.CharID];
                        var netID = (int)client[Clients.NetID];

                        var agentID = (int)GetCharacter(Chars.CharID, charID)[Chars.AgentID];
                        var localID = (int)GetCharacter(Chars.CharID, charID)[Chars.LocalID];

                        charLocalIDs.FreeID(localID);
                        map.CharAgentIDManager.FreeID(agentID);

                        // kick DataClient connection
                        NetworkManager.Instance.RemoveClient(netID);

                        // remove DataClient
                        clients.Remove(new KeyValuePair<Clients, object>(identType, identKey));

                        // remove character
                        chars.Remove(new KeyValuePair<Chars, object>(Chars.CharID, charID));

                        // remove from map
                        map.CharIDs.Remove(charID);

                        Debug.WriteLine("DataClient[{0}] kicked.", netID);
                }

                public static IEnumerable<DataClient> GetUnauthorizedClients()
                {
                        return (from c in clients.Values
                               where c.Status == SyncStatus.Unauthorized
                               select c).Distinct();
                }

                public static IEnumerable<DataClient> GetDispatchedClients()
                {
                        return (from c in clients.Values
                                where c.Status == SyncStatus.Dispatching
                                select c).Distinct();
                }

                public static UInt16[] GetMapIDs()
                {
                        return (from m in maps.Values
                                select ((ushort) ((int)m[Maps.MapID]))).Distinct().ToArray();
                }

                public static IEnumerable<Map> GetMaps()
                {
                        return maps.Values;
                }

                public static void RegisterCharacterIDs(out int localID, out int agentID, int mapIDToRegisterWith)
                {
                        localID = charLocalIDs.RequestID();
                        agentID = GetMap(Maps.MapID, mapIDToRegisterWith).CharAgentIDManager.RequestID();
                }

                public static void UnRegisterCharacterIDs(int localID, int agentID, int mapIDToUnRegisterWith)
                {
                        charLocalIDs.FreeID(localID);
                        GetMap(Maps.MapID, mapIDToUnRegisterWith).CharAgentIDManager.FreeID(agentID);
                }

                public static void BuildMap(int newMapID)
                {
                        if (!GetMapIDs().Contains((ushort)newMapID))
                        {
                                using (var db = (MySQL)DataBaseProvider.GetDataBase())
                                {
                                        var mapID = newMapID;

                                        // instant execution:
                                        var map = (from m in db.mapsMasterData
                                                   where m.mapID == mapID
                                                   select m).First();

                                        // create the new map:
                                        var newMap = new Map(mapID, map.gameMapID, map.gameMapFileID);

                                        // get the spawns:
                                        var spawns = from s in db.mapsSpawns
                                                     where s.mapID == mapID
                                                     select s;

                                        // add them
#warning FIXME Maps should distinguish between outposts/explorable areas, pvp/pve!
                                        foreach (var mapSpawn in spawns)
                                        {
                                                var spawn = new MapSpawn()
                                                {
                                                        SpawnID = mapSpawn.spawnID,
                                                        SpawnX = mapSpawn.spawnX,
                                                        SpawnY = mapSpawn.spawnY,
                                                        SpawnPlane = mapSpawn.spawnPlane,
                                                        SpawnRadius = mapSpawn.spawnRadius,
                                                        IsOutpost = (mapSpawn.isOutpost == 0) ? false : true,
                                                        IsPvE = (mapSpawn.isPvE == 0) ? false : true,
                                                        TeamSpawnNumber = mapSpawn.teamSpawnNumber ?? 0
                                                };

                                                newMap.Spawns.Add(spawn.SpawnID, spawn);
                                        }

                                        // get the npcs:
                                        var npcSpawns = from nsp in db.nPcSSpawns
                                                        where nsp.mapID == mapID
                                                        select nsp;

                                        // add them
                                        if (npcSpawns.Count() != 0)
                                        {
                                                foreach (var npc in npcSpawns)
                                                {
                                                        var npcID = npc.npcID;
                                                        var dbNpcs = from n in db.nPcSMasterData
                                                                     where n.npcID == npcID
                                                                     select n;

                                                        

                                                        // check if we found an npc
                                                        if (dbNpcs.Count() == 0) continue;

                                                        var dbNpc = dbNpcs.First();

                                                        var nameID = npc.nameID;
                                                        var nameHash = (from n in db.nPcSNames
                                                                        where n.nameID == nameID
                                                                        select n.nameHash).First();

                                                        var newNpc = new NonPlayerChar()
                                                        {
                                                                AgentID = newMap.CharAgentIDManager.RequestID(),
                                                                Stats = new NonPlayerCharStats()
                                                                {
                                                                        NpcID = npcID,
                                                                        FileID = dbNpc.npcFileID,
                                                                        ModelHash = dbNpc.modelHash,
                                                                        Appearance = dbNpc.appearance,
                                                                        NameHash = nameHash,
                                                                        Scale = dbNpc.scale,
                                                                        ProfessionFlags = dbNpc.professionFlags,
                                                                        Position = new GWVector(npc.spawnX, npc.spawnY, npc.plane),
                                                                        Rotation = npc.rotation,
                                                                        IsRotating = false,
                                                                        Speed = npc.speed,
                                                                        Level = npc.level,
                                                                        Profession = npc.profession,
                                                                        Energy = 100,
                                                                        EnergyRegen = 0.3F,
                                                                        Health = 100,
                                                                        HealthRegen = 0.3F,
                                                                        Direction = new GWVector(0, 0, 0),
                                                                        Morale = 100,
                                                                        VitalStats = (int)VitalStatus.Alive,
                                                                        MoveState = MovementState.MoveKeepDir,
                                                                        MoveType = MovementType.Stop,
                                                                }
                                                        };

                                                        newNpc.Stats.HasNameHash = (newNpc.Stats.NameHash != null && newNpc.Stats.NameHash.Count() != 0);

                                                        newMap.Npcs.Add(newNpc.AgentID, newNpc);
                                                }
                                        }
                                        

                                        // add the map
                                        AddMap(newMap);
                                }
                        }
                }
        }
}
