using System;
using ServerEngine.ProcessorQueues;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 139)]
        public class P139_UnknownAfterIL : IPacket
        {
                public class PacketSt139 : IPacketTemplate
                {
                        public UInt16 Header { get { return 139; } }
                        public UInt16 ArraySize1;
                        [PacketFieldType(ConstSize = false, MaxSize = 108)]
                        public byte[] Data1;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt139>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        message.PacketTemplate = new PacketSt139();
                        pParser((PacketSt139)message.PacketTemplate, message.PacketData);

                        //throw new NotImplementedException();

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt139> pParser;
        }
}
