using System;
using System.Linq;
using System.Collections.Generic;
using GameServer.Enums;
using GameServer.Packets.ToClient;
using ServerEngine;
using ServerEngine.DataManagement.DataWrappers;
using ServerEngine.NetworkManagement;

namespace GameServer.ServerData.Items
{
        public class Weaponset
        {
                /// <summary>
                ///   This property determines the number off 
                /// </summary>
                public int Number { get; set; }

                /// <summary>
                ///   The lead hand item, be sure to syncronize that with the chara's items!
                /// </summary>
                public Item LeadHand { get; set; }

                /// <summary>
                ///   The off hand item, be sure to syncronize that with the chara's items!
                /// </summary>
                public Item OffHand { get; set; }

                public void SendPackets(NetID netID, int itemStreamID)
                {
                        var weaponbarSlot = new NetworkMessage(netID)
                        {
                                PacketTemplate = new P317_ItemStreamWeaponBarSlot.PacketSt317
                                {
                                        ItemStreamID = (ushort)itemStreamID,
                                        LeadhandItemLocalID = (uint)LeadHand.Data.ItemLocalID,
                                        OffhandItemLocalID = (uint)OffHand.Data.ItemLocalID,
                                        SlotNumber = (byte)Number
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(weaponbarSlot);
                }
        }
}