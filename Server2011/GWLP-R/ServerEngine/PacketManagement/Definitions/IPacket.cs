using ServerEngine.NetworkManagement;

namespace ServerEngine.PacketManagement.Definitions
{
        public interface IPacket
        {
                void InitPacket(object parser);
                bool Handler(ref NetworkMessage message);

                bool IsInitialized { get; set; }
                bool IsInUse { get; set; }
        }
}