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
using ServerEngine.GuildWars.Tools;

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
            DateTime dt = new DateTime();
            dt = DateTime.Now;
            UInt32 LocID = (uint)dt.Second;

            var testItem = new NetworkMessage(chara.Data.NetID)
            {
                PacketTemplate = new P343_ItemGeneral.PacketSt343
                {
                    LocalID = LocID,
                    FileID = 0x80038637,
                    ItemType = 0x16,
                    Data2 = 4,
                    DyeColor = 0,
                    Data4 = 0,
                    CanBeDyed = 0,
                    Flags = 0x2200C611,
                    MerchantPrice = 1337,
                    ItemID = 0x0000073D,
                    Quantity = 1,
                    NameHash = BitConverter.ToChar(new byte[] { 0x08, 0x01 }, 0).ToString()+BitConverter.ToChar(new byte[] { 0x07, 0x01 }, 0).ToString() +"Hacker's Earthwand" +BitConverter.ToChar(new byte[] { 0x01, 0x00 }, 0).ToString(),
                    NumStats = 0x05,
                    Stats = new UInt32[] {  0x24B80B00,
                                            0x26980003,
                                            0x22186409,
                                            0x23986409,
                                            0xA488C864}
                }
            };
            QueuingService.PostProcessingQueue.Enqueue(testItem);

            var testSpawn = new NetworkMessage(chara.Data.NetID)
            {
                PacketTemplate = new P021_SpawnObject.PacketSt21
                {
                    AgentID = LocID,
                    Data1 = LocID,
                    Data2 = 4,
                    Data3 = 0,
                    PosX = chara.Data.Position.X,
                    PosY = chara.Data.Position.Y,
                    Plane = (ushort)chara.Data.Position.PlaneZ,
                    Data4 = 1,
                    Rotation = 0,
                    Data5 = 1,
                    Speed = 0,
                    Data12 = 1,
                    Data13 = 0x34000000
                }
            };
            QueuingService.PostProcessingQueue.Enqueue(testSpawn);
        }
    }
}
