using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Enums;
using GameServer.Interfaces;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.GuildWars.DataWrappers.Clients;
using ServerEngine.NetworkManagement;

namespace GameServer.Actions
{
        public class ChatMessage : IAction
        {
                private readonly CharID newCharID;
                private string newMessage;

                public ChatMessage(CharID charID, string newMessage)
                {
                        newCharID = charID;
                        this.newMessage = newMessage;
                }

                public void Execute(DataMap map)
                {
                        var messageType = newMessage[0];
                        newMessage = newMessage.Substring(1);

                        // all #############################################
                        if (messageType == '!')
                        {
                                // send message to all available players
                                var chara = GameServerWorld.Instance.Get<DataClient>(newCharID).Character;

                                var prefix = chara.Data.ShowPrefix ? chara.Data.ChatPrefix : "";
                                var chatColor = chara.Data.ShowColor ? chara.Data.ChatColor : (byte)ChatColors.Yellow_White;

                                // the following linq expression returns an IEnumerable<CharID> of all characters on that map
                                foreach (var charID in map.GetAll<DataCharacter>().Select(x => x.Data.CharID))
                                {
                                        CreateChatMessageFor(newCharID, charID, newMessage, prefix, chatColor);
                                }
                        }

                        // team #############################################
                        if (messageType == '#')
                        {
                        }

                        // trade ############################################
                        if (messageType == '$')
                        {
                        }

                        // guild ############################################
                        if (messageType == '@')
                        {
                        }

                        // alliance #########################################
                        if (messageType == '%')
                        {
                        }

                        // whsp #############################################
                        if (messageType == '"')
                        {
                        }

                        // cmd ##############################################
                        if (messageType == '/')
                        {
                                ExecuteCommand(newCharID, newMessage);
                        }
                }

                private static void CreateChatMessageFor(CharID senderCharID, CharID recipientCharID, string message, string prefix ,byte color)
                {
                        var chara = GameServerWorld.Instance.Get<DataClient>(senderCharID).Character;

                        // get the recipient of all those packets
                        var reNetID = recipientCharID.Value != senderCharID.Value ?
                                GameServerWorld.Instance.Get<DataClient>(recipientCharID).Data.NetID :
                                chara.Data.NetID;

                        var prefixString = prefix != "" ? "[" + prefix + "] " : "";

                        // Note: CHAT MESSAGE
                        var chatMsg = new NetworkMessage(reNetID)
                        {
                                PacketTemplate = new P081_GeneralChatMessage.PacketSt81
                                {
                                        Message = "Ĉć" +
                                                prefixString +
                                                message +
                                                BitConverter.ToChar(new byte[]{ 0x01, 0x00 }, 0)
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(chatMsg);

                        // Note: CHAT MESSAGE OWNER
                        var chatOwner = new NetworkMessage(reNetID)
                        {
                                PacketTemplate = new P085_GeneralChatOwner.PacketSt85
                                {
                                        Data1 = (ushort)chara.Data.LocalID.Value,
                                        Data2 = color
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(chatOwner);
                }

                private static void ExecuteCommand(CharID charID, string message)
                {
                        var chara = GameServerWorld.Instance.Get<DataClient>(charID).Character;

                        try
                        {
                                var command = message.Split(' ')[0];
                                var parameter = message.Split(' ').ToList();
                                parameter.RemoveAt(0);

                                if (chara.Data.ChatCommands[command])
                                {
                                        Type commandType;
                                        GameServerWorld.Instance.ChatCommandsDict.TryGetValue(command, out commandType);

                                        var map = GameServerWorld.Instance.Get<DataMap>(chara.Data.MapID);
                                        
                                        var parameters = new List<object>(new object[] {charID});
                                        parameters.AddRange(parameter);

                                        map.Data.ActionQueue.Enqueue(
                                                ((IAction)Activator.CreateInstance(
                                                commandType, 
                                                parameters.ToArray())).Execute);
                                }
                                else
                                {
                                        throw new UnauthorizedAccessException();
                                }
                        }
                        catch (Exception e)
                        {
                                var reNetID = chara.Data.NetID;

                                var errorMsg = "Error in command. " + e.Message;
                                if (e.GetType() == typeof(KeyNotFoundException))
                                {
                                        errorMsg = "Command not found.";
                                }
                                else if (e.GetType() == typeof(UnauthorizedAccessException))
                                {
                                        errorMsg = "Access denied.";
                                }

                                // Note: CHAT MESSAGE
                                var chatMsg = new NetworkMessage(reNetID)
                                {
                                        PacketTemplate = new P081_GeneralChatMessage.PacketSt81
                                        {
                                                Message = "Ĉć" +
                                                errorMsg +
                                                BitConverter.ToChar(new byte[] { 0x01, 0x00 }, 0)
                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(chatMsg);

                                // Note: CHAT MESSAGE NO OWNER
                                var chatOwner = new NetworkMessage(reNetID)
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
