using System;
using GameServer.Enums;
using GameServer.Packets.ToLoginServer;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromLoginServer
{
        [PacketAttributes(IsIncoming = true, Header = 65284)]
        public class P65284_AcceptPlayerRequest : IPacket
        {
                public class PacketSt65284 : IPacketTemplate
                {
                        public UInt16 Header { get { return 65284; } }
                        [PacketFieldType(ConstSize = true, MaxSize = 4)]
                        public byte[] Key1;
                        [PacketFieldType(ConstSize = true, MaxSize = 4)]
                        public byte[] Key2;
                        public UInt32 AccID;
                        public UInt32 CharID;
                        public UInt32 MapID;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt65284>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        message.PacketTemplate = new PacketSt65284();
                        pParser((PacketSt65284)message.PacketTemplate, message.PacketData);

                        var newClient = new Client(0, (int)((PacketSt65284)message.PacketTemplate).AccID, (int)((PacketSt65284)message.PacketTemplate).CharID);
                        newClient.MapID = (ushort)((PacketSt65284) message.PacketTemplate).MapID;
                        newClient.SecurityKeys[0] = ((PacketSt65284)message.PacketTemplate).Key1;
                        newClient.SecurityKeys[1] = ((PacketSt65284)message.PacketTemplate).Key2;
                        newClient.Status = SyncState.Unauthorized;

                        World.AddClient(newClient);


                        // response
                        var reply = new NetworkMessage(message.NetID) { PacketTemplate = new P65284_AcceptPlayerReply.PacketSt65284() };
                        ((P65284_AcceptPlayerReply.PacketSt65284)reply.PacketTemplate).AccID = (UInt32)((int)newClient[Clients.AccID]);
                        QueuingService.PostProcessingQueue.Enqueue(reply);
                        

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt65284> pParser;
        }
}
