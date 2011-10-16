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
using ServerEngine.GuildWars.Tools;
using ServerEngine.NetworkManagement;

namespace GameServer.Commands
{
        [CommandAttribute(Description = "No Parameters. Does stuff for testing purpose.")]
        class Test : IAction
        {
                private readonly CharID newCharID;

                public Test(CharID charID)
                {
                        newCharID = charID;
                }

                public void Execute(DataMap map)
                {
                        var chara = map.Get<DataCharacter>(newCharID);

                        //var packet343 = new NetworkMessage(chara.Data.NetID);
                        //var rawData = new byte[]{ // 1d01
                        //        0x57, 0x01, 
                        //        0xA2, 0x03, 0x00, 0x00, // ItemLocalID
                        //        0x0C, 0xAE, 0x02, 0x00, // ItemFileID
                        //        0x09, //unknown
                        //        0x00, //Color?
                        //        0x00, 0x00, // unknown always 0?
                        //        0xDC, 0x01, // unknown
                        //        0x01, // unknown
                        //        0x01, 0x02, 0x0D, 0x21, // bitfield: uniqueness, rarity etc. //3: 08:white B3:gold_unidentified
                        //        0x00, 0x00, 0x00, 0x00, // merchant price
                        //        0x13, 0x05, 0x00, 0x00, // itemID D1 09
                        //        0x01, 0x00, 0x00, 0x00, // quantitiy
                        //        //0x04, 0x00, // this and the following UTF16: item name
                        //        //0xCB, 0x56, 0xDC, 0xEA, 0x3E, 0xD4, 0xB7, 0x3C,
                        //        0x07, 0x00,
                        //        0x08, 0x01, 0x07, 0x01, 0x54, 0x00, 0x65, 0x00, 0x73, 0x00, 0x74, 0x00, 0x01, 0x00,
                        //        0x01, // this and the following ints: item mods.
                        //        //0x01, 0x00, 0x58, 0x24, 
                        //        0x02, 0x00, 0x88, 0x27, };
                        //packet343.PacketData = new MemoryStream(rawData);
                        //QueuingService.NetOutQueue.Enqueue(packet343);

                        //var posX = BitConverter.GetBytes(chara.Data.Position.X);
                        //var posY = BitConverter.GetBytes(chara.Data.Position.Y);
                        //var plane = BitConverter.GetBytes(chara.Data.Position.PlaneZ);

                        //var packet21 = new NetworkMessage(chara.Data.NetID);
                        //rawData = new byte[]{
                        //        0x15, 0x00, 
                        //        0x66, 0x00, 0x00, 0x00, 
                        //        0xA2, 0x03, 0x00, 0x00, //1d01
                        //        0x04, 
                        //        0x00, 
                        //        posX[0], posX[1],posX[2], posX[3], 
                        //        posY[0], posY[1],posY[2], posY[3], 
                        //        plane[0], plane[1], 
                        //        0x00, 0x00, 0x80, 0x3F, 
                        //        0x00, 0x00, 0x00, 0x00, 
                        //        0x01, 0x00, 0x00, 0x00, 
                        //        0x00, 
                        //        0x00, 0x00, 0x80, 0x3F, 
                        //        0x00, 0x00, 0x00, 0x34, 
                        //        0x00, 0x00, 0x00, 0x00, 0x00, 
                        //        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                        //        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                        //        0x00, 0x00, 0x00, 
                        //        0x00, 0x00, 0x80, 0x7F, 
                        //        0x00, 0x00, 0x80, 0x7F, 
                        //        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                        //        0x00, 0x00, 0x80, 0x7F, 
                        //        0x00, 0x00, 0x80, 0x7F, 
                        //        0x00, 0x00, };
                        //packet21.PacketData = new MemoryStream(rawData);
                        //QueuingService.NetOutQueue.Enqueue(packet21);

                        // Note: NPC GENERAL STATS
                        var npcStats = new NetworkMessage(chara.Data.NetID)
                        {
                                PacketTemplate = new P074_NpcGeneralStats.PacketSt74
                                {
                                        NpcID = 1095,
                                        FileID = 116228,
                                        Data1 = 0,
                                        Scale = (uint)(100 << 24),
                                        Data2 = 0,
                                        ProfessionFlags = 0,
                                        Profession = 1,
                                        Level = 20,
                                        ArraySize1 = (ushort)"roflmao".ToGW().Length,
                                        Appearance = Encoding.Unicode.GetBytes("roflmao".ToGW())
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(npcStats);

                        // Note: NPC MODEL
                        var npcModel = new NetworkMessage(chara.Data.NetID)
                        {
                                PacketTemplate = new P075_NpcModel.PacketSt75
                                {
                                        NpcID = 1095,
                                        ArraySize1 = 0,
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(npcModel);

                        // Note: UPDATE AGENT MAIN STATS
                        var charMain = new NetworkMessage(chara.Data.NetID)
                        {
                                PacketTemplate = new P021_SpawnObject.PacketSt21
                                {
                                        AgentID = 20,
                                        Data1 = (0x20 << 24) | 1095, // was assumed to be LocalID
                                        Data2 = 1,
                                        Data3 = 9,
                                        PosX = chara.Data.Position.X,
                                        PosY = chara.Data.Position.Y,
                                        Plane = (ushort)chara.Data.Position.PlaneZ,
                                        Data4 = float.PositiveInfinity,
                                        Rotation = 1F,
                                        Data5 = 1,
                                        Speed = 288,
                                        Data12 = 1F,
                                        Data13 = 0x41400000,
                                        //Data14 = 1886151033,
                                        Data14 = 1852796515,
                                        Data15 = 0,
                                        Data16 = 0,
                                        Data17 = 0,
                                        Data18 = 0,
                                        Data19 = 0,
                                        Data20 = 0,
                                        Data21 = 0,
                                        Data22 = float.PositiveInfinity,
                                        Data23 = float.PositiveInfinity,
                                        Data24 = 0,
                                        Data25 = 0,
                                        Data26 = float.PositiveInfinity,
                                        Data27 = float.PositiveInfinity,
                                        Data28 = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(charMain);
                }
        }
}
