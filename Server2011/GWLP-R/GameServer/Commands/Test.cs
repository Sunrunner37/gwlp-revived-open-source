using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GameServer.Enums;
using GameServer.Interfaces;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.GuildWars.DataWrappers.Clients;
using ServerEngine.NetworkManagement;

namespace GameServer.Commands
{
        [CommandAttribute(Description = "No Parameters. Does stuff for testing purpose.")]
        class Test : IAction
        {
                private CharID newCharID;

                public Test(CharID charID)
                {
                        newCharID = charID;
                }

                public void Execute(DataMap map)
                {
                        var chara = map.Get<DataCharacter>(newCharID);

                        //var packet323 = new NetworkMessage(netID);
                        //var rawData = new byte[]{
                        //        0x43, 0x01, 0x01, 0x00, 0xBE, 0x00, 0x00, 0x00, };
                        //packet323.PacketData = new MemoryStream(rawData);
                        //QueuingService.NetOutQueue.Enqueue(packet323);

                        var packet343 = new NetworkMessage(chara.Data.NetID);
                        var rawData = new byte[]{ // 1d01
                                0x57, 0x01, 
                                0xA2, 0x03, 0x00, 0x00, // ItemLocalID
                                0x0C, 0xAE, 0x02, 0x00, // ItemFileID
                                0x09, //unknown
                                0x00, //Color?
                                0x00, 0x00, // unknown always 0?
                                0xDC, 0x01, // unknown
                                0x01, // unknown
                                0x01, 0x02, 0x0D, 0x21, // bitfield: uniqueness, rarity etc. //3: 08:white B3:gold_unidentified
                                0x00, 0x00, 0x00, 0x00, // merchant price
                                0x13, 0x05, 0x00, 0x00, // itemID D1 09
                                0x01, 0x00, 0x00, 0x00, // quantitiy
                                //0x04, 0x00, // this and the following UTF16: item name
                                //0xCB, 0x56, 0xDC, 0xEA, 0x3E, 0xD4, 0xB7, 0x3C,
                                0x06, 0x00,
                                0x54, 0x65, 0x73, 0x74, 0x49, 0x74, 0x65, 0x6D, 0x4E, 0x61, 0x6D, 0x65,
                                0x01, // this and the following ints: item mods.
                                //0x01, 0x00, 0x58, 0x24, 
                                0x02, 0x00, 0x88, 0x27, };
                        packet343.PacketData = new MemoryStream(rawData);
                        QueuingService.NetOutQueue.Enqueue(packet343);

                        /*
                        55 01 
                         * 31 01 00 00 
                         * 0C AE 02 00 
                         * 1A 
                         * 02 
                         * 00 00 
                         * DC 01 
                         * 01 
                         * 40 04 B3 2E 
                         * 32 00 00 00 
                         * 13 05 00 00 
                         * 01 00 00 00 
                         * 04 00 
                         * CB 56 DC EA 3E D4 B7 3C 
                         * 06 
                         * 01 14 A8 23 
                         * 0C 22 98 27 
                         * 00 03 B8 24 
                         * 06 00 98 26 
                         * 00 0A C8 62 
                         * 0B 16 A8 A7
                         */
                        //var packet299 = new NetworkMessage(netID);
                        //rawData = new byte[]{
                        //        0x2B, 0x01, 0x1D, 0x01, 0x00, 0x00, 0x19, 0x00, 0x00, 0x00, 0x16, 0x44, 
                        //        };
                        //packet299.PacketData = new MemoryStream(rawData);
                        //QueuingService.NetOutQueue.Enqueue(packet299);

                        //var packet350 = new NetworkMessage(netID);
                        //rawData = new byte[]{
                        //        0x5E, 0x01, 0x66, 0x00, 0x1A, 0x00, };
                        //packet350.PacketData = new MemoryStream(rawData);
                        //QueuingService.NetOutQueue.Enqueue(packet350);

                        var posX = BitConverter.GetBytes(chara.Data.Position.X);
                        var posY = BitConverter.GetBytes(chara.Data.Position.Y);
                        var plane = BitConverter.GetBytes(chara.Data.Position.PlaneZ);

                        var packet21 = new NetworkMessage(chara.Data.NetID);
                        rawData = new byte[]{
                                0x15, 0x00, 
                                0x66, 0x00, 0x00, 0x00, 
                                0xA2, 0x03, 0x00, 0x00, //1d01
                                0x04, 
                                0x00, 
                                posX[0], posX[1],posX[2], posX[3], 
                                posY[0], posY[1],posY[2], posY[3], 
                                plane[0], plane[1], 
                                0x00, 0x00, 0x80, 0x3F, 
                                0x00, 0x00, 0x00, 0x00, 
                                0x01, 0x00, 0x00, 0x00, 
                                0x00, 
                                0x00, 0x00, 0x80, 0x3F, 
                                0x00, 0x00, 0x00, 0x34, 
                                0x00, 0x00, 0x00, 0x00, 0x00, 
                                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                                0x00, 0x00, 0x00, 
                                0x00, 0x00, 0x80, 0x7F, 
                                0x00, 0x00, 0x80, 0x7F, 
                                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                                0x00, 0x00, 0x80, 0x7F, 
                                0x00, 0x00, 0x80, 0x7F, 
                                0x00, 0x00, };
                        packet21.PacketData = new MemoryStream(rawData);
                        QueuingService.NetOutQueue.Enqueue(packet21);
                }
        }
}
