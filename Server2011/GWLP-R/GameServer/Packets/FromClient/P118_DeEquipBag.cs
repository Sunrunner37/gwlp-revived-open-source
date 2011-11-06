using System;
using System.Linq;
using GameServer.Enums;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using GameServer.ServerData.Items;
using ServerEngine;
using ServerEngine.DataManagement.DataWrappers;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 118)]
        public class P118_DeEquipBag : IPacket
        {
                public class PacketSt118 : IPacketTemplate
                {
                        public UInt16 Header { get { return 118; } }
                        public UInt32 BagLocalID;
                        public UInt16 PageID;
                        public byte Slot; //0 = into bag directly, 1 based slotcount
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt118>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        var pack = new PacketSt118();
                        pParser(pack, message.PacketData);

                        var chara = GameServerWorld.Instance.Get<DataClient>(message.NetID).Character;

                        Item bag;
                        if (!chara.Data.Items.TryGetValue((int)pack.BagLocalID, out bag)) return true;

                        var storage = (ItemStorage) pack.PageID;

                        if ((bag.Data.Slot - (int)AgentEquipment.Backpack) == (int)storage)
                        {
                                ActionTerminator(pack.BagLocalID, chara.Data.NetID);
                                return true;
                        }

                        byte slot = pack.Slot;

                        if (slot == 0) // bag directly deequiped
                        {
                                if (!chara.Data.Items.GetFirstFreeSlotInBag(storage, out slot))
                                {
                                        ActionTerminator(pack.BagLocalID, chara.Data.NetID);
                                        return true;
                                }
                        }
                        else
                        {
                                slot--;
                        }

                        if (chara.Data.Items.Get(storage).Values.Where(item => item.Data.Slot == slot).Count() != 0)
                        {
                                ActionTerminator(pack.BagLocalID, chara.Data.NetID);
                                return true;
                        }

                        var moveEquipedBagIntoOtherStorage = new NetworkMessage(chara.Data.NetID)
                        {
                                PacketTemplate = new P329_MoveEquipedBagIntoOtherStorage.PacketSt329
                                {
                                        ItemStreamID = 1,
                                        BagLocalID = pack.BagLocalID,
                                        NewPageID = pack.PageID,
                                        NewSlot = slot
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(moveEquipedBagIntoOtherStorage);

                        chara.Data.Items.Equipment.Remove((AgentEquipment)bag.Data.Slot);

                        bag.Data.Storage = storage;
                        bag.Data.Slot = slot;
                        bag.SaveToDB();

                        return true;
                }

                private static void ActionTerminator(uint bagItemLocalID, NetID netID)
                {
                        var equipBag2 = new NetworkMessage(netID)
                        {
                                PacketTemplate = new P306_EquipBag2.PacketSt306
                                {
                                        StorageID = 11,
                                        BagLocalID = bagItemLocalID
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(equipBag2);
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt118> pParser;
        }
}
