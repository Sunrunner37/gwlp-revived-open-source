using System;
using System.IO;
using System.Linq;
using GameServer.DataBase;
using GameServer.Enums;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.StaticConvert;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;
using ServerEngine.GuildWars.Tools;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 1280)]
        public class NotEncP1280_VerifyClient : IPacket
        {
                public class PacketSt1280 : IPacketTemplate
                {
                        public UInt16 Header { get { return 1280; } }
                        public UInt16 Data1;
                        public UInt32 Data2;
                        public UInt32 Data3;
                        [PacketFieldType(ConstSize = true, MaxSize = 4)]
                        public byte[] Key1;
                        public UInt32 Data4;
                        [PacketFieldType(ConstSize = true, MaxSize = 4)]
                        public byte[] Key2;
                        [PacketFieldType(ConstSize = true, MaxSize = 16)]
                        public byte[] AccHash;
                        [PacketFieldType(ConstSize = true, MaxSize = 16)]
                        public byte[] CharHash;
                        public UInt32 Data5;
                        public UInt32 Data6;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt1280>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        message.PacketTemplate = new PacketSt1280();
                        pParser((PacketSt1280)message.PacketTemplate, message.PacketData);

                        var key1 = ((PacketSt1280) message.PacketTemplate).Key1;
                        var key2 = ((PacketSt1280) message.PacketTemplate).Key2;

                        var clients = World.GetUnauthorizedClients();

                        // check the security keys
                        var verfClient = clients.FirstOrDefault(client => client.SecurityKeys[0].SequenceEqual(key1) && client.SecurityKeys[1].SequenceEqual(key2));

                        if (verfClient != null)
                        {
                                var newClient = new DataClient(message.NetID,(int)verfClient[Clients.AccID],(int)verfClient[Clients.CharID])
                                {
                                        LoginCount = verfClient.LoginCount,
                                        SecurityKeys = verfClient.SecurityKeys,
                                        Status = SyncStatus.ConnectionEstablished,
                                        MapID = verfClient.MapID,
                                };

                                World.UpdateClient(verfClient, newClient);

                                // add char
                                using (var db = (MySQL)DataBaseProvider.GetDataBase())
                                {
                                        // get the char id
                                        var chID = (int)newClient[Clients.CharID];
                                        
                                        // get the char db object
                                        var ch = (from c in db.charsMasterData
                                                        where c.charID == chID
                                                        select c).First();
                                                
                                        // get some necessary IDs
                                        int localID, agentID;
                                        World.RegisterCharacterIDs(out localID, out agentID, newClient.MapID);

                                        // get the appearance
                                        var appearance = new MemoryStream();
                                        RawConverter.WriteByte((byte)((ch.lookHeight << 4) | ch.lookSex), appearance);
                                        RawConverter.WriteByte((byte)((ch.lookHairColor << 4) | ch.lookSkinColor), appearance);
                                        RawConverter.WriteByte((byte)((ch.professionPrimary << 4) | ch.lookHairStyle), appearance);
                                        RawConverter.WriteByte((byte)((ch.lookCampaign << 4) | ch.lookSex), appearance);

                                        // get the spawn point
                                        var map = GameServerWorld.Instance.Get<DataMap>(Maps.MapID, newClient.MapID);

                                        var spawns = from s in map.Spawns.Values
                                                     where s.IsOutpost && s.IsPvE
                                                     select s;
                                        
                                        var spawn = spawns.Count() != 0? spawns.First() :

                                                new MapSpawn()
                                                {
                                                        IsOutpost = true,
                                                        IsPvE = true,
                                                        SpawnID = 0,
                                                        SpawnPlane = 0,
                                                        SpawnRadius = 0,
                                                        SpawnX = 0,
                                                        SpawnY = 0,
                                                        TeamSpawnNumber = 0,
                                                };
                                        

                                        // get the chat stuff
                                        var accID = (int)newClient[Clients.AccID];
                                        var accGrpID = (from a in db.accountsMasterData
                                                        where a.accountID == accID
                                                        select a).First().groupID;

                                        var grp = (from g in db.groupsMasterData
                                                   where g.groupID == accGrpID
                                                   select g).First();
                                                
                                        // create the new char
                                        var newChar = new DataCharacter(chID, 
                                                (int)newClient[Clients.AccID], 
                                                (int)newClient[Clients.NetID],
                                                localID,
                                                agentID,
                                                ch.charName)
                                        {
                                                MapID = newClient.MapID,
                                                IsAtOutpost = true,
                                                LastHeartBeat = DateTime.Now,
                                                PingTime = DateTime.Now,
                                                CharStats =
                                                {
                                                        ProfessionPrimary = (byte)ch.professionPrimary,
                                                        ProfessionSecondary = (byte)ch.professionSecondary,
                                                        Level = ch.level,
                                                        Morale = 100,
                                                        SkillBar = ch.skillBar,
                                                        UnlockedSkills = ch.skillsAvailable,
                                                        AttPtsFree = ch.attrPtsFree,
                                                        AttPtsTotal = ch.attrPtsTotal,
                                                        SkillPtsFree = ch.skillPtsFree,
                                                        SkillPtsTotal = ch.skillPtsTotal,
                                                        Appearance = appearance.ToArray(),
                                                        Position = {X = spawn.SpawnX, Y = spawn.SpawnY, PlaneZ = spawn.SpawnPlane},
                                                        Direction = new GWVector(0, 0, 0),
                                                        MoveState = MovementState.NotMoving,
                                                        TrapezoidIndex = 0,
                                                        IsRotating = false,
                                                        Rotation = BitConverter.ToSingle(new byte[] { 0xB6, 0xC0, 0x4F, 0xBF }, 0),
                                                        Speed = 288F,
                                                        ChatPrefix = grp.groupPrefix,
                                                        ChatColor = (byte)grp.groupChatColor,
                                                }
                                        };

                                        

                                        // dont forget to add available commands (its easier after the creation of newChar)
                                        var cmds = from g in db.groupsCommands
                                                        select g;

                                        var grpID = grp.groupID;
                                        foreach (var cmd in cmds)
                                        {
                                                newChar.CharStats.Commands.Add(cmd.commandName, (grpID >= cmd.groupID));  
                                        }

                                        // add the char
                                        World.AddChar(newChar);
                                }
                        }

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt1280> pParser;
        }
}
