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

                        Item leadWeapon;
                        if (!chara.Data.Items.Equipment.TryGetValue(AgentEquipment.Leadhand, out leadWeapon)) return;

                        Item myItem = leadWeapon.Clone();
                        myItem.Data.ItemLocalID = map.Data.ItemLocalIDs.RequestID();
                        myItem.Data.ItemAgentID = map.Data.AgentIDs.RequestID();
                        myItem.Data.Position = chara.Data.Position.Clone();
                        map.Data.MapItems.Add(myItem.Data.ItemAgentID, myItem);

                        foreach (var charID in map.GetAll<DataCharacter>().Select(x => x.Data.CharID))
                        {
                                var reNetID = GameServerWorld.Instance.Get<DataClient>(charID).Data.NetID;

                                myItem.SendGeneral(reNetID);
                                myItem.SendSpawn(reNetID);
                        }
                }
        }
}
