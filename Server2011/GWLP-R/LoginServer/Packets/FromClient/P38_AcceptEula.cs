using System;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 38)]
        public class P38_AcceptEula : IPacket
        {
                public class PacketSt38 : IPacketTemplate
                {
                        public UInt16 Header { get { return 38; } }
                        public byte EulaNumber; // was given by the server
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt38>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        var pack = new PacketSt38();
                        pParser(pack, message.PacketData);

                        // do nothing here

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt38> pParser;
        }
}
