using System;
using GameServer.Enums;
using GameServer.Interfaces;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.GuildWars.DataWrappers.Clients;
using ServerEngine.NetworkManagement;

namespace GameServer.Commands
{
        [CommandAttribute(Description = "No parameters. Shows some server info.")]
        public class ServerInfo : IAction
        {
                private readonly CharID newCharID;

                public ServerInfo(CharID charID)
                {
                        newCharID = charID;
                }

                public void Execute(DataMap map)
                {
                        // get some info to display (srvInfo is of type string[] btw)
                        var chara = map.Get<DataCharacter>(newCharID);
                        var srvInfo = GameServerWorld.Instance.ServerInfo();

                        // iterate trough the lines, show each line as one chat message (with orange color)
                        foreach (var info in srvInfo)
                        {
                                // Note: CHAT MESSAGE
                                var chatMsg = new NetworkMessage(chara.Data.NetID)
                                {
                                        PacketTemplate = new P081_GeneralChatMessage.PacketSt81
                                        {
                                                Message = "Ĉć" +
                                                        info +
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
