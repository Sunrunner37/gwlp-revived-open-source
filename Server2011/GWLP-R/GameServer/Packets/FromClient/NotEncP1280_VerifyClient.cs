using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Enums;
using GameServer.ServerData;
using ServerEngine.DataManagement.DataWrappers;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

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
                        var pack = new PacketSt1280();
                        pParser(pack, message.PacketData);

                        var key1 = pack.Key1;
                        var key2 = pack.Key2;

                        IEnumerable<DataClient> clients;
                        // get unauthorized clients or kick
                        if (!GameServerWorld.Instance.ClientWhereStatus(SyncStatus.Unauthorized, out clients))
                        {
                                if (!GameServerWorld.Instance.ClientWhereStatus(SyncStatus.Dispatching, out clients))
                                {
                                        NetworkManager.Instance.RemoveClient(message.NetID);

                                        // tell the packetman that everything is OK
                                        return true;
                                }
                        }

                        // check if we've got a client with that keys
                        var verfClient = clients.FirstOrDefault(c => c.Data.SecurityKeys[0].SequenceEqual(key1) && c.Data.SecurityKeys[1].SequenceEqual(key2));

                        if (verfClient != null)
                        {
                                // check if the map exists (necessary for dispatch clients)
                                var map = GameServerWorld.Instance.Get<DataMap>(verfClient.Data.MapID);
                                // kick the client if not
                                if (map == null)
                                {
                                        NetworkManager.Instance.RemoveClient(message.NetID);

                                        // tell the packetman that everything is OK
                                        return true;
                                }

                                // get some network stuff
                                byte[] ip;
                                uint port;
                                NetworkManager.Instance.GetClientInfo(message.NetID, out ip, out port);

                                var newClientData = new ClientData
                                {
                                        NetID = message.NetID,
                                        IPAddress = new IPAddress(ip),
                                        Port = new Port(port),
                                        AccID = verfClient.Data.AccID,
                                        CharID = verfClient.Data.CharID,
                                        SyncCount = verfClient.Data.SyncCount,
                                        SecurityKeys = verfClient.Data.SecurityKeys,
                                        Status = SyncStatus.ConnectionEstablished,
                                        MapID = map.Data.MapID,
                                        GameMapID = map.Data.GameMapID,
                                        GameFileID = map.Data.GameFileID
                                };

                                var newClient = new DataClient(newClientData);
                                GameServerWorld.Instance.Update(verfClient, newClient);

                                // add the character
                                DataCharacter newChar;
                                if (!map.LoadCharacter(newClient.Data.CharID, out newChar))
                                {
                                        // if the character cannot be added, kick the client
                                        GameServerWorld.Instance.Kick(newClient);
                                }

                                // set the client's character
                                newClient.Character = newChar;

                                // finally, kick the old socket if we had a dispatching client)
                                if (verfClient.Data.Status == SyncStatus.Dispatching && verfClient.Data.NetID.Value != 0)
                                {
                                        NetworkManager.Instance.RemoveClient(verfClient.Data.NetID);
                                }
                        }

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt1280> pParser;
        }
}
