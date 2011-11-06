using System;
using System.Linq;
using GameServer.Actions;
using GameServer.Enums;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using GameServer.ServerData.Items;
using ServerEngine;
using ServerEngine.GuildWars.Tools;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 43)]
        public class P043_ChangeWeaponSet : IPacket
        {
                public class PacketSt43 : IPacketTemplate
                {
                        public UInt16 Header { get { return 43; } }
                        public byte WeaponSet;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt43>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }
                        
                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        var pack = new PacketSt43();
                        pParser(pack, message.PacketData);

                        var chara = GameServerWorld.Instance.Get<DataClient>(message.NetID).Character;

                        Weaponset newWeaponset = chara.Data.Items.Weaponsets[pack.WeaponSet];
                        byte numNewWeapons = GetWeaponsInfo(newWeaponset);

                        Weaponset activeWeaponset = chara.Data.Items.ActiveWeaponset;
                        byte numActiveWeapons = GetWeaponsInfo(activeWeaponset);

                        int slotsRequired = numActiveWeapons - numNewWeapons;

                        bool hasEnoughSpace = (chara.Data.Items.GetNumFreeSlots() >= slotsRequired);

                        if (hasEnoughSpace)
                        {
                                // update the weaponset selection
                                chara.Data.Items.ActiveWeaponset = newWeaponset;

                                var updateActiveWeaponset = new NetworkMessage(chara.Data.NetID)
                                {
                                        PacketTemplate = new P318_UpdateActiveWeaponset.PacketSt318
                                        {
                                                ItemStreamID = 1,
                                                ActiveWeaponSlot = (byte)chara.Data.Items.ActiveWeaponset.Number
                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(updateActiveWeaponset);

                                SwitchEquipmentSlot(chara, activeWeaponset.LeadHand, newWeaponset.LeadHand, 0);
                                SwitchEquipmentSlot(chara, activeWeaponset.OffHand, newWeaponset.OffHand, 1);

                                chara.Data.SaveToDB();

                                var map = GameServerWorld.Instance.Get<DataMap>(chara.Data.MapID);
                                map.Data.ActionQueue.Enqueue(
                                        new SendToAllClients(
                                                new P099_UpdateAgentEquipment.PacketSt99
                                                {
                                                        AgentID = chara.Data.AgentID.Value,
                                                        EquipmentSlot = 0,
                                                        ItemLocalID = (uint)newWeaponset.LeadHand.Data.ItemLocalID
                                                }
                                        ).Execute);

                                map.Data.ActionQueue.Enqueue(
                                        new SendToAllClients(
                                                new P099_UpdateAgentEquipment.PacketSt99
                                                {
                                                        AgentID = chara.Data.AgentID.Value,
                                                        EquipmentSlot = 1,
                                                        ItemLocalID = (uint)newWeaponset.OffHand.Data.ItemLocalID
                                                }
                                        ).Execute);
                        }
                        else
                        {
                                var equipBag2 = new NetworkMessage(chara.Data.NetID)
                                {
                                        PacketTemplate = new P306_EquipBag2.PacketSt306
                                        {
                                                StorageID = 3,
                                                BagLocalID = (uint)chara.Data.Items.ActiveWeaponset.Number
                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(equipBag2);

                                // no inventory space
                                var chatMessage = new NetworkMessage(chara.Data.NetID)
                                {
                                        PacketTemplate = new P081_GeneralChatMessage.PacketSt81
                                        {
                                                Message = "Your inventory is full.".ToGW()
                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(chatMessage);

                                var chatNoOwner = new NetworkMessage(chara.Data.NetID)
                                {
                                        PacketTemplate = new P082_GeneralChatNoOwner.PacketSt82
                                        {
                                                Data1 = 1,
                                                Data2 = 7
                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(chatNoOwner);
                        }

                        return true;
                }

                private static byte GetWeaponsInfo(Weaponset weaponset)
                {
                        byte numWeapons = 0;

                        if (weaponset.LeadHand.Data.PersonalItemID > 0) numWeapons++;
                        if (weaponset.OffHand.Data.PersonalItemID > 0) numWeapons++;

                        return numWeapons;
                }

                private static void SwitchEquipmentSlot(DataCharacter chara, Item activeWeapon, Item newWeapon, byte equipmentSlot)
                {
                        if (activeWeapon.Data.PersonalItemID == newWeapon.Data.PersonalItemID) return; // it's not the same weapon

                        bool hasActiveWeapon = activeWeapon.Data.PersonalItemID > 0;
                        bool hasNewWeapon = newWeapon.Data.PersonalItemID > 0;

                        if (hasNewWeapon)
                        {
                                if (hasActiveWeapon)
                                {
                                        // new and active so swap them
                                        activeWeapon.Data.Storage = newWeapon.Data.Storage;
                                        activeWeapon.Data.Slot = newWeapon.Data.Slot;
                                        activeWeapon.SaveToDB();
                                        newWeapon.Data.Storage = ItemStorage.Equiped;
                                        newWeapon.Data.Slot = equipmentSlot;
                                        newWeapon.SaveToDB();

                                        chara.Data.Items.Equipment[(AgentEquipment)equipmentSlot] = newWeapon;

                                        var swapItems = new NetworkMessage(chara.Data.NetID)
                                        {
                                                PacketTemplate = new P328_SwapItems.PacketSt328
                                                {
                                                        ItemStreamID = 1,
                                                        MovedItemLocalID = (uint)newWeapon.Data.ItemLocalID,
                                                        ItemToBeSwappedWithLocalID = (uint)activeWeapon.Data.ItemLocalID
                                                }
                                        };
                                        QueuingService.PostProcessingQueue.Enqueue(swapItems);
                                }
                                else
                                {
                                        // new but no active
                                        newWeapon.Data.Storage = ItemStorage.Equiped;
                                        newWeapon.Data.Slot = equipmentSlot;
                                        newWeapon.SaveToDB();
                                        newWeapon.SendMove(chara.Data.NetID);
                                        chara.Data.Items.Equipment.Add((AgentEquipment)equipmentSlot, newWeapon);
                                }
                        }
                        else
                        {
                                if (hasActiveWeapon)
                                {
                                        // no new but active
                                        ItemStorage storage;
                                        byte slot;
                                        if (chara.Data.Items.GetFirstFreeSlot(out storage, out slot))
                                        {
                                                activeWeapon.Data.Storage = storage;
                                                activeWeapon.Data.Slot = slot;
                                                activeWeapon.SaveToDB();
                                                activeWeapon.SendMove(chara.Data.NetID);
                                                chara.Data.Items.Equipment.Remove((AgentEquipment)equipmentSlot);
                                        }
                                }
                        }
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt43> pParser;
        }
}
