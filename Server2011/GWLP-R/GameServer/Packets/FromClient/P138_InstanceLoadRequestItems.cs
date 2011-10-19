using System;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

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

#warning FIXME: Item stuff is not implemented!
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

                        // Note: ITEM STREAM HEAD2
                        var head2 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new P318_UpdateActiveWeaponslot.PacketSt318
                                {
                                        ItemStreamID = itemStreamID,
                                        ActiveWeaponSlot = 0,
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(head2);

                        var Backpack = new NetworkMessage(chara.Data.NetID)
                        {
                            PacketTemplate = new P343_ItemGeneral.PacketSt343
                            {
                                LocalID = 42,
                                FileID = 0x8001B536,
                                ItemType = 0x3,
                                Data2 = 1,
                                DyeColor = 0,
                                Data4 = 0,
                                CanBeDyed = 0,
                                Flags = 0x20001000,
                                MerchantPrice = 5,
                                ItemID = 0x00000020,
                                Quantity = 1,
                                NameHash = BitConverter.ToChar(new byte[] { 0x08, 0x01 }, 0).ToString() + BitConverter.ToChar(new byte[] { 0x07, 0x01 }, 0).ToString() + "Hacker's Backpack" + BitConverter.ToChar(new byte[] { 0x01, 0x00 }, 0).ToString(),
                                NumStats = 0x01,
                                Stats = new UInt32[] { 0x24481400 }
                            }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(Backpack);
                        
                        /*var OwnerName = new NetworkMessage(chara.Data.NetID)
                        {
                            PacketTemplate = new P304_ItemOwnerName.PacketSt304
                            {
                                ItemLocalID = 42,
                                CharName = BitConverter.ToChar(new byte[] { 0x08, 0x01 }, 0).ToString() + BitConverter.ToChar(new byte[] { 0x07, 0x01 }, 0).ToString() + "Coca Cola" + BitConverter.ToChar(new byte[] { 0x01, 0x00 }, 0).ToString(),
                            }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(OwnerName);*/

                        var Page = new NetworkMessage(chara.Data.NetID)
                        {
                            PacketTemplate = new P309_ItemPagePacket.PacketSt309
                            {
                                  ItemStreamID = itemStreamID,
                                  StorageType = 1,
                                  StorageID = 0,
                                  PageID = 2,
                                  Slots = 20,
                                  ItemLocalID = 42
                            }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(Page);

                        var testItem = new NetworkMessage(chara.Data.NetID)
                        {
                            PacketTemplate = new P343_ItemGeneral.PacketSt343
                            {
                                LocalID = 41,
                                FileID = 0x80038637,
                                ItemType = 0x16,
                                Data2 = 4,
                                DyeColor = 0,
                                Data4 = 0,
                                CanBeDyed = 0,
                                Flags = 0x2200C611,
                                MerchantPrice = 1337,
                                ItemID = 0,
                                Quantity = 1,
                                NameHash = BitConverter.ToChar(new byte[] { 0x08, 0x01 }, 0).ToString() + BitConverter.ToChar(new byte[] { 0x07, 0x01 }, 0).ToString() + "Hacker's Earthwand" + BitConverter.ToChar(new byte[] { 0x01, 0x00 }, 0).ToString(),
                                NumStats = 0x05,
                                Stats = new UInt32[] {  0x24B80B00,
                                            0x26980003,
                                            0x22186409,
                                            0xA4980A10,
                                            0xA488C864}
                            }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(testItem);

                        var testLocation = new NetworkMessage(chara.Data.NetID)
                        {
                            PacketTemplate = new P308_ItemLocation.PacketSt308
                            {
                                ItemStreamID = itemStreamID,
                                ItemLocalID = 41,
                                PageID = 2,
                                UserSlot = 7
                            }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(testLocation);

                        // Note: ITEM STREAM WEAPON BAR SLOTS
                        // (would go here)

                        // Note: ITEM STREAM END
                        // (would go here)

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
