using System;
using System.Linq;
using GameServer.Enums;
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
                        Console.WriteLine("wabadoo");
                        // parse the message
                        var pack = new PacketSt138();
                        pParser(pack, message.PacketData);

                        // get the character
                        var client = GameServerWorld.Instance.Get<DataClient>(message.NetID);
                        var chara = client.Character;
                        var map = GameServerWorld.Instance.Get<DataMap>(chara.Data.MapID);
                        const ushort itemStreamID = 1;

                        // update the clients status
                        /*client.Data.Status = SyncStatus.TriesToLoadInstance;

                        // Note: INSTANCE LOAD HEADER
                        var ilHeader = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new P370_InstanceLoadHead.PacketSt370()
                                {
                                        Data1 = (byte)(chara.Data.IsOutpost ? 0x3F : 0x1F),
                                        Data2 = (byte)(chara.Data.IsOutpost ? 0x3F : 0x1F),
                                        Data3 = 0x00,
                                        Data4 = 0x00,
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ilHeader);


                        // Note: INSTANCE LOAD CHAR NAME
                        var ilChar = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new P371_InstanceLoadCharName.PacketSt371
                                {
                                        CharName = chara.Data.Name.Value,
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ilChar);

                        // Note: INSTANCE LOAD DISTRICT INFO
                        var ilDInfo = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new P395_InstanceLoadDistrictInfo.PacketSt395
                                {
                                        LocalID = chara.Data.LocalID.Value,
                                        GameMapID = (ushort)map.Data.GameMapID.Value,
                                        DistrictNumber = (ushort)(chara.Data.IsOutpost ? map.Data.DistrictNumber : 0),
                                        DistrictRegion = (ushort)(chara.Data.IsOutpost ? map.Data.DistrictCountry : 0),
                                        IsOutpost = (byte)(chara.Data.IsOutpost ? 1 : 0),
                                        ObserverMode = 0,
                                        Data1 = (byte)(chara.Data.IsOutpost ? 0 : 3),
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ilDInfo);*/

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

                        // Note: ACTIVE WEAPONSET
                        var activeWeaponset = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new P318_UpdateActiveWeaponset.PacketSt318
                                {
                                        ItemStreamID = itemStreamID,
                                        ActiveWeaponSlot = (byte)chara.Data.Items.ActiveWeaponset.Number,
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
                        foreach (var charItem in chara.Data.Items)
                        {
                                charItem.Value.SendPackets(message.NetID);
                        }

                        //weaponbar slots
                        for (int i = 0; i < 4; i++)
                        {
                                foreach (var weaponset in chara.Data.Items.Weaponsets.Values)
                                {
                                        weaponset.SendPackets(message.NetID, itemStreamID);
                                }
                        }

                        var goldOnCharacter = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new P310_AddGoldOnCharacter.PacketSt310
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
