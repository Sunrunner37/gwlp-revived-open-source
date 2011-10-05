﻿using System;
using System.Linq;
using GameServer.Enums;
using GameServer.Packets.ToClient;
using GameServer.Packets.ToLoginServer;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;
using ServerEngine.GuildWars.Tools;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 16896)]
        class NotEncP16896_ClientSeed : IPacket
        {
                public class PacketSt16896 : IPacketTemplate
                {
                        public UInt16 Header { get { return 16896; } }
                        [PacketFieldType(ConstSize = true, MaxSize = 64)]
                        public byte[] Seed;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt16896>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        message.PacketTemplate = new PacketSt16896();
                        pParser((PacketSt16896)message.PacketTemplate, message.PacketData);

                        var client = GameServerWorld.Instance.Get<DataClient>(Clients.NetID, message.NetID);
                        DataCharacter chara;

                        // unkown client, probably a dispatching client
                        if (client == null)
                        {
                                byte[] ip;
                                int port;
                                NetworkManager.Instance.GetClientInfo(message.NetID, out ip, out port);

                                // we have a dispatch client, lets check that
                                foreach (var dispatchedClient in World.GetDispatchedClients())
                                {
                                        // get the ip and port of clients with status dispatching
                                        byte[] cIp;
                                        int cPort;
                                        NetworkManager.Instance.GetClientInfo((int)dispatchedClient[Clients.NetID], out cIp, out cPort);

                                        // dont test the port, its different anyways...
                                        if (ip.SequenceEqual(cIp))
                                        {

                                                // we've found a match, now lets update the client object
                                                var newClient = new DataClient(message.NetID, (int)dispatchedClient[Clients.AccID], (int)dispatchedClient[Clients.CharID])
                                                {
                                                        LoginCount = dispatchedClient.LoginCount,
                                                        SecurityKeys = dispatchedClient.SecurityKeys,
                                                        Status = SyncStatus.ConnectionEstablished,
                                                        MapID = dispatchedClient.MapID
                                                };

                                                World.UpdateClient(dispatchedClient, newClient);

                                                // also, we can safely terminate the dispatchedClient
                                                NetworkManager.Instance.RemoveClient((int)dispatchedClient[Clients.NetID]);

                                                // get the char
                                                chara = GameServerWorld.Instance.Get<DataCharacter>(Chars.CharID, newClient[Clients.CharID]);

                                                // and the char object as well
                                                // get some necessary IDs
                                                int localID, agentID;
                                                World.RegisterCharacterIDs(out localID, out agentID, newClient.MapID);

                                                // get the spawn point
                                                var map = GameServerWorld.Instance.Get<DataMap>(Maps.MapID, chara.MapID);
#warning FIXME Supports currently Outposts (and PvE) zones only
                                                MapSpawn spawn;
                                                var spawnEnum = from s in map.Spawns.Values
                                                                where s.IsOutpost && s.IsPvE
                                                                select s;

                                                if (spawnEnum.Count() == 0)
                                                {
                                                        spawn = new MapSpawn
                                                        {
                                                                SpawnX = 1F,
                                                                SpawnY = 1F,
                                                                SpawnPlane = 0
                                                        };
                                                }
                                                else
                                                {
                                                        spawn = spawnEnum.First();
                                                }

                                                // create the character object
                                                var newChar = new DataCharacter(
                                                        (int)newClient[Clients.CharID],
                                                        (int)newClient[Clients.AccID],
                                                        (int)newClient[Clients.NetID],
                                                        localID,
                                                        agentID,
                                                        (string)chara[Chars.Name])
                                                {
                                                        MapID = newClient.MapID,
                                                        IsAtOutpost = chara.IsAtOutpost,
                                                        LastHeartBeat = DateTime.Now,
                                                        PingTime = DateTime.Now,
                                                        CharStats = new CharacterStats()
                                                                {
                                                                        ProfessionPrimary = chara.CharStats.ProfessionPrimary,
                                                                        ProfessionSecondary = chara.CharStats.ProfessionSecondary,
                                                                        Level = chara.CharStats.Level,
                                                                        Morale = 100,
                                                                        SkillBar = chara.CharStats.SkillBar,
                                                                        UnlockedSkills = chara.CharStats.UnlockedSkills,
                                                                        AttPtsFree = chara.CharStats.AttPtsFree,
                                                                        AttPtsTotal = chara.CharStats.AttPtsTotal,
                                                                        SkillPtsFree = chara.CharStats.SkillPtsFree,
                                                                        SkillPtsTotal = chara.CharStats.SkillPtsTotal,
                                                                        Appearance = chara.CharStats.Appearance,
                                                                        Position = {X = spawn.SpawnX, Y = spawn.SpawnY, PlaneZ = spawn.SpawnPlane},
                                                                        Direction = new GWVector(0, 0, 0),
                                                                        MoveState = MovementState.NotMoving,
                                                                        IsRotating = false,
                                                                        Rotation = BitConverter.ToSingle(new byte[] { 0xB6, 0xC0, 0x4F, 0xBF }, 0),
                                                                        Speed = 288F,
                                                                        ChatPrefix = chara.CharStats.ChatPrefix,
                                                                        ChatColor = chara.CharStats.ChatColor,
                                                                        Commands = chara.CharStats.Commands,
                                                                }
                                                };
                                                
                                                // add the char
                                                World.UpdateChar(chara , newChar);

                                                // finally, kick the old socket
                                                NetworkManager.Instance.RemoveClient((int)dispatchedClient[Clients.NetID]);

                                                //// login server acknowledgement
                                                //var msg = new NetworkMessage(World.LoginSrvNetID);
                                                //// set the message type
                                                //msg.PacketTemplate = new P65285_ClientDispatchAcknowledgement.PacketSt65285()
                                                //{
                                                //        AccID = (uint)(int)newChar[Chars.AccID]
                                                //};
                                                //QueuingService.PostProcessingQueue.Enqueue(msg);

                                                // and break, as the World.Clients collection was altered anyways.
                                                break;
                                        }
                                }
                        }

                        // refresh client and character
                        client = GameServerWorld.Instance.Get<DataClient>(Clients.NetID, message.NetID);
                        chara = GameServerWorld.Instance.Get<DataCharacter>(Chars.CharID, client[Clients.CharID]);
                        
                        if (client.Status == SyncStatus.ConnectionEstablished)
                        {
                                client.InitCryptSeed = ((PacketSt16896)message.PacketTemplate).Seed;

                                // send server seed:
                                //
                                var msg = new NetworkMessage(message.NetID);
                                // set the message type
                                msg.PacketTemplate = new NotEncP5633_ServerSeed.PacketSt5633();
                                // set the message data
                                ((NotEncP5633_ServerSeed.PacketSt5633)msg.PacketTemplate).Seed = new byte[20];
                                // send it
                                QueuingService.PostProcessingQueue.Enqueue(msg);

                                client.Status = SyncStatus.TriesToLoadInstance;

                                // send first instance load packets
                                bool isOutpost = chara.IsAtOutpost;

                                // Note: INSTANCE LOAD HEADER
                                var ilHeader = new NetworkMessage(message.NetID);
                                ilHeader.PacketTemplate = new P370_InstanceLoadHead.PacketSt370();
                                // data
                                if (isOutpost)
                                {
                                        ((P370_InstanceLoadHead.PacketSt370)ilHeader.PacketTemplate).Data1 = 0x3F;
                                        ((P370_InstanceLoadHead.PacketSt370)ilHeader.PacketTemplate).Data2 = 0x3F;
                                }
                                else
                                {
                                        ((P370_InstanceLoadHead.PacketSt370)ilHeader.PacketTemplate).Data1 = 0x1F;
                                        ((P370_InstanceLoadHead.PacketSt370)ilHeader.PacketTemplate).Data2 = 0x1F;
                                }
                                ((P370_InstanceLoadHead.PacketSt370)ilHeader.PacketTemplate).Data3 = 0x00;
                                ((P370_InstanceLoadHead.PacketSt370)ilHeader.PacketTemplate).Data4 = 0x00;
                                // send it
                                QueuingService.PostProcessingQueue.Enqueue(ilHeader);

                                // Note: INSTANCE LOAD CHAR NAME
                                var ilChar = new NetworkMessage(message.NetID);
                                ilChar.PacketTemplate = new P371_InstanceLoadCharName.PacketSt371();
                                // data
                                ((P371_InstanceLoadCharName.PacketSt371)ilChar.PacketTemplate).CharName = (string)chara[Chars.Name];
                                // send it
                                QueuingService.PostProcessingQueue.Enqueue(ilChar);

                                // Note: INSTANCE LOAD DISTRICT INFO
                                var ilDInfo = new NetworkMessage(message.NetID);
                                ilDInfo.PacketTemplate = new P395_InstanceLoadDistrictInfo.PacketSt395();
                                // data
                                ((P395_InstanceLoadDistrictInfo.PacketSt395)ilDInfo.PacketTemplate).LocalID = (uint)(int)chara[Chars.LocalID];
                                var gameMapID = (int)GameServerWorld.Instance.Get<DataMap>(Maps.MapID, chara.MapID)[Maps.GameMapID];
                                ((P395_InstanceLoadDistrictInfo.PacketSt395)ilDInfo.PacketTemplate).GameMapID = (ushort)gameMapID;
                                if (isOutpost)
                                {
                                        ((P395_InstanceLoadDistrictInfo.PacketSt395)ilDInfo.PacketTemplate).DistrictNumber = 1;
                                        ((P395_InstanceLoadDistrictInfo.PacketSt395)ilDInfo.PacketTemplate).DistrictRegion = 0;
                                        ((P395_InstanceLoadDistrictInfo.PacketSt395)ilDInfo.PacketTemplate).IsOutpost = 0;
                                        ((P395_InstanceLoadDistrictInfo.PacketSt395)ilDInfo.PacketTemplate).ObserverMode = 0;
                                        ((P395_InstanceLoadDistrictInfo.PacketSt395)ilDInfo.PacketTemplate).Data1 = 0;
                                }
                                else
                                {
                                        ((P395_InstanceLoadDistrictInfo.PacketSt395)ilDInfo.PacketTemplate).DistrictNumber = 0;
                                        ((P395_InstanceLoadDistrictInfo.PacketSt395)ilDInfo.PacketTemplate).DistrictRegion = 0;
                                        ((P395_InstanceLoadDistrictInfo.PacketSt395)ilDInfo.PacketTemplate).IsOutpost = 1;
                                        ((P395_InstanceLoadDistrictInfo.PacketSt395)ilDInfo.PacketTemplate).ObserverMode = 0;
                                        ((P395_InstanceLoadDistrictInfo.PacketSt395)ilDInfo.PacketTemplate).Data1 = 3;
                                }
                                // send it
                                QueuingService.PostProcessingQueue.Enqueue(ilDInfo);


                                return true;
                        }
                        // if the client is in any different sync state, kick it
                        World.KickClient(Clients.NetID, message.NetID);
                                
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt16896> pParser;
        }
}
