using System;
using System.Linq;
using GameServer.Actions;
using GameServer.Enums;
using GameServer.Packets.ToClient;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;
using GameServer.ServerData;
using GameServer.ServerData.Items;
using ServerEngine;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 37)]
        public class P037_DropItem : IPacket
        {
                public class PacketSt37 : IPacketTemplate
                {
                        public UInt16 Header { get { return 37; } }
                        public UInt32 ItemLocalID;
                        public byte Amount;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt37>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        var pack = new PacketSt37();
                        pParser(pack, message.PacketData);

                        var chara = GameServerWorld.Instance.Get<DataClient>(message.NetID).Character;
                        var map = GameServerWorld.Instance.Get<DataMap>(chara.Data.MapID);

                        Item item;
                        if (!chara.Data.Items.TryGetValue((int)pack.ItemLocalID, out item)) return true;

                        Item itemToDrop;

                        if (pack.Amount < item.Data.Quantity)
                        {
                                item.Data.Quantity = item.Data.Quantity - pack.Amount;
                                item.SaveToDB();

                                var updateQuantity = new NetworkMessage(chara.Data.NetID)
                                {
                                        PacketTemplate = new P303_UpdateItemQuantity.PacketSt303
                                        {
                                                ItemLocalID = (uint)item.Data.ItemLocalID,
                                                NewQuantity = (uint)item.Data.Quantity
                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(updateQuantity);

                                itemToDrop = item.Clone();
                                itemToDrop.Data.ItemLocalID = map.Data.ItemLocalIDs.RequestID();
                                itemToDrop.Data.Quantity = pack.Amount;
                                itemToDrop.Data.OwnerAccID.Value = 0;

                                foreach (var charID in map.GetAll<DataCharacter>().Select(x => x.Data.CharID))
                                {
                                        var reNetID = GameServerWorld.Instance.Get<DataClient>(charID).Data.NetID;
                                        itemToDrop.SendGeneral(reNetID);
                                }
                        }
                        else
                        {
                                if (!chara.Data.Items.Remove(item.Data.ItemLocalID)) return true;

                                if (item.Data.Storage != ItemStorage.Equiped)
                                {
                                        var removeItem = new NetworkMessage(chara.Data.NetID)
                                        {
                                                PacketTemplate = new P323_RemoveItemFromInventory.PacketSt323
                                                {
                                                        ItemStreamID = 1,
                                                        ItemLocalID = (uint)item.Data.ItemLocalID
                                                }
                                        };
                                        QueuingService.PostProcessingQueue.Enqueue(removeItem);
                                }
                                else
                                {
                                        chara.Data.Items.Equipment.Remove((AgentEquipment) item.Data.Slot);

                                        if (item.Data.Type == ItemType.Bag)
                                        {
                                                var removeBag = new NetworkMessage(chara.Data.NetID)
                                                {
                                                        PacketTemplate = new P324_DropEquipedBag.PacketSt324
                                                        {
                                                                ItemStreamID = 1,
                                                                PageID = (ushort) (item.Data.Slot - (int)AgentEquipment.Backpack)
                                                        }
                                                };
                                                QueuingService.PostProcessingQueue.Enqueue(removeBag);
                                        }
                                        else
                                        {
                                                switch (item.Data.Slot)
                                                {
                                                        case 0:
                                                                chara.Data.Items.ActiveWeaponset.LeadHand = new Item();
                                                                break;
                                                        case 1:
                                                                chara.Data.Items.ActiveWeaponset.OffHand = new Item();
                                                                break;
                                                }

                                                var removeItem = new NetworkMessage(chara.Data.NetID)
                                                {
                                                        PacketTemplate = new P323_RemoveItemFromInventory.PacketSt323
                                                        {
                                                                ItemStreamID = 1,
                                                                ItemLocalID = (uint)item.Data.ItemLocalID
                                                        }
                                                };
                                                QueuingService.PostProcessingQueue.Enqueue(removeItem);

                                                map.Data.ActionQueue.Enqueue(
                                                        new SendToAllClients(
                                                                new P099_UpdateAgentEquipment.PacketSt99
                                                                {
                                                                        AgentID = chara.Data.AgentID.Value,
                                                                        EquipmentSlot = (uint) item.Data.Slot,
                                                                        ItemLocalID = 0
                                                                }
                                                        ).Execute);
                                        }
                                }

                                itemToDrop = item;
                                itemToDrop.Data.OwnerAccID.Value = 0;
                                itemToDrop.DeleteFromDB();
                        }

                        itemToDrop.Data.ItemAgentID = map.Data.AgentIDs.RequestID();
                        itemToDrop.Data.Position = chara.Data.Position.Clone();
                        map.Data.MapItems.Add(itemToDrop.Data.ItemAgentID, itemToDrop);

                        foreach (var charID in map.GetAll<DataCharacter>().Select(x => x.Data.CharID))
                        {
                                var reNetID = GameServerWorld.Instance.Get<DataClient>(charID).Data.NetID;
                                itemToDrop.SendSpawn(reNetID);
                        }
                        
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt37> pParser;
        }
}
