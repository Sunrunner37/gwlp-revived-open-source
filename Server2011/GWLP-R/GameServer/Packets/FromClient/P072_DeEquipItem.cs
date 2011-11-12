using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Actions;
using GameServer.Enums;
using GameServer.Packets.ToClient;
using GameServer.ServerData.Items;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;
using GameServer.ServerData;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 72)]
        public class P072_DeEquipItem : IPacket
        {
                public class PacketSt72 : IPacketTemplate
                {
                        public UInt16 Header { get { return 72; } }
                        public byte EquipmentSlot;
                        public UInt16 NewPage;
                        public byte NewSlot;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt72>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        var pack = new PacketSt72();
                        pParser(pack, message.PacketData);

                        var chara = GameServerWorld.Instance.Get<DataClient>(message.NetID).Character;
                        
                        Item item;
                        if (!chara.Data.Items.Equipment.TryGetValue((AgentEquipment)pack.EquipmentSlot, out item)) return true;

                        byte slot = pack.NewSlot;

                        if (slot == 0xFF) // item deequiped to bag
                        {
                                if (!chara.Data.Items.GetFirstFreeSlotInBag((ItemStorage)pack.NewPage, out slot))
                                {
                                        var equipBag2 = new NetworkMessage(chara.Data.NetID)
                                        {
                                                PacketTemplate = new P306_EquipBag2.PacketSt306
                                                {
                                                        StorageID = 11,
                                                        BagLocalID = 0
                                                }
                                        };
                                        QueuingService.PostProcessingQueue.Enqueue(equipBag2);

                                        return true;
                                }
                        }
                        
                        if (!chara.Data.Items.Equipment.Remove((AgentEquipment)pack.EquipmentSlot)) return true;

                        item.Data.Storage = (ItemStorage)pack.NewPage;
                        item.Data.Slot = slot;
                        item.SaveToDB();
                        item.SendMove(chara.Data.NetID);
                        
                        // update weaponsets
                        switch (pack.EquipmentSlot)
                        {
                        case 0:
                                chara.Data.Items.ActiveWeaponset.LeadHand = new Item();
                                chara.Data.SaveToWeaponsetsDB();
                                break;
                        case 1:
                                chara.Data.Items.ActiveWeaponset.OffHand = new Item();
                                chara.Data.SaveToWeaponsetsDB();
                                break;
                        }

                        var map = GameServerWorld.Instance.Get<DataMap>(chara.Data.MapID);
                        map.Data.ActionQueue.Enqueue(
                                new SendToAllClients(
                                        new P099_UpdateAgentEquipment.PacketSt99
                                        {
                                                AgentID = chara.Data.AgentID.Value,
                                                EquipmentSlot = pack.EquipmentSlot,
                                                ItemLocalID = 0
                                        }
                                ).Execute);

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt72> pParser;
        }
}
