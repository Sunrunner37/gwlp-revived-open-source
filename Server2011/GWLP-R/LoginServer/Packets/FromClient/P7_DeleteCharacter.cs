using System;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 7)]
        public class P7_DeleteCharacter : IPacket
        {
                public class PacketSt7 : IPacketTemplate
                {
                        public UInt16 Header { get { return 7; } }
                        public UInt32 CharacterID;//not really sure bout this
                        [PacketFieldType(ConstSize = false, MaxSize = 20)]
                        public string CharacterName;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt7>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        throw new NotImplementedException();
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt7> pParser;
        }
}
