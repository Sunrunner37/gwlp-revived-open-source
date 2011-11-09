using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Enums;
using GameServer.Packets.ToClient;
using GameServer.Packets.ToLoginServer;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.DataManagement.DataWrappers;
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
                        var pack = new PacketSt16896();
                        pParser(pack, message.PacketData);

                        var client = GameServerWorld.Instance.Get<DataClient>(message.NetID);

                        // unkown client, probably a dispatching client
                        if (client == null)
                        {
                                // we have a dispatch client, lets check that

                                // get the ip and port of the client
                                byte[] cIp;
                                uint cPort;
                                NetworkManager.Instance.GetClientInfo(message.NetID, out cIp, out cPort);

                                IEnumerable<DataClient> clients;

                                // get all dispatch clients if possible
                                if (!GameServerWorld.Instance.ClientWhereStatus(SyncStatus.Dispatching, out clients))
                                {
                                        return true;
                                }

                                foreach (var dpClient in clients)
                                {
                                        // check the ip
                                        if (!dpClient.Data.IPAddress.Value.SequenceEqual(cIp)) continue;

                                        // check if the map exists (necessary for dispatch clients)
                                        var tmpMap = GameServerWorld.Instance.Get<DataMap>(dpClient.Data.MapID);
                                        // kick the client if not
                                        if (tmpMap == null)
                                        {
                                                NetworkManager.Instance.RemoveClient(message.NetID);

                                                // tell the packetman that everything is OK
                                                return true;
                                        }

                                        // we've found a match, now lets update the client object
                                        var newClientData = new ClientData
                                        {
                                                NetID = message.NetID,
                                                IPAddress = new IPAddress(cIp),
                                                Port = new Port(cPort),
                                                AccID = dpClient.Data.AccID,
                                                CharID = dpClient.Data.CharID,
                                                SyncCount = dpClient.Data.SyncCount,
                                                SecurityKeys = dpClient.Data.SecurityKeys,
                                                Status = SyncStatus.ConnectionEstablished,
                                                MapID = tmpMap.Data.MapID,
                                                GameMapID = tmpMap.Data.GameMapID,
                                                GameFileID = tmpMap.Data.GameFileID
                                        };

                                        var newClient = new DataClient(newClientData);
                                        GameServerWorld.Instance.Update(dpClient, newClient);

                                        // add the character
                                        DataCharacter newChar;
                                        if (!tmpMap.LoadCharacter(newClient.Data.CharID, out newChar))
                                        {
                                                // if the character cannot be added, kick the client
                                                GameServerWorld.Instance.Kick(newClient);
                                        }

                                        // set the client's character
                                        newClient.Character = newChar;

                                        // finally, kick the old socket
                                        if (dpClient.Data.NetID.Value != 0)
                                        {
                                                NetworkManager.Instance.RemoveClient(dpClient.Data.NetID);
                                        }

                                        // set the new reference to client
                                        client = newClient;

                                        // and break, as the GSWorld.Clients collection was altered anyways.
                                        break;
                                }
                        }

                        // failcheck
                        if (client == null)
                        {
                                // kick the client if the client couldnt be created
                                // this should never be the case!
                                NetworkManager.Instance.RemoveClient(message.NetID);
                                return true;
                        }

                        // get the map and character
                        var map = GameServerWorld.Instance.Get<DataMap>(client.Data.MapID);
                        var chara = map.Get<DataCharacter>(client.Data.CharID);

                        if (client.Data.Status == SyncStatus.ConnectionEstablished)
                        {
                                client.Data.EncryptionSeed = pack.Seed;

                                // Note: SERVER SEED
                                var msg = new NetworkMessage(message.NetID)
                                {
                                        PacketTemplate = new NotEncP5633_ServerSeed.PacketSt5633
                                        {
                                                Seed = new byte[20],
                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(msg);

                                // update the clients status
                                client.Data.Status = SyncStatus.TriesToLoadInstance;

                                // Note: INSTANCE LOAD HEADER
                                var ilHeader = new NetworkMessage(message.NetID)
                                {
                                        PacketTemplate = new P370_InstanceLoadHead.PacketSt370()
                                        {
                                                Data1 = (byte)(chara.Data.IsOutpost ? 0x3F : 0x1F),
                                                Data2 = (byte)(chara.Data.IsOutpost ? 0x3F : 0x1F),
                                                Data3 = 0x00,
                                                Data4 = 0x00,
                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(ilHeader);


                                if (map.Data.GameMapID.Value != 0)
                                {
                                        // Note: INSTANCE LOAD CHAR NAME
                                        var ilChar = new NetworkMessage(message.NetID)
                                        {
                                                PacketTemplate = new P371_InstanceLoadCharName.PacketSt371
                                                {
                                                        CharName = chara.Data.Name.Value,
                                                }
                                        };
                                        QueuingService.PostProcessingQueue.Enqueue(ilChar);

                                        // Note: INSTANCE LOAD DISTRICT INFO
                                        var ilDInfo = new NetworkMessage(message.NetID)
                                        {
                                                PacketTemplate = new P395_InstanceLoadDistrictInfo.PacketSt395
                                                {
                                                        LocalID = chara.Data.LocalID.Value,
                                                        GameMapID = (ushort)map.Data.GameMapID.Value,
                                                        DistrictNumber = (ushort)(chara.Data.IsOutpost ? map.Data.DistrictNumber : 0),
                                                        DistrictRegion = (ushort)(chara.Data.IsOutpost ? map.Data.DistrictCountry : 0),
                                                        IsOutpost = (byte)(chara.Data.IsOutpost ? 1 : 0),
                                                        ObserverMode = 0,
                                                        Data1 = (byte)(chara.Data.IsOutpost ? 0 : 3),
                                                }
                                        };
                                        QueuingService.PostProcessingQueue.Enqueue(ilDInfo);
                                        }
                                else
                                {
                                        client.Character = new DataCharacter();
                                        var unknown = new NetworkMessage(message.NetID)
                                        {
                                                PacketTemplate = new Packet379.PacketSt379()
                                        };
                                        QueuingService.PostProcessingQueue.Enqueue(unknown);
                                }

                                return true;
                        }

                        // if the client is in any different sync state, kick it
                        GameServerWorld.Instance.Kick(client);
                                
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt16896> pParser;
        }
}
