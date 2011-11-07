using System;
using System.Linq;
using GameServer.Actions;
using GameServer.Packets.ToClient;
using GameServer.ServerData.Items;
using ServerEngine.GuildWars.Tools;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;
using GameServer.ServerData;
using GameServer.Enums;
using ServerEngine;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 56)]
        public class P056_PickupItem : IPacket
        {
                public class PacketSt56 : IPacketTemplate
                {
                        public UInt16 Header { get { return 56; } }
                        public UInt32 AgentID; //was ID!!
                        public byte Flag;//0
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt56>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        var pack = new PacketSt56();
                        pParser(pack, message.PacketData);

                        // NOTE: ONCE PATHING IS COMPLETE WE HAVE TO CREATE AN "ON TARGET DO SOMETHING"-FEATURE
                        // NOTE: THIS IS ONLY THE REQUEST TO PICKUP AN ITEM THE CHAR THEN RUNS TO IT AN PICKS IT UP
                        // NOTE: WHEN REACHING THE ITEM. SO IT SHOULD BE IN "ON TARGET PICKUP". THE CODE SHOULD BE REUSABLE.
                        var chara = GameServerWorld.Instance.Get<DataClient>(message.NetID).Character;
                        var map = GameServerWorld.Instance.Get<DataMap>(chara.Data.MapID);

                        Item item;
                        int itemAgentID = (int) pack.AgentID;

                        // check if the item is still on the ground
                        if (!map.Data.MapItems.TryGetValue(itemAgentID, out item)) return true; // someone was first

                        // distance apropriate
                        if ((chara.Data.Position - item.Data.Position).Length > 100) return true; // item is too far away

                        // item unbounded or for the chara
                        if (item.Data.OwnerCharID.Value > 0 && item.Data.OwnerCharID.Value != chara.Data.CharID.Value) return true; // chara was not the owner

                        // gold coins
                        // TODO: Cleanup
                        if (item.Data.Type == ItemType.Coins)
                        {
                                // item not already picked up
                                if (!map.Data.MapItems.Remove(itemAgentID)) return true; // someone was first

                                // now finally we can start the pickup procedure
                                var freezeCoin = new NetworkMessage(chara.Data.NetID)
                                {
                                        PacketTemplate = new P147_UpdateGenericValueInt.PacketSt147
                                        {
                                                AgentID = chara.Data.AgentID.Value,
                                                ValueID = (uint)GenericValues.FreezePlayer,
                                                Value = 1
                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(freezeCoin);

                                map.Data.ActionQueue.Enqueue(
                                        new SendToAllClients(
                                                new P147_UpdateGenericValueInt.PacketSt147
                                                {
                                                        AgentID = chara.Data.AgentID.Value,
                                                        ValueID = 39,
                                                        Value = 9
                                                }
                                        ).Execute);

                                var confirmationCoin = new NetworkMessage(chara.Data.NetID)
                                {
                                        PacketTemplate = new P335_PickupItemConfirmation.PacketSt335
                                        {
                                                ItemLocalID = (uint)item.Data.ItemLocalID,
                                                Data2 = (ushort)chara.Data.AgentID.Value
                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(confirmationCoin);

                                var addGold = new NetworkMessage(chara.Data.NetID)
                                {
                                        PacketTemplate = new P310_AddGoldOnCharacter.PacketSt310
                                        {
                                                ItemStreamID = 1,
                                                GoldOnCharacter = (uint)item.Data.Quantity
                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(addGold);

                                // remove item from ground
                                map.Data.ActionQueue.Enqueue(
                                        new SendToAllClients(
                                                new P022_DespawnObject.PacketSt22
                                                {
                                                        AgentID = (uint)item.Data.ItemAgentID,
                                                }
                                        ).Execute);

                                freezeCoin = new NetworkMessage(chara.Data.NetID)
                                {
                                        PacketTemplate = new P147_UpdateGenericValueInt.PacketSt147
                                        {
                                                AgentID = chara.Data.AgentID.Value,
                                                ValueID = (uint)GenericValues.FreezePlayer,
                                                Value = 0
                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(freezeCoin);
                                return true;
                        }

                        // slot free
                        ItemStorage storage;
                        byte slot;
                        if (!chara.Data.Items.GetFirstFreeSlot(out storage, out slot))
                        {
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

                                return true; 
                        }

                        // item not already picked up
                        if (!map.Data.MapItems.Remove(itemAgentID)) return true; // someone was first

                        // now finally we can start the pickup procedure
                        var freeze = new NetworkMessage(chara.Data.NetID)
                        {
                                PacketTemplate = new P147_UpdateGenericValueInt.PacketSt147
                                {
                                        AgentID = chara.Data.AgentID.Value,
                                        ValueID = (uint)GenericValues.FreezePlayer,
                                        Value = 1
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(freeze);

                        map.Data.ActionQueue.Enqueue(
                                new SendToAllClients(
                                        new P147_UpdateGenericValueInt.PacketSt147
                                        {
                                                AgentID = chara.Data.AgentID.Value,
                                                ValueID = 39,
                                                Value = 9
                                        }
                                ).Execute);

                        // Add item to charas inventory
                        item.Data.OwnerAccID.Value = chara.Data.AccID.Value;
                        item.Data.OwnerCharID.Value = chara.Data.CharID.Value;
                        item.Data.Storage = storage;
                        item.Data.Slot = slot;
                        chara.Data.Items.AddSave(item.Data.ItemLocalID, item);
                        item.SendLocation(chara.Data.NetID);

                        var confirmation = new NetworkMessage(chara.Data.NetID)
                        {
                                PacketTemplate = new P335_PickupItemConfirmation.PacketSt335
                                {
                                        ItemLocalID = (uint)item.Data.ItemLocalID,
                                        Data2 = (ushort)chara.Data.AgentID.Value
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(confirmation);

                        // remove item from ground
                        map.Data.ActionQueue.Enqueue(
                                new SendToAllClients(
                                        new P022_DespawnObject.PacketSt22
                                        {
                                                AgentID = (uint)item.Data.ItemAgentID,
                                        }
                                ).Execute);

                        freeze = new NetworkMessage(chara.Data.NetID)
                        {
                                PacketTemplate = new P147_UpdateGenericValueInt.PacketSt147
                                {
                                        AgentID = chara.Data.AgentID.Value,
                                        ValueID = (uint)GenericValues.FreezePlayer,
                                        Value = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(freeze);

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt56> pParser;
        }
}
