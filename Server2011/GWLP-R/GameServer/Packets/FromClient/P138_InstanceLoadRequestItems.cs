using System;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 138)]
        public class P138_InstanceLoadRequestItems : IPacket
        {
                public class PacketSt138 : IPacketTemplate
                {
                        public UInt16 Header { get { return 138; } }
                        public byte Data1;
                        public byte Data2;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt138>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        var pack = new PacketSt138();
                        pParser(pack, message.PacketData);

                        // get the character
                        var chara = GameServerWorld.Instance.Get<DataClient>(message.NetID).Character;

#warning FIXME: Item stuff is not implemented!
                        const ushort itemStreamID = 1;

                        // Note: ITEM STREAM HEAD1 
                        var head1 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new P314_ItemStreamHead1.PacketSt314
                                {
                                        Data1 = itemStreamID,
                                        Data2 = 0,
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(head1);

                        // Note: ITEM STREAM HEAD2
                        var head2 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new P318_ItemStreamHead2.PacketSt318
                                {
                                        Data1 = itemStreamID,
                                        Data2 = 0,
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(head2);

                        // Note: ITEM STREAM WEAPON BAR SLOTS
                        // (would go here)

                        // Note: ITEM STREAM END
                        // (would go here)

                        // Note: ITEM STREAM Terminator
                        var terminator = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new P393_ItemStreamTerminator.PacketSt393
                                {
                                        // only works as terminator when this is 0
                                        Data1 = 0,
                                        GameMapID = (ushort)chara.Data.GameMapID.Value,
                                        Data3 = 0,
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(terminator);
                        

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt138> pParser;
        }
}
