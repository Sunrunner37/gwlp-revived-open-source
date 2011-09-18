using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Enums;
using GameServer.Interfaces;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine.ProcessorQueues;

namespace GameServer.Actions
{
        public class ChatMessage : IAction
        {
                private int newCharID;
                private string newMessage;

                public ChatMessage(int charID, string newMessage)
                {
                        newCharID = charID;
                        this.newMessage = newMessage;
                }

                public void Execute(Map map)
                {
                        var messageType = newMessage[0];
                        newMessage = newMessage.Substring(1);

                        // all ###################################
                        if (messageType == '!')
                        {
                                // send message to all available players
                                string prefix;
                                byte chatColor;
                                var chara = World.GetCharacter(Chars.CharID, newCharID);

                                prefix = chara.CharStats.ChatPrefix;
                                chatColor = chara.CharStats.ChatColor != (byte)ChatColors.Yellow_White ? chara.CharStats.ChatColor : (byte)ChatColors.Yellow_White;

                                foreach (var charID in map.CharIDs)
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

                        //// send message to all available players
                        //var mapID = World.GetCharacter(Idents.Chars.CharID, newCharID).MapID;
                        //foreach (var charID in World.GetMap(Idents.Maps.MapID, mapID).CharIDs)
                        //{

                        //}
                }

                private void CreateChatMessageFor(int charID, int recipientCharID, string message, string prefix ,byte color)
                {
                        var chara = World.GetCharacter(Chars.CharID, charID);

                        // get the recipient of all those packets
                        int reNetID = 0;
                        if (recipientCharID != charID)
                        {
                                reNetID = (int)World.GetCharacter(Chars.CharID, recipientCharID)[Chars.NetID];
                        }
                        else
                        {
                                reNetID = (int)chara[Chars.NetID];
                        }

                        var prefixString = prefix != "" ? "[" + prefix + "] " : "";

                        // Note: CHAT MESSAGE
                        var chatMsg = new NetworkMessage(reNetID);
                        chatMsg.PacketTemplate = new P081_GeneralChatMessage.PacketSt81()
                                                                {
                                                                        Message = 
                                                                        BitConverter.ToChar(new byte[]{0x08, 0x01}, 0).ToString() +
                                                                        BitConverter.ToChar(new byte[]{0x07, 0x01}, 0).ToString() +
                                                                        prefixString +
                                                                        message +
                                                                        BitConverter.ToChar(new byte[]{0x01, 0x00}, 0).ToString()
                                                                };
                        QueuingService.PostProcessingQueue.Enqueue(chatMsg);

                        // Note: CHAT MESSAGE OWNER
                        var chatOwner = new NetworkMessage(reNetID);
                        chatOwner.PacketTemplate = new P085_GeneralChatOwner.PacketSt85()
                        {
                                Data1 = (ushort)(int)chara[Chars.LocalID],
                                Data2 = color
                        };
                        QueuingService.PostProcessingQueue.Enqueue(chatOwner);
                }

                private static void ExecuteCommand(int charID, string message)
                {
                        var chara = World.GetCharacter(Chars.CharID, charID);

                        try
                        {
                                var command = message.Split(' ')[0];
                                var parameter = message.Split(' ').ToList();
                                parameter.RemoveAt(0);

                                bool isAvailable = false;
                                if (chara.CharStats.Commands.TryGetValue(command, out isAvailable) && isAvailable)
                                {
                                        Type commandType;
                                        World.ChatCommandsDict.TryGetValue(command, out commandType);

                                        Map map = World.GetMap(Maps.MapID, chara.MapID);
                                        
                                        var parameters = new List<object>(new object[] {charID});
                                        parameters.AddRange(parameter);

                                        map.ActionQueue.Enqueue(((IAction)Activator.CreateInstance(commandType, parameters.ToArray())).Execute);
                                }
                                
                        }
                        catch
                        {
                                var reNetID = (int)chara[Chars.NetID];

                                // Note: CHAT MESSAGE
                                var chatMsg = new NetworkMessage(reNetID);
                                chatMsg.PacketTemplate = new P081_GeneralChatMessage.PacketSt81()
                                {
                                        Message =
                                        BitConverter.ToChar(new byte[] { 0x08, 0x01 }, 0).ToString() +
                                        BitConverter.ToChar(new byte[] { 0x07, 0x01 }, 0).ToString() +
                                        "Error in command." +
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
