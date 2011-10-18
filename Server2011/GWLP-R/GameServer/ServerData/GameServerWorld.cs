using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GameServer.Commands;
using GameServer.DataBase;
using GameServer.Enums;
using ServerEngine;
using ServerEngine.DataManagement;
using ServerEngine.DataManagement.DataWrappers;
using ServerEngine.GuildWars.DataWrappers.Chars;
using ServerEngine.GuildWars.DataWrappers.Maps;
using ServerEngine.GuildWars.Tools;

namespace GameServer.ServerData
{
        public sealed class GameServerWorld : DataManager
        {
                /// <summary>
                ///   Singleton instance
                /// </summary>
                private static readonly GameServerWorld instance = new GameServerWorld();

                /// <summary>
                ///   Creates a new instance of the class
                /// </summary>
                private GameServerWorld()
                {
                        worldData = new Dictionary<Type, MultiKeyDictionary<IEnumerable<IWrapper>>>();

                        // set the start time
                        StartTime = DateTime.Now;

                        // init the motd
                        MessageOfTheDay = new string[0];

                        // add the chat commands below:
                        ChatCommandsDict = new Dictionary<string, Type>();

                        // HelpMe (most necessary)
                        ChatCommandsDict.Add("HelpMe", typeof(HelpMe));

                        // Custom commands, alphabetical order
                        ChatCommandsDict.Add("ChangeMap", typeof(ChangeMap));
                        ChatCommandsDict.Add("ServerInfo", typeof(ServerInfo));
                        ChatCommandsDict.Add("SetSpawn", typeof(SetSpawn));
                        ChatCommandsDict.Add("Test", typeof(Test));

                        // GW commands alphabetical order
                        ChatCommandsDict.Add("stuck", typeof(GWStuck));
                }

                /// <summary>
                ///   This property contains the singleton instance of the class
                /// </summary>
                public static GameServerWorld Instance
                {
                        get { return instance; }
                }

                /// <summary>
                ///   This property contains the chat commands and their corresponding class types
                /// </summary>
                public Dictionary<string, Type> ChatCommandsDict { get; private set; }

                /// <summary>
                ///   This property contains the local config file thats loaded at server startup
                /// </summary>
                public ConfigFile LocalConfig { get; set; }

                /// <summary>
                ///   This property contains some lines server info.
                /// </summary>
                public string[] MessageOfTheDay { get; set; }

                /// <summary>
                ///   This property is set when the server starts up
                /// </summary>
                public DateTime StartTime { get; private set; }

                /// <summary>
                ///   This property contains the login server network ID 
                ///   (because there is only one login server connection)
                /// </summary>
                public NetID LoginSrvNetID { get; set;}

                /// <summary>
                ///   Returns an enumeration of clients that share the given syncro status
                /// </summary>
                public bool ClientWhereStatus(SyncStatus status, out IEnumerable<DataClient> value)
                {
                        // get the right dict
                        MultiKeyDictionary<IEnumerable<IWrapper>> tmpDict;

                        try
                        {
                                tmpDict = worldData[typeof(DataClient)];
                        }
                        catch (Exception)
                        {
                                // we've got no clients
                                value = default(IEnumerable<DataClient>);
                                return false;
                        }

                        // if we've got a client dict
                        if (tmpDict.Values.Count() > 0)
                        {
                                var clients = from nc in tmpDict.Values
                                              where ((DataClient)nc).Data.Status  == status
                                              select ((DataClient)nc);

                                // if we've got clients with that status
                                if (clients.Count() > 0)
                                {
                                        // ToList() will execute the linq expression directly here
                                        value = clients.ToList();
                                        return true;
                                }
                        }

                        value = default(IEnumerable<DataClient>);
                        return false;
                }

                /// <summary>
                ///   Returns the map identifiers of available maps
                /// </summary>
                public IEnumerable<MapID> GetMapIDs()
                {
                        var maps = GetAll<DataMap>();

                        return maps.Select(map => map.Data.MapID);
                }

                /// <summary>
                ///   Creates a new map (DataMap) and adds it (if it doesnt exist)
                /// </summary>
                public bool BuildMap(MapID newMapID, bool isOutpost, bool isPvE)
                {
                        if (GetMapIDs().Contains(newMapID, newMapID.Comparer())) return true;

                        using (var db = (MySQL)DataBaseProvider.GetDataBase())
                        {
                                var mapID = newMapID.Value;

                                // get the database stuff
                                var dbMaps = from m in db.mapsMasterData
                                          where m.mapID == mapID
                                          select m;

                                // failcheck
                                if (dbMaps.Count() == 0) return false;

                                // take the first of found maps
                                var map = dbMaps.First();

                                // create a new map data object, so we can fill in data ;)
                                var data = new MapData
                                {
                                        MapID = newMapID,
                                        GameMapID = new GameMapID((uint)map.gameMapID),
                                        GameFileID = new GameFileID((uint)map.gameMapFileID),
                                        IsOutpost = isOutpost,
                                        IsPvE = isPvE,
                                        DistrictCountry = 2, // america
                                        DistrictNumber = 1,
                                };

                                // get the spawns: (hopefully dblinq can compute that...)
                                var otp = isOutpost ? 1 : 0;
                                var pve = isPvE ? 1 : 0;
                                var spawns = from s in db.mapsSpawns
                                             where (s.mapID == mapID) &&
                                             (s.isOutpost == otp) &&
                                             (s.isPvE == pve)
                                             select s;

                                // add them
                                foreach (var tmpVector in
                                        spawns.Select(s => new GWVector(s.spawnX, s.spawnY, s.spawnPlane)))
                                {
                                        data.PossibleSpawns.Add(tmpVector);
                                }

                                // failcheck
                                if (data.PossibleSpawns.Count == 0) data.PossibleSpawns.Add(new GWVector(0,0,0));

                                // build the map here, because NPC's are not added to Data but the map itself
                                var newMap = new DataMap(data);

                                // get the npcs:
                                // using otp and pve from before
                                var npcSpawns = from nsp in db.nPcSSpawns
                                                where (nsp.mapID == mapID) &&
                                                (nsp.atOutpost == otp) &&
                                                (nsp.atPvE == pve)
                                                select nsp;

                                // add them
                                if (npcSpawns.Count() != 0)
                                {
                                        foreach (var npcSp in npcSpawns)
                                        {
                                                // get the NPC's masterdata
                                                var npcID = npcSp.npcID;
                                                var dbNpcs = from n in db.nPcSMasterData
                                                             where n.npcID == npcID
                                                             select n;

                                                // failcheck
                                                if (dbNpcs.Count() == 0) continue;
                                                var dbNpc = dbNpcs.First();

                                                // get the npc name
                                                var nameID = npcSp.nameID;
                                                var nameHashes = from n in db.nPcSNames
                                                               where n.nameID == nameID
                                                               select n.nameHash;

                                                // failcheck
                                                if (nameHashes.Count() == 0) continue;
                                                var nameHash = nameHashes.First();

                                                // create new npc data
                                                var npcData = new NpcData
                                                {
                                                        AgentID = new AgentID((uint)data.AgentIDs.RequestID()),                   
                                                        LocalID = new LocalID((uint)npcID),
                                                        NpcFileID = dbNpc.npcFileID,
                                                        ModelHash = dbNpc.modelHash,
                                                        Appearance = dbNpc.appearance,
                                                        Name = new Name(Encoding.Unicode.GetString(nameHash)),
                                                        Scale = dbNpc.scale,
                                                        NpcFlags = dbNpc.professionFlags,
                                                        Position = new GWVector(npcSp.spawnX, npcSp.spawnY, npcSp.plane),
                                                        Rotation = npcSp.rotation,
                                                        IsRotating = false,
                                                        Speed = npcSp.speed,
                                                        Level = (uint)npcSp.level,
                                                        ProfessionPrimary = (byte)npcSp.profession,
                                                        Energy = 100,
                                                        EnergyRegen = 0.3F,
                                                        Health = 100,
                                                        HealthRegen = 0.3F,
                                                        Direction = new GWVector(0, 0, 0),
                                                        Morale = 100,
                                                        VitalStatus = (int)VitalStatus.Alive,
                                                        MoveState = MovementState.MoveKeepDir,
                                                        MoveType = MovementType.Stop
                                                };

                                                newMap.Add(new DataNpc(npcData));
                                        }
                                }

                                // add the map
                                Add(newMap);

                                return true;
                        }
                }

                /// <summary>
                ///   Returns some lines of server stats, and the message of the day
                /// </summary>
                public IEnumerable<string> ServerInfo()
                {
                        var result = new List<String>();

                        result.Add("[Message of the day:]");
                        result.AddRange(MessageOfTheDay);

                        result.Add(string.Format("[Users online:  ] {0}", GetAll<DataClient>().Count()));
                        result.Add(string.Format("[Available maps:] {0}", GetAll<DataMap>().Count()));
                        result.Add(string.Format("[Server Uptime: ] {0}", DateTime.Now.Subtract(StartTime)));

                        return result;
                }

                /// <summary>
                ///   This handler should be attached to the NetworkManager's LostClient event
                /// </summary>
                public override void LostNetworkClientHandler(NetID netID)
                {
                        try
                        {
                                // get the client
                                var tmpClient = Get<DataClient>(netID);

                                // remove the character from the map
                                tmpClient.RemoveCharacter();

                                // remove the client
                                Remove(tmpClient);
                        }
                        catch (Exception)
                        {
                                Debug.WriteLine("Error: NetworkClient[{0}] could not be removed, although it has no connection.", netID.Value);
                        }
                }
        }
}
