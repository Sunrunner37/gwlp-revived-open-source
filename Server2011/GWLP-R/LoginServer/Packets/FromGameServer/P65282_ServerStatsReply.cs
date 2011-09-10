using System;
using LoginServer.ServerData;
using ServerEngine.ProcessorQueues;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.FromGameServer
{
        [PacketAttributes(IsIncoming = true, Header = 65282)]
        public class P65282_ServerStatsReply : IPacket
        {
                public class PacketSt65282 : IPacketTemplate
                {
                        public UInt16 Header { get { return 65282; } }
                        public byte Utilization;
                        public UInt16 ArraySize1;
                        [PacketFieldType(ConstSize = false, MaxSize = 1024)]
                        public UInt16[] MapIDs;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt65282>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        message.PacketTemplate = new PacketSt65282();
                        pParser((PacketSt65282)message.PacketTemplate, message.PacketData);

                        GameServer gameServer;
                        lock (gameServer = World.GetGameServer(Idents.GameServers.NetID, message.NetID))
                        {
                                gameServer.Utilization = ((PacketSt65282) message.PacketTemplate).Utilization;
                                gameServer.AvailableMaps = ((PacketSt65282) message.PacketTemplate).MapIDs;
                        }

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt65282> pParser;
        }
}
