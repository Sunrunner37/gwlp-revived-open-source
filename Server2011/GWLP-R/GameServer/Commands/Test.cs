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
                        myItem.Data.ItemLocalID = map.Data.LocalIDs.RequestID()+100;
                        map.Data.MapItems.Add(myItem.Data.ItemLocalID, myItem);

                        foreach (var charID in map.GetAll<DataCharacter>().Select(x => x.Data.CharID))
                        {
                                myItem.SendGeneral(GameServerWorld.Instance.Get<DataClient>(charID).Data.NetID);
                        }

                        var testSpawn = new NetworkMessage(chara.Data.NetID)
                        {
                                PacketTemplate = new P021_SpawnObject.PacketSt21
                                {
                                        AgentID = (uint)myItem.Data.ItemLocalID,
                                        Data1 = (uint)myItem.Data.ItemLocalID,
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
