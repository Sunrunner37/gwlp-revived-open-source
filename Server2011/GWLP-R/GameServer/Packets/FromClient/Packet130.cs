using System;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.GuildWars.Tools;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 130)]
        public class Packet130 : IPacket
        {
                public class PacketSt130 : IPacketTemplate
                {
                        public UInt16 Header { get { return 130; } }
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt130>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // get the character
                        var chara = GameServerWorld.Instance.Get<DataClient>(message.NetID).Character;
                        const ushort itemStreamID = 1;

                        var ppp2 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P314_ItemStreamHead.PacketSt314
                                {
                                        ItemStreamID = 23,
                                        Data2 = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp2);

                        var ppp3 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P318_UpdateActiveWeaponset.PacketSt318
                                {
                                        ItemStreamID = 23,
                                        ActiveWeaponSlot = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp3);

                        var ppp4 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P343_ItemGeneral.PacketSt343
                                {
                                        LocalID = 2483,
                                        FileID = 2147595574,
                                        ItemType = 3,
                                        Data2 = 1,
                                        DyeColor = 0,
                                        Data4 = 0,
                                        CanBeDyed = 0,
                                        Flags = 536875008,
                                        MerchantPrice = 5,
                                        ItemID = 32,
                                        Quantity = 1,
                                        NameHash = "Backypacky".ToGW(),
                                        NumStats = 1,
                                        Stats = new uint[] {
                0x24481400 }
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp4);

                        var ppp5 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P309_ItemPagePacket.PacketSt309
                                {
                                        ItemStreamID = 23,
                                        StorageType = 1,
                                        StorageID = 0,
                                        PageID = 97,
                                        Slots = 20,
                                        ItemLocalID = 2483
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp5);

                        var ppp6 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P309_ItemPagePacket.PacketSt309
                                {
                                        ItemStreamID = 23,
                                        StorageType = 2,
                                        StorageID = 16,
                                        PageID = 330,
                                        Slots = 9,
                                        ItemLocalID = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp6);

                        var ppp7 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P309_ItemPagePacket.PacketSt309
                                {
                                        ItemStreamID = 23,
                                        StorageType = 3,
                                        StorageID = 6,
                                        PageID = 119,
                                        Slots = 12,
                                        ItemLocalID = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp7);

                        var ppp8 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P309_ItemPagePacket.PacketSt309
                                {
                                        ItemStreamID = 23,
                                        StorageType = 4,
                                        StorageID = 7,
                                        PageID = 67,
                                        Slots = 20,
                                        ItemLocalID = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp8);

                        var ppp9 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P309_ItemPagePacket.PacketSt309
                                {
                                        ItemStreamID = 23,
                                        StorageType = 4,
                                        StorageID = 8,
                                        PageID = 93,
                                        Slots = 20,
                                        ItemLocalID = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp9);

                        var ppp10 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P309_ItemPagePacket.PacketSt309
                                {
                                        ItemStreamID = 23,
                                        StorageType = 4,
                                        StorageID = 9,
                                        PageID = 88,
                                        Slots = 20,
                                        ItemLocalID = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp10);

                        var ppp11 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P309_ItemPagePacket.PacketSt309
                                {
                                        ItemStreamID = 23,
                                        StorageType = 4,
                                        StorageID = 10,
                                        PageID = 161,
                                        Slots = 20,
                                        ItemLocalID = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp11);

                        var ppp12 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P309_ItemPagePacket.PacketSt309
                                {
                                        ItemStreamID = 23,
                                        StorageType = 5,
                                        StorageID = 5,
                                        PageID = 24,
                                        Slots = 41,
                                        ItemLocalID = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp12);

                        var ppp13 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P317_ItemStreamWeaponBarSlot.PacketSt317
                                {
                                        ItemStreamID = 23,
                                        SlotNumber = 0,
                                        LeadhandItemLocalID = 0,
                                        OffhandItemLocalID = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp13);

                        var ppp14 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P317_ItemStreamWeaponBarSlot.PacketSt317
                                {
                                        ItemStreamID = 23,
                                        SlotNumber = 1,
                                        LeadhandItemLocalID = 0,
                                        OffhandItemLocalID = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp14);

                        var ppp15 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P317_ItemStreamWeaponBarSlot.PacketSt317
                                {
                                        ItemStreamID = 23,
                                        SlotNumber = 2,
                                        LeadhandItemLocalID = 0,
                                        OffhandItemLocalID = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp15);

                        var ppp16 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P317_ItemStreamWeaponBarSlot.PacketSt317
                                {
                                        ItemStreamID = 23,
                                        SlotNumber = 3,
                                        LeadhandItemLocalID = 0,
                                        OffhandItemLocalID = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp16);

                        var ppp17 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.Packet79.PacketSt79
                                {
                                        Data1 = 2
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp17);

                        var ppp18 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P222_UpdateMaxKurzickFaction.PacketSt222
                                {
                                        MaxKurzickFaction = 25000
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp18);

                        var ppp19 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P223_UpdateMaxLuxonFaction.PacketSt223
                                {
                                        MaxLuxonFaction = 10000
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp19);

                        var ppp20 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P222_UpdateMaxKurzickFaction.PacketSt222
                                {
                                        MaxKurzickFaction = 25000
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp20);

                        var ppp21 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P223_UpdateMaxLuxonFaction.PacketSt223
                                {
                                        MaxLuxonFaction = 10000
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp21);

                        var ppp22 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P224_UpdateMaxBalthazarFaction.PacketSt224
                                {
                                        MaxBalthazarFaction = 17000
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp22);

                        var ppp23 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P225_UpdateMaxImperialFaction.PacketSt225
                                {
                                        MaxImperialFaction = 35000
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp23);

                        var ppp24 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P044_UpdateAttribPts.PacketSt44
                                {
                                        ID1 = 37,
                                        FreePts = 0,
                                        MaxPts = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp24);

                        var ppp25 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P171_UpdatePrivProfessions.PacketSt171
                                {
                                        ID1 = 37,
                                        Prof1 = 0,
                                        Prof2 = 0,
                                        Data3 = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp25);

                        var ppp26 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.Packet170.PacketSt170
                                {
                                        ID1 = 37,
                                        Data1 = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp26);

                        var ppp27 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P206_UpdateSkillBar.PacketSt206
                                {
                                        ID1 = 37,
                                        ArraySize1 = 8,
                                        SkillBar = new uint[] {
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000},
                ArraySize2 = 8,
                                        SkillBarPvPMask = new uint[] {
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000,
                0x00000000},
                Data3 = 1
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp27);

                        var ppp28 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P147_UpdateGenericValueInt.PacketSt147
                                {
                                        ValueID = 41,
                                        AgentID = 37,
                                        Value = 1
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp28);

                        var ppp29 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P147_UpdateGenericValueInt.PacketSt147
                                {
                                        ValueID = 42,
                                        AgentID = 37,
                                        Value = 1
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp29);

                        var ppp30 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P144_UpdateMorale.PacketSt144
                                {
                                        ID1 = 37,
                                        Morale = 100
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp30);

                        var ppp31 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P221_UpdateFactionPts.PacketSt221
                                {
                                        ExpPts = 0,
                                        KurzFree = 1000,
                                        KurzTotal = 15500,
                                        LuxFree = 480,
                                        LuxTotal = 6730,
                                        ImpFree = 9450,
                                        ImpTotal = 20930,
                                        Data1 = 0,
                                        Data2 = 0,
                                        Level = 1,
                                        Morale = 100,
                                        BalthFree = 10880,
                                        BalthTotal = 143280,
                                        SkillFree = 0,
                                        SkillTotal = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp31);

                        var ppp32 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P227_UpdateEquipmentDisplayStatus.PacketSt227
                                {
                                        DisplayStatus = 255,
                                        DisplayPart = 255
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp32);

                        var ppp33 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P171_UpdatePrivProfessions.PacketSt171
                                {
                                        ID1 = 37,
                                        Prof1 = 1,
                                        Prof2 = 0,
                                        Data3 = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp33);

                        var ppp34 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P207_UpdateAvailableSkills.PacketSt207
                                {
                                        ArraySize1 = 0,
                                        SkillsBitfield = new byte[0]
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp34);

                        var ppp35 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P147_UpdateGenericValueInt.PacketSt147
                                {
                                        ValueID = 41,
                                        AgentID = 37,
                                        Value = 20
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp35);

                        var ppp36 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P150_UpdateGenericValueFloat.PacketSt150
                                {
                                        ValueID = 43,
                                        AgentID = 37,
                                        Value = (float) 0.0329999998211861
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp36);

                        var ppp37 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P147_UpdateGenericValueInt.PacketSt147
                                {
                                        ValueID = 42,
                                        AgentID = 37,
                                        Value = 100
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp37);

                        var ppp38 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P147_UpdateGenericValueInt.PacketSt147
                                {
                                        ValueID = 64,
                                        AgentID = 37,
                                        Value = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp38);

                        var ppp39 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.Packet380.PacketSt380()
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp39);

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt130> pParser;
        }
}
