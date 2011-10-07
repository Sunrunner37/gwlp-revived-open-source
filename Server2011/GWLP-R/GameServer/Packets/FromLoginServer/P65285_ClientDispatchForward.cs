using System;
using GameServer.Enums;
using GameServer.ServerData;
using ServerEngine.GuildWars.DataWrappers.Clients;
using ServerEngine.GuildWars.DataWrappers.Maps;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromLoginServer
{
        [PacketAttributes(IsIncoming = true, Header = 65285)]
        public class P65285_ClientDispatchForward : IPacket
        {
                public class PacketSt65285 : IPacketTemplate
                {
                        public UInt16 Header { get { return 65285; } }
                        [PacketFieldType(ConstSize = true, MaxSize = 4)]
                        public byte[] Key1;
                        [PacketFieldType(ConstSize = true, MaxSize = 4)]
                        public byte[] Key2;
                        public UInt32 AccID;
                        public UInt32 CharID;
                        public UInt32 MapID;
                        public byte IsOutpost;
                        public byte IsPvE;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt65285>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        var pack = new PacketSt65285();
                        pParser(pack, message.PacketData);

                        // create a new client
                        var newClientData = new ClientData
                        {
                                AccID = new AccID(pack.AccID),
                                CharID = new CharID(pack.CharID),
                                MapID = new MapID(pack.MapID),
                                SecurityKeys = new[] { pack.Key1, pack.Key2 },
                                Status = SyncStatus.Dispatching,
                        };

                        // try to add the client
                        var added = GameServerWorld.Instance.Add(new DataClient(newClientData));

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt65285> pParser;
        }
}
