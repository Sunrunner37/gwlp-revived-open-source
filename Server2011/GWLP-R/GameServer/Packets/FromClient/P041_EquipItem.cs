using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Actions;
using GameServer.Enums;
using GameServer.Packets.ToClient;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;
using GameServer.ServerData.Items;
using GameServer.ServerData;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 41)]
        public class P041_EquipItem : IPacket
        {
                public class PacketSt41 : IPacketTemplate
                {
                        public UInt16 Header { get { return 41; } }
                        public UInt32 ItemLocalID;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt41>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        var pack = new PacketSt41();
                        pParser(pack, message.PacketData);

                        var chara = GameServerWorld.Instance.Get<DataClient>(message.NetID).Character;

                        // item exists
                        Item itemToEquip;
                        if (!chara.Data.Items.TryGetValue((int)pack.ItemLocalID, out itemToEquip)) return true; // item did not exist

                        AgentEquipment equipmentSlot;

                        switch (itemToEquip.Data.Type)
                        {
                                case ItemType.Head:
                                        equipmentSlot = AgentEquipment.Head;
                                        break;
                                case ItemType.Chest:
                                        equipmentSlot = AgentEquipment.Chest;
                                        break;
                                case ItemType.Arms:
                                        equipmentSlot = AgentEquipment.Arms;
                                        break;
                                case ItemType.Legs:
                                        equipmentSlot = AgentEquipment.Legs;
                                        break;
                                case ItemType.Feet:
                                        equipmentSlot = AgentEquipment.Feet;
                                        break;
                                default:
                                        equipmentSlot = itemToEquip.Data.GetFlag(ItemFlagEnums.Leadhand) ? AgentEquipment.Leadhand : AgentEquipment.Offhand;
                                        break;
                        }

                        Item itemEquiped;
                        if (!chara.Data.Items.Equipment.TryGetValue(equipmentSlot, out itemEquiped))
                        {
                                itemToEquip.Data.Storage = ItemStorage.Equiped;
                                itemToEquip.Data.Slot = (int)equipmentSlot;
                                itemToEquip.SaveToDB();
                                itemToEquip.SendMove(chara.Data.NetID);
                                chara.Data.Items.Equipment.Add(equipmentSlot, itemToEquip);

                                // update weaponset
                                switch (equipmentSlot)
                                {
                                case AgentEquipment.Leadhand:
                                        chara.Data.Items.ActiveWeaponset.LeadHand = itemToEquip;
                                        chara.Data.SaveToWeaponsetsDB();
                                        break;
                                case AgentEquipment.Offhand:
                                        chara.Data.Items.ActiveWeaponset.OffHand = itemToEquip;
                                        chara.Data.SaveToWeaponsetsDB();
                                        break;
                                }
                        }
                        else // swap
                        {
                                itemEquiped.Data.Storage = itemToEquip.Data.Storage;
                                itemEquiped.Data.Slot = itemToEquip.Data.Slot;
                                itemEquiped.SaveToDB();
                                itemToEquip.Data.Storage = ItemStorage.Equiped;
                                itemToEquip.Data.Slot = (int)equipmentSlot;
                                itemToEquip.SaveToDB();

                                chara.Data.Items.Equipment[equipmentSlot] = itemToEquip;

                                var swapItems = new NetworkMessage(chara.Data.NetID)
                                {
                                        PacketTemplate = new P328_SwapItems.PacketSt328
                                        {
                                                ItemStreamID = 1,
                                                MovedItemLocalID = (uint)itemToEquip.Data.ItemLocalID,
                                                ItemToBeSwappedWithLocalID = (uint)itemEquiped.Data.ItemLocalID
                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(swapItems);

                                // update weaponset
                                switch (equipmentSlot)
                                {
                                case AgentEquipment.Leadhand:
                                        chara.Data.Items.ActiveWeaponset.LeadHand = itemToEquip;
                                        chara.Data.SaveToWeaponsetsDB();
                                        break;
                                case AgentEquipment.Offhand:
                                        chara.Data.Items.ActiveWeaponset.OffHand = itemToEquip;
                                        chara.Data.SaveToWeaponsetsDB();
                                        break;
                                }
                        }

                        var map = GameServerWorld.Instance.Get<DataMap>(chara.Data.MapID);
                        map.Data.ActionQueue.Enqueue(
                                new SendToAllClients(
                                        new P099_UpdateAgentEquipment.PacketSt99
                                        {
                                                AgentID = chara.Data.AgentID.Value,
                                                EquipmentSlot = (uint)equipmentSlot,
                                                ItemLocalID = (uint)chara.Data.Items.Equipment[equipmentSlot].Data.ItemLocalID
                                        }
                                ).Execute);

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt41> pParser;
        }
}
