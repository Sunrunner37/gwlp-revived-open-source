using System;
using System.Linq;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using GameServer.ServerData.Items;
using ServerEngine;
using ServerEngine.GuildWars.DataWrappers.Chars;
using ServerEngine.GuildWars.Tools;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 40)]
        public class P040_DropGold : IPacket
        {
                public class PacketSt40 : IPacketTemplate
                {
                        public UInt16 Header { get { return 40; } }
                        public UInt32 Amount;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt40>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        var pack = new PacketSt40();
                        pParser(pack, message.PacketData);

                        var chara = GameServerWorld.Instance.Get<DataClient>(message.NetID).Character;
                        var map = GameServerWorld.Instance.Get<DataMap>(chara.Data.MapID);

                        var removeGold = new NetworkMessage(chara.Data.NetID)
                        {
                                PacketTemplate = new P325_RemoveGoldFromCharacter.PacketSt325
                                {
                                        ItemStreamID = 1,
                                        Amount = pack.Amount
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(removeGold);

                        Item coins = pack.Amount == 1 ? Item.CreateItemStubFromDB(10, map.Data.ItemLocalIDs.RequestID()) : Item.CreateItemStubFromDB(11, map.Data.ItemLocalIDs.RequestID());

                        coins.Data.Flags = 537395201;
                        coins.Data.MerchantPrice = pack.Amount;
                        coins.Data.Quantity = (int) pack.Amount;
                        coins.Data.ItemAgentID = map.Data.AgentIDs.RequestID();
                        coins.Data.Position = chara.Data.Position.Clone();
                        map.Data.MapItems.Add(coins.Data.ItemAgentID, coins);

                        foreach (var charID in map.GetAll<DataCharacter>().Select(x => x.Data.CharID))
                        {
                                var reNetID = GameServerWorld.Instance.Get<DataClient>(charID).Data.NetID;
                                coins.SendGeneral(reNetID);
                                coins.SendSpawn(reNetID);
                        }
                        
                        /*var itemGeneral2 = new NetworkMessage(chara.Data.NetID)
                        {
                                PacketTemplate = new P344_ItemGeneral2.PacketSt344
                                {
                                        LocalID = (uint) map.Data.ItemLocalIDs.RequestID(),
                                        FileID = 97552,
                                        ItemType = 20,
                                        Flags = 537395201,
                                        MerchantPrice = pack.Amount,
                                        ItemID = 2511,
                                        Quantity = pack.Amount,
                                        NameHash = pack.Amount > 0 ? "Gold Coin".ToGW() : "Gold Coins".ToGW()
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(itemGeneral2);*/

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt40> pParser;
        }
}
