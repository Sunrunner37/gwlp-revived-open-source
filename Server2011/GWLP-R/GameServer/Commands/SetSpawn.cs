using System;
using System.Diagnostics;
using GameServer.Enums;
using GameServer.Interfaces;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.GuildWars.DataBase;
using ServerEngine.GuildWars.DataWrappers.Clients;
using ServerEngine.NetworkManagement;

namespace GameServer.Commands
{
        [CommandAttribute(Description = "No parameters. Creates a new map-spawn entry in the database.")]
        public class SetSpawn : IAction
        {
                private CharID newCharID;

                public SetSpawn(CharID charID)
                {
                        newCharID = charID;
                }

                public void Execute(DataMap map)
                {
                        using (var db = (MySQL)DataBaseProvider.GetDataBase())
                        {
                                var chara = map.Get<DataCharacter>(newCharID);

                                var spawn = new mapsSpawns
                                {
                                        isOutpost = (sbyte)(map.Data.IsOutpost ? 1 : 0),
                                        isPvE = (sbyte)(map.Data.IsPvE ? 1 : 0),
                                        spawnID = 0,
                                        spawnPlane = chara.Data.Position.PlaneZ,
                                        spawnRadius = 0,
                                        spawnX = chara.Data.Position.X,
                                        spawnY = chara.Data.Position.Y,
                                        teamSpawnNumber = 0,
                                        mapID = (int)map.Data.MapID.Value
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
                                var chatMsg = new NetworkMessage(chara.Data.NetID)
                                {
                                        PacketTemplate = new P081_GeneralChatMessage.PacketSt81()
                                        {
                                                Message ="Ĉć" + 
                                                        message +
                                                        BitConverter.ToChar(new byte[] { 0x01, 0x00 }, 0)
                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(chatMsg);

                                // Note: CHAT MESSAGE NO OWNER
                                var chatOwner = new NetworkMessage(chara.Data.NetID)
                                {
                                        PacketTemplate = new P082_GeneralChatNoOwner.PacketSt82
                                        {
                                                Data1 = 0,
                                                Data2 = (byte)ChatColors.DarkOrange_DarkOrange
                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(chatOwner);
                        }
                }
        }
}
