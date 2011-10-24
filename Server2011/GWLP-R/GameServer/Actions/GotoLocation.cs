using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.Enums;
using GameServer.Interfaces;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.GuildWars.DataWrappers.Clients;
using ServerEngine.GuildWars.Tools;
using ServerEngine.NetworkManagement;

namespace GameServer.Actions
{
        class GotoLocation : IAction
        {
                private readonly CharID newCharID;
                private static GWVector newAim;

                public GotoLocation(CharID charID, GWVector aim)
                {
                        newCharID = charID;
                        newAim = aim;
                }

                public void Execute(DataMap map)
                {
                        // send message to all available players
                        // the following linq expression returns an IEnumerable<CharID> of all characters on that map
                        foreach (var charID in map.GetAll<DataCharacter>().Select(x => x.Data.CharID))
                        {
                                CreatePackets(newCharID, charID);
                        }
                }

                private static void CreatePackets(CharID senderCharID, CharID recipientCharID)
                {
                        var chara = GameServerWorld.Instance.Get<DataClient>(senderCharID).Character;

                        // get the recipient of all those packets
                        var reNetID = recipientCharID.Value != senderCharID.Value ?
                                GameServerWorld.Instance.Get<DataClient>(recipientCharID).Data.NetID :
                                chara.Data.NetID;

                        // Note: MOVEMENT AIM
                        var gotoLoc = new NetworkMessage(reNetID)
                        {
                                PacketTemplate = new P031_MovementGotoLocation.PacketSt31
                                {
                                        AgentID = (ushort)chara.Data.AgentID.Value,
                                        PosX = newAim.X,
                                        PosY = newAim.Y,
                                        Plane = (ushort)newAim.PlaneZ,
                                        Data1 = 0,
                                        Data2 = 0x9C
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(gotoLoc);
                        
                }
        }
}
