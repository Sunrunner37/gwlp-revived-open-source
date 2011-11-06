using System;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;
using GameServer.ServerData;
using GameServer.Packets.ToClient;
using ServerEngine;
using GameServer.ServerData.Items;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 98)]
        public class P098_DestroyItem : IPacket
        {
                public class PacketSt98 : IPacketTemplate
                {
                        public UInt16 Header { get { return 98; } }
                        public UInt32 ÏtemLocalID;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt98>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        var pack = new PacketSt98();
                        pParser(pack, message.PacketData);

                        var chara = GameServerWorld.Instance.Get<DataClient>(message.NetID).Character;

                        Item item;
                        if (!chara.Data.Items.TryGetValue((int)pack.ÏtemLocalID, out item)) return true;

                        item.DeleteFromDB();

                        var removeItem = new NetworkMessage(chara.Data.NetID)
                        {
                                PacketTemplate = new P323_RemoveItemFromInventory.PacketSt323
                                {
                                        ItemStreamID = 1,
                                        ItemLocalID = (uint)item.Data.ItemLocalID
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(removeItem);

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt98> pParser;
        }
}
