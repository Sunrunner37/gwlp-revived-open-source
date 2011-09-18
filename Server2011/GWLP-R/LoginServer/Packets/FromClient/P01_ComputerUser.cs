using System;
using LoginServer.Packets.ToClient;
using LoginServer.ServerData;
using ServerEngine.ProcessorQueues;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 1)]
        public class P01_ComputerUser : IPacket
        {
                public class PacketSt1 : IPacketTemplate
                {
                        public UInt16 Header { get { return 1; } }
                        [PacketFieldType(ConstSize = false, MaxSize = 32)]
                        public string Data1;
                        [PacketFieldType(ConstSize = false, MaxSize = 32)]
                        public string Data2;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt1>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        message.PacketTemplate = new PacketSt1();
                        pParser((PacketSt1)message.PacketTemplate, message.PacketData);

                        // check the sync state of the client
                        var client = World.GetClient(Idents.Clients.NetID, message.NetID);

                        var msg = new NetworkMessage(message.NetID)
                                                {
                                                        PacketTemplate = new P01_ComputerInfoReply.PacketSt1()
                                                };
                        // set the message data
                        ((P01_ComputerInfoReply.PacketSt1)msg.PacketTemplate).StaticData1 = 3732952299;
                        ((P01_ComputerInfoReply.PacketSt1)msg.PacketTemplate).LoginCount = (uint)client.LoginCount;
                        ((P01_ComputerInfoReply.PacketSt1)msg.PacketTemplate).Data3 = 0;
                        // send it
                        QueuingService.PostProcessingQueue.Enqueue(msg);
                        

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt1> pParser;
        }
}
