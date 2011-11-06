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
        [PacketAttributes(IsIncoming = true, Header = 100)]
        public class P100_EquipBag : IPacket
        {
                public class PacketSt100 : IPacketTemplate
                {
                        public UInt16 Header { get { return 100; } }
                        public UInt32 BagItemLocalID;
                        public byte EquipmentSlot;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt100>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        var pack = new PacketSt100();
                        pParser(pack, message.PacketData);

                        var chara = GameServerWorld.Instance.Get<DataClient>(message.NetID).Character;

                        Item bag;
                        if (!chara.Data.Items.TryGetValue((int)pack.BagItemLocalID, out bag)) return true;

                        var bagType = bag.Data.Stats.First().Value1;
                        var equipmentSlot = pack.EquipmentSlot;

                        if (equipmentSlot == 0) //direct equip
                        {
                                switch (bagType)
                                {
                                case 2:
                                        if (chara.Data.Items.Equipment.ContainsKey(AgentEquipment.Bag1))
                                        {
                                                equipmentSlot = 3;
                                        }
                                        break;
                                case 3:
                                        equipmentSlot = 4;
                                        break;
                                default:
                                        equipmentSlot = bagType;
                                        break;
                                }
                        }
                        else
                        {
                                equipmentSlot--;
                        }

                        switch ((ItemStorage)equipmentSlot)
                        {
                        case ItemStorage.Backpack:
                                if (bagType != 0)
                                {
                                        ActionTerminator(pack.BagItemLocalID, chara.Data.NetID); 
                                        return true;
                                }
                                break;
                        case ItemStorage.BeltPouch:
                                if (bagType != 1)
                                {
                                        ActionTerminator(pack.BagItemLocalID, chara.Data.NetID);
                                        return true;
                                }
                                break;
                        case ItemStorage.EquipmentPack:
                                if (bagType != 3)
                                {
                                        ActionTerminator(pack.BagItemLocalID, chara.Data.NetID);
                                        return true;
                                }
                                break;
                        default:
                                if (bagType != 2)
                                {
                                        ActionTerminator(pack.BagItemLocalID, chara.Data.NetID);
                                        return true;
                                }
                                break;
                        }

                        var equipBag = new NetworkMessage(chara.Data.NetID)
                        {
                                PacketTemplate = new P316_EquipBag.PacketSt316
                                {
                                        ItemStreamID = 1,
                                        StorageID = equipmentSlot,
                                        PageID = equipmentSlot,
                                        Slots = (byte)bag.GetBagSize(),
                                        BagLocalID = pack.BagItemLocalID
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(equipBag);

                        bag.Data.Storage = ItemStorage.Equiped;
                        bag.Data.Slot = (int)(equipmentSlot + AgentEquipment.Backpack);
                        bag.SaveToDB();

                        chara.Data.Items.Equipment.Add((AgentEquipment)bag.Data.Slot, bag);

                        ActionTerminator(pack.BagItemLocalID, chara.Data.NetID);

                        return true;
                }

                private static void ActionTerminator(uint bagItemID, NetID netID)
                {
                        var equipBag2 = new NetworkMessage(netID)
                        {
                                PacketTemplate = new P306_EquipBag2.PacketSt306
                                {
                                        StorageID = 2,
                                        BagLocalID = bagItemID
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(equipBag2);
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt100> pParser;
        }
}
