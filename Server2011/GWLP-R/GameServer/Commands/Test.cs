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
using GameServer.ServerData.Items;
using GameServer.Actions;

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

                        ShowDialog dialog = new ShowDialog("", 0, chara.Data.NetID);
                        dialog.AddButton(ButtonIcons.Assassin, "Assassin", 5);
                        dialog.AddButton(ButtonIcons.Mesmer, "Mesmer", 5);
                        dialog.AddButton(ButtonIcons.Necromancer, "Necromancer", 5);
                        dialog.AddButton(ButtonIcons.Decline1, "Decline1", 5);
                        dialog.AddButton(ButtonIcons.Elementalist, "Elementalist", 5);
                        dialog.AddButton(ButtonIcons.Monk, "Monk", 5);
                        dialog.AddButton(ButtonIcons.Warrior, "Warrior", 5);
                        dialog.AddButton(ButtonIcons.Ranger, "Ranger", 5);
                        dialog.AddButton(ButtonIcons.Dervish, "Dervish", 5);
                        dialog.AddButton(ButtonIcons.Ritualist, "Ritualist", 5);
                        dialog.AddButton(ButtonIcons.Paragon, "Paragon", 5);
                        dialog.AddButton(ButtonIcons.NextPage, "NextPage", 5);
                        dialog.AddButton(ButtonIcons.BlueButtonStyle, "BlueButtonStyle", 5);
                        dialog.AddButton(ButtonIcons.Accept1, "Accept1", 5);
                        dialog.AddButton(ButtonIcons.Decline2, "Decline2", 5);
                        dialog.AddButton(ButtonIcons.Cancel, "Cancel", 5);
                        dialog.AddButton(ButtonIcons.Accept2, "Accept2", 5);
                        dialog.AddButton(ButtonIcons.Decline3, "Decline3", 5);
                        dialog.AddButton(ButtonIcons.QuestMark, "QuestMark", 5);
                        dialog.AddButton(ButtonIcons.Flag, "Flag", 5);
                        dialog.AddButton(ButtonIcons.Dungeon, "Dungeon", 5);
                        dialog.AddButton(ButtonIcons.GreenBubble, "GreenBubble", 5);
                        dialog.AddButton(ButtonIcons.QuestionMark, "QuestionMark", 5);
                        dialog.AddButton(ButtonIcons.Bag, "Bag", 5);
                        dialog.AddButton(ButtonIcons.Accept3, "Accept3", 5);
                        dialog.AddButton(ButtonIcons.Action, "Action", 5);
                        dialog.AddButton(ButtonIcons.Link, "Link", 5);
                        map.Data.ActionQueue.Enqueue(dialog.Execute);

                }
        }
}
