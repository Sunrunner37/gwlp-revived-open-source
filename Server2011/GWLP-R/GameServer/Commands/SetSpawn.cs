using System;
using System.Diagnostics;
using GameServer.Actions;
using GameServer.DataBase;
using GameServer.Enums;
using GameServer.Interfaces;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.NetworkManagement;

namespace GameServer.Commands
{
        [CommandAttribute(Description = "No parameters. Shows the available commands.")]
        public class SetSpawn : IAction
        {
                private int newCharID;

                public SetSpawn(int charID)
                {
                        newCharID = charID;
                }

                public void Execute(Map map)
                {
                        using (var db = (MySQL)DataBaseProvider.GetDataBase())
                        {
                                var pos = GameServerWorld.Instance.Get<DataCharacter>(Chars.CharID, newCharID).CharStats.Position;
                                var reNetID = (int)GameServerWorld.Instance.Get<DataCharacter>(Chars.CharID, newCharID)[Chars.NetID];

                                var spawn = new mapsSpawns()
                                {
                                        isOutpost = 1,
                                        isPvE = 1,
                                        spawnID = 0,
                                        spawnPlane = 0,
                                        spawnRadius = 0,
                                        spawnX = 0,
                                        spawnY = 0,
                                        teamSpawnNumber = 0,
                                        mapID = (int)map[Maps.MapID]
                                };

                                db.mapsSpawns.InsertOnSubmit(spawn);

                                var message = "Spawn added for this map. (default settings)";

                                try
                                {
                                        db.SubmitChanges();
                                }
                                catch (Exception e)
                                {
                                        Debug.WriteLine(e.Message);
                                        message = "Could not add spawn to database.";
                                }

                                // Note: CHAT MESSAGE
                                var chatMsg = new NetworkMessage(reNetID);
                                chatMsg.PacketTemplate = new P081_GeneralChatMessage.PacketSt81()
                                {
                                        Message =
                                        BitConverter.ToChar(new byte[] { 0x08, 0x01 }, 0).ToString() +
                                        BitConverter.ToChar(new byte[] { 0x07, 0x01 }, 0).ToString() +
                                        message +
                                        BitConverter.ToChar(new byte[] { 0x01, 0x00 }, 0).ToString()
                                };
                                QueuingService.PostProcessingQueue.Enqueue(chatMsg);

                                // Note: CHAT MESSAGE NO OWNER
                                var chatOwner = new NetworkMessage(reNetID);
                                chatOwner.PacketTemplate = new P082_GeneralChatNoOwner.PacketSt82()
                                {
                                        Data1 = 0,
                                        Data2 = (byte)ChatColors.DarkOrange_DarkOrange
                                };
                                QueuingService.PostProcessingQueue.Enqueue(chatOwner);
                        }
                }
        }
}
