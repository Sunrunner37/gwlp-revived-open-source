using System.IO;

namespace ServerEngine.PacketManagement.Definitions
{
        public delegate void PacketParser<T>(T pStruct, MemoryStream pRaw) where T : IPacketTemplate;
}
