using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GameServer.Enums;
using GameServer.Interfaces;
using GameServer.Modules;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.DataManagement.DataWrappers;
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

                        Movement.PathingMap pmap;
                        if (!Movement.maps.TryGetValue((int) map.Data.MapID.Value, out pmap)) return;
                        Console.WriteLine("Trying to spawn stuff");

                        foreach (var trapezoid in pmap.Trapezoids)
                        {
                                if ((trapezoid.BottomLeft - chara.Data.Position).Length < 1000) spawny(trapezoid.BottomLeft, map);
                                if ((trapezoid.TopLeft - chara.Data.Position).Length < 1000) spawny(trapezoid.TopLeft, map);
                                if ((trapezoid.BottomRight - chara.Data.Position).Length < 1000) spawny(trapezoid.BottomRight, map);
                                if ((trapezoid.TopRight - chara.Data.Position).Length < 1000) spawny(trapezoid.TopRight, map);
                        }
                }

                private static void spawny(GWVector pos, DataMap map)
                {
                        var coins = Item.CreateItemStubFromDB(10, map.Data.ItemLocalIDs.RequestID());

                        coins.Data.Flags = 537395201;
                        coins.Data.MerchantPrice = 1;
                        coins.Data.Quantity = 1;
                        coins.Data.ItemAgentID = map.Data.AgentIDs.RequestID();
                        coins.Data.Position = pos.Clone();
                        map.Data.MapItems.Add(coins.Data.ItemAgentID, coins);

                        foreach (var charID in map.GetAll<DataCharacter>().Select(x => x.Data.CharID))
                        {
                                var reNetID = GameServerWorld.Instance.Get<DataClient>(charID).Data.NetID;
                                coins.SendGeneral(reNetID);
                                coins.SendSpawn(reNetID);
                        }
                }
        }
}
