using System;
using GameServer.Enums;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 3)]
        public class P003_PingReply : IPacket
        {
                public class PacketSt3 : IPacketTemplate
                {
                        public UInt16 Header { get { return 3; } }
                        public UInt32 Data1;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt3>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        var pack = new PacketSt3();
                        pParser(pack, message.PacketData);

                        var client = GameServerWorld.Instance.Get<DataClient>(message.NetID);

                        // Note: PING REPLY
                        var chatMsg = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new P002_PingReply.PacketSt2
                                {
                                        Data1 = (uint)DateTime.Now.Subtract(client.Data.PingTime).TotalMilliseconds
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(chatMsg);
                        

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt3> pParser;
        }
}
