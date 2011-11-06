using System;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 109)]
        public class P109_MoveBag : IPacket
        {
                public class PacketSt109 : IPacketTemplate
                {
                        public UInt16 Header { get { return 109; } }
                        public byte SourceBag;
                        public byte DestBag;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt109>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        var pack = new PacketSt109();
                        pParser(pack, message.PacketData);

                        var chara = GameServerWorld.Instance.Get<DataClient>(message.NetID).Character;

                        if (pack.SourceBag == 2 && pack.DestBag == 3)
                        {
                                var moveBag = new NetworkMessage(chara.Data.NetID)
                                {
                                        PacketTemplate = new P322_MoveBag.PacketSt322
                                        {
                                                ItemStreamID = 1,
                                                FromStorageID = pack.SourceBag,
                                                ToStorageID = pack.DestBag
                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(moveBag);
                        }
                        else if (pack.SourceBag == 3 && pack.DestBag == 2)
                        {
                                var moveBag = new NetworkMessage(chara.Data.NetID)
                                {
                                        PacketTemplate = new P322_MoveBag.PacketSt322
                                        {
                                                ItemStreamID = 1,
                                                FromStorageID = pack.SourceBag,
                                                ToStorageID = pack.DestBag
                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(moveBag);
                        }
                        else
                        {
                                var equipBag2 = new NetworkMessage(chara.Data.NetID)
                                {
                                        PacketTemplate = new P306_EquipBag2.PacketSt306
                                        {
                                                StorageID = 8,
                                                BagLocalID = pack.SourceBag
                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(equipBag2);
                        }

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt109> pParser;
        }
}
