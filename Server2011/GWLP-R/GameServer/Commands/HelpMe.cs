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
        [CommandAttribute(Description = "No parameters. Shows the available commands.")]
        public class HelpMe : IAction
        {
                private readonly CharID newCharID;

                public HelpMe(CharID charID)
                {
                        newCharID = charID;
                }

                public void Execute(DataMap map)
                {
                        foreach (var cmd in GameServerWorld.Instance.ChatCommandsDict.Values)
                        {
                                var attributes = cmd.GetCustomAttributes(typeof(CommandAttribute), false);
                                
                                // failcheck
                                if (attributes.Length != 1) continue;

                                var name = "[/" + cmd.Name + "]: ";
                                var message = ((CommandAttribute)attributes[0]).Description;

                                var chara = map.Get<DataCharacter>(newCharID);       

                                // Note: CHAT MESSAGE
                                var chatMsg = new NetworkMessage(chara.Data.NetID)
                                {
                                        PacketTemplate = new P081_GeneralChatMessage.PacketSt81
                                        {
                                                Message = "Ĉć" +
                                                        name +
                                                        message +
                                                        BitConverter.ToChar(new byte[] { 0x01, 0x00 }, 0).ToString()
                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(chatMsg);

                                // Note: CHAT MESSAGE NO OWNER
                                var chatOwner = new NetworkMessage(chara.Data.NetID)
                                {
                                        PacketTemplate = new P082_GeneralChatNoOwner.PacketSt82()
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
