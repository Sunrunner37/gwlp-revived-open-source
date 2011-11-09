using System;
using GameServer.Enums;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.GuildWars.Tools;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 131)]
        public class Packet131 : IPacket
        {
                public class PacketSt131 : IPacketTemplate
                {
                        public UInt16 Header { get { return 131; } }
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt131>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        /*var ppp0 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P018_InstanceLoadZoneDataHeader.PacketSt18
                                {
                                        ArraySize1 = 56,
                                        Data1 = new byte[] {
                0x06,
                0x04,
                0x00,
                0xD6,
                0x00,
                0x01,
                0x00,
                0x02,
                0x38,
                0x00,
                0x19,
                0x10,
                0x01,
                0x62,
                0x40,
                0x00,
                0x88,
                0x80,
                0x00,
                0x02,
                0x24,
                0x30,
                0x13,
                0x00,
                0x24,
                0x01,
                0x08,
                0x50,
                0x22,
                0x08,
                0xA0,
                0x11,
                0x00,
                0x00,
                0x20,
                0x02,
                0x21,
                0x29,
                0x2A,
                0x02,
                0x04,
                0x04,
                0x00,
                0xB4,
                0x00,
                0x08,
                0x00,
                0x40,
                0x43,
                0x97,
                0x40,
                0x08,
                0x00,
                0x07,
                0x1B,
                0x40 }
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp0);*/

                        // Note: ZONE DATA HEADER
                        var zdHead = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new P018_InstanceLoadZoneDataHeader.PacketSt18
                                {
                                        ArraySize1 = 70,
                                        Data1 = new byte[] {0x66, 0xE4, 0xA6, 0xFE, 0xE7, 0x5D, 0x0C, 0x66,
                                                        0x3A, 0x3F, 0xFB, 0x11, 0x50, 0xE3, 0xFC, 0x0D, 0xA0, 0xEE, 0x06, 0x7B,
                                                        0x74, 0xB3, 0xDB, 0xFD, 0xFF, 0x0D, 0xC8, 0xDF, 0x22, 0x58, 0xBE, 0xD7,
                                                        0xBF, 0x9C, 0xB3, 0x8E, 0xF7, 0xFB, 0xFE, 0x63, 0x4C, 0x7C, 0xDB, 0xB4,
                                                        0x8D, 0x4C, 0x09, 0x55, 0x43, 0xDD, 0x00, 0x18, 0x00, 0x07, 0x1B, 0x46,
                                                        0xEC, 0x6F, 0x5A, 0x3D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                        0x00, 0x00, 0x00, 0x1C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                        0x00, 0x00, 0x00, 0x00, 0x94, 0x97, 0xDF, 0x03, 0x71, 0x60, 0x8B, 0x57,
                                                        0x01, 0x01, 0x20, 0x0C, 0x82, 0x50, 0xD3, 0x00, 0x04, 0xC0, 0x8C, 0x2A,
                                                        0x56, 0x81, 0x40, 0x41, 0x10, 0x20, 0xE8, 0xA4, 0x00, 0x00, 0x00, 0x5C,
                                                        0x9D, 0x21, 0x38, 0x42, 0x40, 0x00, 0x60, 0x10, 0x68, 0x01, 0x00, 0x2C,
                                                        0x4A, 0x29, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x90, 0x00, 0x28,
                                                        0xCC, 0xE2, 0x76, 0xC0, 0x3A, 0x2E, 0x3A, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                        0x00, 0x00, 0x80, 0x10, 0x05, 0x11, 0x39, 0xCC, 0x78, 0x00, 0x9A, 0x93,
                                                        0x58, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0xDF, 0xB2, 0x25, 0xD0,
                                                        0x2F, 0xFD, 0xE7, 0x85, 0xB8, 0xBD, 0xF8, 0x30, 0x20, 0x16, 0xA3, 0x48,
                                                        0x2B, 0x00, 0x00, 0x00, 0x60, 0xC6, 0xCE, 0x44, 0x00, 0x00, 0xC0, 0x0E,
                                                        0x22, 0x00, 0x00, 0x85, 0x71, 0xD7, 0x71, 0xA1, 0xDC, 0x6B, 0x7B, 0x01,
                                                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                        0x58, 0x09, 0x5C, 0x18, 0x03, 0x00, 0x00, 0x00, 0x00, 0xE5, 0xB3, 0x06,
                                                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x59, 0x50, 0x00, 0x00, 0x00,
                                                        0x00, 0xA0, 0x4C, 0x65, 0x01, 0x00, 0x00, 0x40, 0x0D, 0x00, 0x01, 0x00,
                                                        0xFA, 0x4E, 0xAD, 0x0F
                                                }
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(zdHead);


                        var ppp1 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.Packet13.PacketSt13
                                {
                                        ArraySize1 = 1,
                                        Data1 = new uint[] {
                0x0D2D0ED8 }
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp1);

                        var ppp2 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.Packet16.PacketSt16()
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp2);

                        var ppp3 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.Packet4.PacketSt4
                                {
                                        Data1 = 1,
                                        Data2 = 6,
                                        Data3 = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp3);

                        var ppp4 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.Packet4.PacketSt4
                                {
                                        Data1 = 98,
                                        Data2 = 2,
                                        Data3 = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp4);

                        var ppp5 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P171_UpdatePrivProfessions.PacketSt171
                                {
                                        ID1 = 37,
                                        Prof1 = 1,
                                        Prof2 = 0,
                                        Data3 = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp5);

                        /*var ppp6 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P207_UpdateAvailableSkills.PacketSt207
                                {
                                        ArraySize1 = 0,
                                        SkillsBitfield = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp6);*/

                        var ppp7 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P343_ItemGeneral.PacketSt343
                                {
                                        LocalID = 1932,
                                        FileID = 2147484993,
                                        ItemType = 7,
                                        Data2 = 19,
                                        DyeColor = 11,
                                        Data4 = 0,
                                        CanBeDyed = 0,
                                        Flags = 536876038,
                                        MerchantPrice = 0,
                                        ItemID = 15700,
                                        Quantity = 1,
                                        NameHash = "Someitem".ToGW(),
                                        NumStats = 3,
                                        Stats = new uint[] {
                0xA3C81900,
                0x80400000,
                0xA0F80A00 }
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp7);

                        var ppp8 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P336_ItemProfession.PacketSt336
                                {
                                        ItemLocalID = 1932,
                                        Profession = 1
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp8);

                        var ppp9 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P308_ItemLocation.PacketSt308
                                {
                                        ItemStreamID = 23,
                                        ItemLocalID = 1932,
                                        PageID = 97,
                                        UserSlot = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp9);

                        var ppp10 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P321_MoveItem.PacketSt321
                                {
                                        ItemStreamID = 23,
                                        ItemLocalID = 1932,
                                        NewPageID = 330,
                                        NewSlot = 2
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp10);

                        var ppp11 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P343_ItemGeneral.PacketSt343
                                {
                                        LocalID = 240,
                                        FileID = 2147484995,
                                        ItemType = 19,
                                        Data2 = 19,
                                        DyeColor = 11,
                                        Data4 = 0,
                                        CanBeDyed = 0,
                                        Flags = 536876038,
                                        MerchantPrice = 0,
                                        ItemID = 15703,
                                        Quantity = 1,
                                        NameHash = "Someitem".ToGW(),
                                        NumStats = 3,
                                        Stats = new uint[] {
                0xA3C81900,
                0x80400000,
                0xA0F80A00 }
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp11);

                        var ppp12 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P336_ItemProfession.PacketSt336
                                {
                                        ItemLocalID = 240,
                                        Profession = 1
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp12);

                        var ppp13 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P308_ItemLocation.PacketSt308
                                {
                                        ItemStreamID = 23,
                                        ItemLocalID = 240,
                                        PageID = 97,
                                        UserSlot = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp13);

                        var ppp14 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P321_MoveItem.PacketSt321
                                {
                                        ItemStreamID = 23,
                                        ItemLocalID = 240,
                                        NewPageID = 330,
                                        NewSlot = 3
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp14);

                        var ppp15 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P343_ItemGeneral.PacketSt343
                                {
                                        LocalID = 809,
                                        FileID = 2147484992,
                                        ItemType = 4,
                                        Data2 = 19,
                                        DyeColor = 11,
                                        Data4 = 0,
                                        CanBeDyed = 0,
                                        Flags = 536876038,
                                        MerchantPrice = 0,
                                        ItemID = 15699,
                                        Quantity = 1,
                                        NameHash = "Someitem".ToGW(),
                                        NumStats = 3,
                                        Stats = new uint[] {
                0xA3C81900,
                0x80400000,
                0xA0F80A00 }
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp15);

                        var ppp16 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P336_ItemProfession.PacketSt336
                                {
                                        ItemLocalID = 809,
                                        Profession = 1
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp16);

                        var ppp17 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P308_ItemLocation.PacketSt308
                                {
                                        ItemStreamID = 23,
                                        ItemLocalID = 809,
                                        PageID = 97,
                                        UserSlot = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp17);

                        var ppp18 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P321_MoveItem.PacketSt321
                                {
                                        ItemStreamID = 23,
                                        ItemLocalID = 809,
                                        NewPageID = 330,
                                        NewSlot = 5
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp18);

                        var ppp19 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P343_ItemGeneral.PacketSt343
                                {
                                        LocalID = 135,
                                        FileID = 2147484994,
                                        ItemType = 13,
                                        Data2 = 19,
                                        DyeColor = 11,
                                        Data4 = 0,
                                        CanBeDyed = 0,
                                        Flags = 536876038,
                                        MerchantPrice = 0,
                                        ItemID = 15701,
                                        Quantity = 1,
                                        NameHash = "Someitem".ToGW(),
                                        NumStats = 3,
                                        Stats = new uint[] {
                0xA3C81900,
                0x80400000,
                0xA0F80A00 }
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp19);

                        var ppp20 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P336_ItemProfession.PacketSt336
                                {
                                        ItemLocalID = 135,
                                        Profession = 1
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp20);

                        var ppp21 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P308_ItemLocation.PacketSt308
                                {
                                        ItemStreamID = 23,
                                        ItemLocalID = 135,
                                        PageID = 97,
                                        UserSlot = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp21);

                        var ppp22 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P321_MoveItem.PacketSt321
                                {
                                        ItemStreamID = 23,
                                        ItemLocalID = 135,
                                        NewPageID = 330,
                                        NewSlot = 6
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp22);

                        var ppp23 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P343_ItemGeneral.PacketSt343
                                {
                                        LocalID = 2002,
                                        FileID = 2147484991,
                                        ItemType = 16,
                                        Data2 = 19,
                                        DyeColor = 11,
                                        Data4 = 0,
                                        CanBeDyed = 0,
                                        Flags = 536876038,
                                        MerchantPrice = 0,
                                        ItemID = 15702,
                                        Quantity = 1,
                                        NameHash = "Someitem".ToGW(),
                                        NumStats = 3,
                                        Stats = new uint[] {
                0xA3C81900,
                0x80400000,
                0xA0F80A00 }
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp23);

                        var ppp24 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P336_ItemProfession.PacketSt336
                                {
                                        ItemLocalID = 2002,
                                        Profession = 1
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp24);

                        var ppp25 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P308_ItemLocation.PacketSt308
                                {
                                        ItemStreamID = 23,
                                        ItemLocalID = 2002,
                                        PageID = 97,
                                        UserSlot = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp25);

                        var ppp26 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P321_MoveItem.PacketSt321
                                {
                                        ItemStreamID = 23,
                                        ItemLocalID = 2002,
                                        NewPageID = 330,
                                        NewSlot = 4
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp26);

                        var ppp27 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P343_ItemGeneral.PacketSt343
                                {
                                        LocalID = 2002,
                                        FileID = 2147484991,
                                        ItemType = 16,
                                        Data2 = 19,
                                        DyeColor = 4,
                                        Data4 = 0,
                                        CanBeDyed = 0,
                                        Flags = 536876038,
                                        MerchantPrice = 0,
                                        ItemID = 15702,
                                        Quantity = 1,
                                        NameHash = "Someitem".ToGW(),
                                        NumStats = 3,
                                        Stats = new uint[] {
                0xA3C81900,
                0x80400000,
                0xA0F80A00 }
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp27);

                        var ppp28 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P336_ItemProfession.PacketSt336
                                {
                                        ItemLocalID = 2002,
                                        Profession = 1
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp28);

                        var ppp29 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.Packet337.PacketSt337
                                {
                                        Data1 = 2002,
                                        Data2 = 2002
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp29);

                        var ppp30 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P343_ItemGeneral.PacketSt343
                                {
                                        LocalID = 1932,
                                        FileID = 2147484993,
                                        ItemType = 7,
                                        Data2 = 19,
                                        DyeColor = 4,
                                        Data4 = 0,
                                        CanBeDyed = 0,
                                        Flags = 536876038,
                                        MerchantPrice = 0,
                                        ItemID = 15700,
                                        Quantity = 1,
                                        NameHash = "Someitem".ToGW(),
                                        NumStats = 3,
                                        Stats = new uint[] {
                0xA3C81900,
                0x80400000,
                0xA0F80A00 }
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp30);

                        var ppp31 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P336_ItemProfession.PacketSt336
                                {
                                        ItemLocalID = 1932,
                                        Profession = 1
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp31);

                        var ppp32 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.Packet337.PacketSt337
                                {
                                        Data1 = 1932,
                                        Data2 = 1932
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp32);

                        var ppp33 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P343_ItemGeneral.PacketSt343
                                {
                                        LocalID = 240,
                                        FileID = 2147484995,
                                        ItemType = 19,
                                        Data2 = 19,
                                        DyeColor = 4,
                                        Data4 = 0,
                                        CanBeDyed = 0,
                                        Flags = 536876038,
                                        MerchantPrice = 0,
                                        ItemID = 15703,
                                        Quantity = 1,
                                        NameHash = "Someitem".ToGW(),
                                        NumStats = 3,
                                        Stats = new uint[] {
                0xA3C81900,
                0x80400000,
                0xA0F80A00 }
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp33);

                        var ppp34 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P336_ItemProfession.PacketSt336
                                {
                                        ItemLocalID = 240,
                                        Profession = 1
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp34);

                        var ppp35 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.Packet337.PacketSt337
                                {
                                        Data1 = 240,
                                        Data2 = 240
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp35);

                        var ppp36 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P343_ItemGeneral.PacketSt343
                                {
                                        LocalID = 135,
                                        FileID = 2147484994,
                                        ItemType = 13,
                                        Data2 = 19,
                                        DyeColor = 4,
                                        Data4 = 0,
                                        CanBeDyed = 0,
                                        Flags = 536876038,
                                        MerchantPrice = 0,
                                        ItemID = 15701,
                                        Quantity = 1,
                                        NameHash = "Someitem".ToGW(),
                                        NumStats = 3,
                                        Stats = new uint[] {
                0xA3C81900,
                0x80400000,
                0xA0F80A00 }
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp36);

                        var ppp37 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P336_ItemProfession.PacketSt336
                                {
                                        ItemLocalID = 135,
                                        Profession = 1
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp37);

                        var ppp38 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.Packet337.PacketSt337
                                {
                                        Data1 = 135,
                                        Data2 = 135
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp38);

                        var ppp39 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P343_ItemGeneral.PacketSt343
                                {
                                        LocalID = 809,
                                        FileID = 2147484992,
                                        ItemType = 4,
                                        Data2 = 19,
                                        DyeColor = 4,
                                        Data4 = 0,
                                        CanBeDyed = 0,
                                        Flags = 536876038,
                                        MerchantPrice = 0,
                                        ItemID = 15699,
                                        Quantity = 1,
                                        NameHash = "Someitem".ToGW(),
                                        NumStats = 3,
                                        Stats = new uint[] {
                0xA3C81900,
                0x80400000,
                0xA0F80A00 }
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp39);

                        var ppp40 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P336_ItemProfession.PacketSt336
                                {
                                        ItemLocalID = 809,
                                        Profession = 1
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp40);

                        var ppp41 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.Packet337.PacketSt337
                                {
                                        Data1 = 809,
                                        Data2 = 809
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp41);

                        GameServerWorld.Instance.Get<DataClient>(message.NetID).Data.Status = SyncStatus.Playing;

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt131> pParser;
        }
}
