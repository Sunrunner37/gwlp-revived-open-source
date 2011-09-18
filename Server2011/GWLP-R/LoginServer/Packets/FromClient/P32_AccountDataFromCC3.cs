using System;
using System.IO;
using LoginServer.Packets.ToClient;
using LoginServer.ServerData;
using ServerEngine.ProcessorQueues;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 32)]
        public class P32_AccountDataFromCC3 : IPacket
        {
                public class PacketSt32 : IPacketTemplate
                {
                        public UInt16 Header { get { return 32; } }
                        public UInt32 LoginCount;
                        public UInt16 ArraySize1;
                        [PacketFieldType(ConstSize = false, MaxSize = 512)]
                        public byte[] Data2;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt32>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        message.PacketTemplate = new PacketSt32();
                        pParser((PacketSt32)message.PacketTemplate, message.PacketData);

                        var client = World.GetClient(Idents.Clients.NetID, message.NetID);
                        
                        client.LoginCount = (int)((PacketSt32)message.PacketTemplate).LoginCount;

                        // send a stream terminator:
                        var msg = new NetworkMessage((int)client[Idents.Clients.NetID])
                        {
                                PacketTemplate = new P03_StreamTerminator.PacketSt3()
                        };
                        // set the message data
                        ((P03_StreamTerminator.PacketSt3)msg.PacketTemplate).LoginCount = (uint)client.LoginCount;
                        ((P03_StreamTerminator.PacketSt3)msg.PacketTemplate).ErrorCode = 0;
                        // send it
                        QueuingService.PostProcessingQueue.Enqueue(msg);
                        
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt32> pParser;
        }
}
