using System;
using LoginServer.Packets.ToClient;
using LoginServer.ServerData;
using ServerEngine.ProcessorQueues;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace LoginServer.Packets.FromGameServer
{
        [PacketAttributes(IsIncoming = true, Header = 65285)]
        public class P65285_ClientDispatchAcknowledgement : IPacket
        {
                public class PacketSt65285 : IPacketTemplate
                {
                        public UInt16 Header { get { return 65285; } }
                        public UInt32 AccID;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt65285>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        message.PacketTemplate = new PacketSt65285();
                        pParser((PacketSt65285)message.PacketTemplate, message.PacketData);

                        // get client and send stream terminator
                        var client = World.GetClient(Idents.Clients.AccID, ((PacketSt65285) message.PacketTemplate).AccID);
                        
#warning Dispatch should require the login server to search for a game server!
                        //client.LoginCount++;

                        //// send a stream terminator:
                        //var msg = new NetworkMessage((int)client[Idents.Clients.NetID])
                        //{
                        //        PacketTemplate = new P03_StreamTerminator.PacketSt3()
                        //};
                        //// set the message data
                        //((P03_StreamTerminator.PacketSt3)msg.PacketTemplate).LoginCount = (uint)client.LoginCount;
                        //((P03_StreamTerminator.PacketSt3)msg.PacketTemplate).ErrorCode = 0;
                        //// send it
                        //QueuingService.PostProcessingQueue.Enqueue(msg);

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt65285> pParser;
        }
}
