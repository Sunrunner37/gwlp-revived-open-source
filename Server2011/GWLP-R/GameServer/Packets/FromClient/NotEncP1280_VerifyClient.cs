using System;
using System.IO;
using System.Linq;
using GameServer.DataBase;
using GameServer.Enums;
using GameServer.ServerData;
using ServerEngine.DataBase;
using ServerEngine.PacketManagement.StaticConvert;
using ServerEngine.ProcessorQueues;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;
using ServerEngine.Tools;

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
                        Client verfClient = null;
                        foreach (var client in clients)
                        {
                                lock (client)
                                {
                                        if (client.SecurityKeys[0].SequenceEqual(key1) && client.SecurityKeys[1].SequenceEqual(key2))
                                        {
                                                verfClient = client;
                                                break;
                                        }
                                }
                        }

                        if (verfClient != null)
                        {
                                lock (verfClient)
                                {
                                        var newClient = new Client(message.NetID,(int)verfClient[Clients.AccID],(int)verfClient[Clients.CharID])
                                                                {
                                                                        LoginCount = verfClient.LoginCount,
                                                                        SecurityKeys = verfClient.SecurityKeys,
                                                                        Status = SyncState.ConnectionEstablished,
                                                                };
                                        newClient.MapID = verfClient.MapID;

                                        World.UpdateClient(verfClient, newClient);

                                        // add char
                                        using (var db = (MySQL)DataBaseProvider.GetDataBase())
                                        {
                                                var chID = (int)newClient[Clients.CharID];
                                                // execute here
                                                var ch = (from c in db.charsMasterData
                                                         where c.charID == chID
                                                         select c).First();
                                                
                                                // get some necessary IDs
                                                int localID, agentID;
                                                World.RegisterCharacterIDs(out localID, out agentID, newClient.MapID);
                                                
                                                var newChar = new Character(chID, 
                                                        (int)newClient[Clients.AccID], 
                                                        (int)newClient[Clients.NetID],
                                                        localID,
                                                        agentID,
                                                        ch.charName);

                                                newChar.MapID = newClient.MapID;
                                                newChar.IsAtOutpost = true;
                                                newChar.LastHeartBeat = DateTime.Now;
                                                newChar.PingTime = DateTime.Now;
                                                newChar.CharStats.ProfessionPrimary = (byte)ch.professionPrimary;
                                                newChar.CharStats.ProfessionSecondary = (byte)ch.professionSecondary;
                                                newChar.CharStats.Level = ch.level;
                                                newChar.CharStats.Morale = 100;
                                                newChar.CharStats.SkillBar = ch.skillBar;
                                                newChar.CharStats.UnlockedSkills = ch.skillsAvailable;
                                                newChar.AttPtsFree = ch.attrPtsFree;
                                                newChar.AttPtsTotal = ch.attrPtsTotal;
                                                newChar.SkillPtsFree = ch.skillPtsFree;
                                                newChar.SkillPtsTotal = ch.skillPtsTotal;

                                                // appearance
                                                var appearance = new MemoryStream();
                                                RawConverter.WriteByte((byte)((ch.lookHeight << 4) | ch.lookSex), appearance);
                                                RawConverter.WriteByte((byte)((ch.lookHairColor << 4) | ch.lookSkinColor), appearance);
                                                RawConverter.WriteByte((byte)((ch.professionPrimary << 4) | ch.lookHairStyle), appearance);
                                                RawConverter.WriteByte((byte)((ch.lookCampaign << 4) | ch.lookSex), appearance);
                                                newChar.CharStats.Appearance = appearance.ToArray();

                                                // spawn point
                                                Map map;
                                                lock (map = World.GetMap(Maps.MapID, newChar.MapID))
                                                {
                                                        var spawn = (from s in map.Spawns.Values
                                                                     where s.IsOutpost && s.IsPvE
                                                                     select s).First();

                                                        newChar.CharStats.Position.X = spawn.SpawnX;
                                                        newChar.CharStats.Position.Y = spawn.SpawnY;
                                                        newChar.CharStats.Position.PlaneZ = spawn.SpawnPlane;
                                                        newChar.CharStats.Direction = new GWVector(0,0,0);
                                                        newChar.CharStats.MoveState = MovementState.NotMovingHandled;
                                                        newChar.CharStats.IsRotating = false;
                                                }

                                                newChar.CharStats.Rotation = BitConverter.ToSingle(new byte[]{0xB6, 0xC0, 0x4F, 0xBF}, 0);

                                                newChar.CharStats.Speed = 288F;

                                                // get the chat stuff
                                                var accID = (int) newClient[Clients.AccID];
                                                var accGrpID = (from a in db.accountsMasterData
                                                           where a.accountID == accID
                                                           select a).First().groupID;

                                                var grp = (from g in db.groupsMasterData
                                                             where g.groupID == accGrpID
                                                             select g).First();

                                                newChar.ChatPrefix = grp.groupPrefix;
                                                newChar.ChatColor = (byte)grp.groupChatColor;

                                                var cmds = from g in db.groupsCommands
                                                            select g;

                                                var grpID = grp.groupID;
                                                foreach (var cmd in cmds)
                                                {
                                                      newChar.Commands.Add(cmd.commandName, (grpID >= cmd.groupID));  
                                                }

                                                // add the char
                                                World.AddChar(newChar);
                                        }
                                }
                        }

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt1280> pParser;
        }
}
