using System.Linq;
using GameServer.Interfaces;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.GuildWars.DataWrappers.Clients;
using ServerEngine.NetworkManagement;
using ServerEngine.DataManagement.DataWrappers;
using GameServer.Enums;
using System.Collections.Generic;
using ServerEngine.GuildWars.Tools;
using System;

namespace GameServer.Actions
{
        public class ShowDialog : IAction
        {
                private string body;
                private List<DialogButton> buttons = new List<DialogButton>();
                private readonly uint senderAgentID;
                private readonly NetID receiverNetID;

                public ShowDialog(string body, uint senderAgentID, NetID receiverNetID)
                {
                        this.body = body;
                        this.senderAgentID = senderAgentID;
                        this.receiverNetID = receiverNetID;
                }

                public void AddLine(string caption)
                {
                        body = body + "<brx>" + caption;
                }

                public void AddDialogButton(string caption, uint buttonID)
                {
                        body = body + "<a=" + buttonID.ToString() + ">" + caption + "</a>";
                }

                public void AddButton(ButtonIcons icon, string caption, uint buttonID)
                {
                        buttons.Add(new DialogButton(icon, caption, buttonID));
                }

                public void Execute(DataMap map)
                {
                        string remainder = body.ToGW();
                        int length = remainder.Length;
                        int stubLength;

                        while (length > 0)
                        {
                                stubLength = Math.Min(61, length);
                                CreateBodyPacket(remainder.Substring(0, stubLength));
                                remainder = remainder.Substring(stubLength);
                                length = remainder.Length;
                        }

                        var senderPacket = new NetworkMessage(receiverNetID)
                        {
                                PacketTemplate = new P117_DialogSender.PacketSt117
                                {
                                        AgentID = senderAgentID
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(senderPacket);

                        foreach (var button in buttons)
                        {
                                CreateButtonPacket(button);
                        }
                }

                private void CreateBodyPacket(string body)
                {
                        var bodyPacket = new NetworkMessage(receiverNetID)
                        {
                                PacketTemplate = new P116_DialogBody.PacketSt116
                                {
                                        DialogData = body
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(bodyPacket);
                }

                private void CreateButtonPacket(DialogButton button)
                {
                        var buttonPacket = new NetworkMessage(receiverNetID)
                        {
                                PacketTemplate = new P114_DialogButton.PacketSt114
                                {
                                        Icon = (byte)button.icon,
                                        ButtonId = button.buttonID,
                                        Text = GWStringExtensions.ToGW(button.caption),
                                        Flag = 0xFFFFFFFF
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(buttonPacket);
                }

                internal class DialogButton
                {
                        public readonly ButtonIcons icon;
                        public readonly string caption;
                        public readonly uint buttonID;

                        public DialogButton(ButtonIcons icon, string caption, uint buttonID)
                        {
                                this.icon = icon;
                                this.caption = caption;
                                this.buttonID = buttonID;
                        }
                }
        }
}
