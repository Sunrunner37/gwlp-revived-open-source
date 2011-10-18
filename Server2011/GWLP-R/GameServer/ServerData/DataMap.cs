using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GameServer.DataBase;
using GameServer.Enums;
using GameServer.ServerData.DataInterfaces;
using ServerEngine;
using ServerEngine.DataManagement;
using ServerEngine.DataManagement.DataInterfaces;
using ServerEngine.GuildWars.DataInterfaces;
using ServerEngine.GuildWars.DataWrappers.Chars;
using ServerEngine.GuildWars.DataWrappers.Clients;
using ServerEngine.GuildWars.DataWrappers.Maps;
using ServerEngine.GuildWars.Tools;
using ServerEngine.PacketManagement.StaticConvert;

namespace GameServer.ServerData
{
        public sealed class DataMap : DataManager, IIdentifiableData<MapData>
        {
                private readonly object objLock = new object();

                private MapData data;

                /// <summary>
                ///   Create a new instance of the class
                /// </summary>
                public DataMap(MapData data)
                {
                        lock (objLock)
                        {
                                this.data = data;

                                Debug.WriteLine(string.Format("Created new {0}", GetType().Name));
                        }
                }

                #region Implementation of IEnumerable

                public IEnumerator<IWrapper> GetEnumerator()
                {
                        lock (objLock)
                        {
                                return (new List<IWrapper>
                                {
                                        // IHasMapData
#warning FIXME/BUG This prevents districts and/or hostile maps (because they would have the same map id)
                                        Data.MapID,
                                }).GetEnumerator();
                        }
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                        return GetEnumerator();
                }

                #endregion

                #region Implementation of IIdentifiableData<MapData>

                public MapData Data
                {
                        get { lock(objLock) return data; }
                        set { lock(objLock) data = value; }
                }

                #endregion

                /// <summary>
                ///   Adds a character or NPC and registers IDs
                ///   New method to replace World's Add()
                /// </summary>
                public bool Add<TData>(IIdentifiableData<TData> value)
                        where TData : class, IHasCharData
                {
                        try
                        {
                                // get the right dict
                                var tmpDict = worldData[value.GetType()];

                                // thats the map-only part:
                                // register ID's
                                lock (objLock)
                                {
                                        value.Data.AgentID = new AgentID((uint)data.AgentIDs.RequestID());
                                        value.Data.LocalID = new LocalID((uint)data.LocalIDs.RequestID());
                                }

                                // add the value
                                return tmpDict.AddAll(value);
                        }
                        // we've got no dict of it ;)
                        catch (KeyNotFoundException)
                        {
                                // create a new dict
                                var tmpDict = new MultiKeyDictionary<IEnumerable<IWrapper>>();

                                // thats the map-only part:
                                // register ID's
                                lock (objLock)
                                {
                                        value.Data.AgentID = new AgentID((uint)data.AgentIDs.RequestID());
                                        value.Data.LocalID = new LocalID((uint)data.LocalIDs.RequestID());
                                }

                                // add the value
                                if (tmpDict.AddAll(value))
                                {
                                        // add the dict
                                        worldData.Add(value.GetType(), tmpDict);

                                        return true;
                                }

                                return false;
                        }

                }

                /// <summary>
                ///   Try to remove a value that implements IIdentifiableData and IHasCharData
                /// </summary>
                public bool Remove<TData>(IIdentifiableData<TData> value)
                        where TData : class, IHasCharData, IHasNetworkData
                {
                        var netID = value.Data.NetID;

                        try
                        {
                                // get the right dict
                                var tmpDict = worldData[value.GetType()];

                                // remove the old value
                                tmpDict.RemoveAll(value as IEnumerable<IWrapper>);

                                // thats the map-only part:
                                // free ids
                                lock (objLock)
                                {
                                        data.AgentIDs.FreeID((int)value.Data.AgentID.Value);
                                        data.LocalIDs.FreeID((int)value.Data.LocalID.Value);
                                }

                                // message
                                Debug.WriteLine("{0}[{1}] removed from map.", value.GetType(), netID.Value);

                                return true;
                        }
                        catch (Exception)
                        {
                                Debug.WriteLine("Error: {0}[{1}] could not be removed from map.", value.GetType(), netID.Value);
                                return false;
                        }
                }

                /// <summary>
                ///   Add a char directly via charID
                /// </summary>
                /// <param name="charID"></param>
                /// <returns></returns>
                public bool LoadCharacter(CharID charID, out DataCharacter newlyCreatedChar)
                {
                        lock (objLock)
                        {

                                var client = GameServerWorld.Instance.Get<DataClient>(charID);

                                // get the database stuff
                                using (var db = (MySQL)DataBaseProvider.GetDataBase())
                                {
                                        // get the char db object
                                        var chs = from c in db.charsMasterData
                                                  where c.charID == charID.Value
                                                  select c;

                                        // failcheck
                                        if (chs.Count() == 0)
                                        {
                                                newlyCreatedChar = null;
                                                return false;
                                        }

                                        var ch = chs.First();

                                        // get the appearance
                                        var appearance = new MemoryStream();
                                        RawConverter.WriteByte((byte)((ch.lookHeight << 4) | ch.lookSex), appearance);
                                        RawConverter.WriteByte((byte)((ch.lookHairColor << 4) | ch.lookSkinColor), appearance);
                                        RawConverter.WriteByte((byte)((ch.professionPrimary << 4) | ch.lookHairStyle), appearance);
                                        RawConverter.WriteByte((byte)((ch.lookCampaign << 4) | ch.lookSex), appearance);

                                        // get a random spawn point
                                        var ran = new Random();
                                        var spawn = data.PossibleSpawns[ran.Next(0, data.PossibleSpawns.Count - 1)];
                                        // if we have a pvp map and the client team number could be an index in possible spawns
                                        if (!data.IsPvE && (client.Data.TeamNumber < Data.PossibleSpawns.Count))
                                        {
                                                spawn = data.PossibleSpawns[client.Data.TeamNumber];
                                        }


                                        // get the chat stuff
                                        var accGrpIDs = from a in db.accountsMasterData
                                                        where a.accountID == client.Data.AccID.Value
                                                        select a;
                                        // failcheck
                                        if (accGrpIDs.Count() == 0)
                                        {
                                                newlyCreatedChar = null;
                                                return false;
                                        }

                                        var accGrpID = accGrpIDs.First().groupID;

                                        var grps = from g in db.groupsMasterData
                                                   where g.groupID == accGrpID
                                                   select g;
                                        //failcheck
                                        if (grps.Count() == 0)
                                        {
                                                newlyCreatedChar = null;
                                                return false;
                                        }

                                        var grp = grps.First();

                                        // create the new char
                                        var charData = new CharacterData
                                        {
                                                Name = new Name(ch.charName),

                                                MapID = data.MapID,
                                                GameMapID = data.GameMapID,
                                                GameFileID = data.GameFileID,
                                                IsOutpost = data.IsOutpost,

                                                ProfessionPrimary = (byte)ch.professionPrimary,
                                                ProfessionSecondary = (byte)ch.professionSecondary,
                                                Level = (uint)ch.level,
                                                Morale = 100,

                                                SkillPtsFree = ch.skillPtsFree,
                                                SkillPtsTotal = ch.skillPtsTotal,
                                                SkillBar = ch.skillBar,
                                                UnlockedSkills = ch.skillsAvailable,

                                                AttPtsFree = ch.attrPtsFree,
                                                AttPtsTotal = ch.attrPtsTotal,

                                                Appearance = appearance.ToArray(),

                                                Position = { X = spawn.X, Y = spawn.Y, PlaneZ = spawn.PlaneZ },
                                                Direction = new GWVector(0, 0, 0),
                                                MoveState = MovementState.NotMoving,
                                                TrapezoidIndex = 0,
                                                IsRotating = false,
                                                Rotation = 0xBF4FC0B6,
                                                Speed = 288F,
                                                ChatPrefix = grp.groupPrefix,
                                                ChatColor = (byte)grp.groupChatColor,
                                        };

                                        // copy some data from the client
                                        charData.Paste<IHasNetworkData>(client.Data);
                                        charData.Paste<IHasClientData>(client.Data);
                                        charData.Paste<IHasTeamData>(client.Data);

                                        // dont forget to add available commands (its easier after the creation of newCharData)
                                        var cmds = from g in db.groupsCommands
                                                   select g;

                                        var grpID = grp.groupID;
                                        foreach (var cmd in cmds)
                                        {
                                                charData.ChatCommands.Add(cmd.commandName, (grpID >= cmd.groupID));
                                        }

                                        // add the char
                                        newlyCreatedChar = new DataCharacter(charData);
                                        return Add(newlyCreatedChar);
                                }
                        }
                }
        }

        public class MapData :
                IHasMapData,
                IHasActionQueueData<DataMap>,
                IHasDistrictData,
                IHasObjectIDManagerData,
                IHasSpawnData
        {
                public MapData()
                {
                        ActionQueue = new ConcurrentQueue<Action<DataMap>>();
                        AgentIDs = new IDManager(1, 1000);
                        LocalIDs = new IDManager(1, 1000);
                        PossibleSpawns = new List<GWVector>();
                }

                #region Implementation of IHasMapData

                public GameFileID GameFileID { get; set; }
                public GameMapID GameMapID { get; set; }
                public MapID MapID { get; set; }

                #endregion

                #region Implementation of IHasActionQueueData<DataMap>

                public ConcurrentQueue<Action<DataMap>> ActionQueue { get; set; }

                #endregion

                #region Implementation of IHasDistrictData

                public bool IsOutpost { get; set; }
                public bool IsPvE { get; set; }
                public int DistrictCountry { get; set; }
                public int DistrictNumber { get; set; }

                #endregion

                #region Implementation of IHasObjectIDManagerData

                public IDManager AgentIDs { get; private set; }
                public IDManager LocalIDs { get; private set; }

                #endregion

                #region Implementation of IHasSpawnData

                public List<GWVector> PossibleSpawns { get; set; }

                #endregion
        }
}
