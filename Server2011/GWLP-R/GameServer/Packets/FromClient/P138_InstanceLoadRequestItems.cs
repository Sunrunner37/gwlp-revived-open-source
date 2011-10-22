using System;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;
using GameServer.ServerData.Items;
using System.Collections.Generic;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 138)]
        public class P138_InstanceLoadRequestItems : IPacket
        {
                public class PacketSt138 : IPacketTemplate
                {
                        public UInt16 Header { get { return 138; } }
                        public byte Data1;
                        public byte Data2;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt138>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        var pack = new PacketSt138();
                        pParser(pack, message.PacketData);

                        // get the character
                        var chara = GameServerWorld.Instance.Get<DataClient>(message.NetID).Character;
                        const ushort itemStreamID = 1;

                        // Note: ITEM STREAM HEAD1 
                        var head1 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new P314_ItemStreamHead.PacketSt314
                                {
                                        ItemStreamID = itemStreamID,
                                        Data2 = 0,
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(head1);

                        var activeWeaponset = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new P318_UpdateActiveWeaponset.PacketSt318
                                {
                                        ItemStreamID = itemStreamID,
                                        ActiveWeaponSlot = 0,
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(activeWeaponset);

                        // itempages for equipment and storage
                        var equipedPage = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new P309_ItemPagePacket.PacketSt309
                                {
                                        ItemStreamID = itemStreamID,
                                        StorageType = 2, // equiped
                                        StorageID = (byte)Enums.ItemStorage.Equiped,
                                        PageID = (ushort)Enums.ItemStorage.Equiped,
                                        Slots = 9,
                                        ItemLocalID = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(equipedPage);

                        // send characters items
                        foreach (KeyValuePair<int, Item> charItem in chara.Data.Items)
                        {
                                charItem.Value.SendPackets(message.NetID);
                        }

                        //weaponbar slots
                        for (int i = 0; i < 4; i++)
                        {
                                var weaponbarSlot = new NetworkMessage(message.NetID)
                                {
                                        PacketTemplate = new P317_ItemStreamWeaponBarSlot.PacketSt317
                                        {
                                                ItemStreamID = itemStreamID,
                                                LeadhandItemLocalID = chara.Data.Weaponset.GetLeadhand(i),
                                                OffhandItemLocalID = chara.Data.Weaponset.GetOffhand(i),
                                                SlotNumber = (byte)i
                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(weaponbarSlot);
                        }

                        var goldOnCharacter = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new P310_ItemStreamGoldOnCharacter.PacketSt310
                                {
                                        ItemStreamID = itemStreamID,
                                        GoldOnCharacter = 1337
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(goldOnCharacter);

                        // Note: ITEM STREAM Terminator
                        var terminator = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new P393_ItemStreamTerminator.PacketSt393
                                {
                                        // only works as terminator when this is 0
                                        Data1 = 0,
                                        GameMapID = (ushort)chara.Data.GameMapID.Value,
                                        Data3 = 0,
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(terminator);
                        

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt138> pParser;
        }
}
