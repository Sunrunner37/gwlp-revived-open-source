using System;
using System.Diagnostics;
using LoginServer.Enums;
using LoginServer.ServerData;
using ServerEngine.ProcessorQueues;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 1024)]
        public class NotEncP1024_ClientVersion : IPacket
        {
                public class PacketSt1024 : IPacketTemplate
                {
                        public UInt16 Header { get { return 1024; } }
                        public UInt16 Data1;
                        public UInt32 ClientVersion;
                        public UInt32 Data3;
                        public UInt32 Data4;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt1024>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        message.PacketTemplate = new PacketSt1024();
                        pParser((PacketSt1024)message.PacketTemplate, message.PacketData);

                        // add a new client here
                        var newClient = new Client(message.NetID) {Status = SyncState.ConnectionEstablished};
                        World.AddClient(newClient);

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt1024> pParser;
        }
}
