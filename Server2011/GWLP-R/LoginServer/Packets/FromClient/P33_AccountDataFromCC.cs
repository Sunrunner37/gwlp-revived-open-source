using System;
using LoginServer.ServerData;
using ServerEngine.ProcessorQueues;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 33)]
        public class P33_AccountDataFromCC : IPacket
        {
                public class PacketSt33 : IPacketTemplate
                {
                        public UInt16 Header { get { return 33; } }
                        public UInt32 LoginCount;
                        public UInt32 Data2;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt33>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        message.PacketTemplate = new PacketSt33();
                        pParser((PacketSt33)message.PacketTemplate, message.PacketData);

                        Client client;
                        lock (client = World.GetClient(Idents.Clients.NetID, message.NetID))
                        {
                                client.LoginCount = (int)((PacketSt33)message.PacketTemplate).LoginCount;
                        }

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt33> pParser;
        }
}
