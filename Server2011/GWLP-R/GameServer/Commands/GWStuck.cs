using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.Enums;
using GameServer.Interfaces;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine.ProcessorQueues;

namespace GameServer.Commands
{
        [CommandAttribute(Description = "No Parameters. Updates the position of all players on the map (including yourself).")]
        class GWStuck : IAction
        {
                private int newCharID;

                public GWStuck(int charID)
                {
                        newCharID = charID;
                }

                public void Execute(Map map)
                {
                        // get the net id of the recipient
                        var netID = (int)World.GetCharacter(Chars.CharID, newCharID)[Chars.NetID];

                        foreach (var charID in map.CharIDs)
                        {
                                // get the char stuff of the agent we are updating
                                var chara = World.GetCharacter(Chars.CharID, charID);
                                var agentID = (uint)(int) chara[Chars.AgentID];

                                // Note: FREEZE PLAYER
                                var freeze = new NetworkMessage(netID);
                                freeze.PacketTemplate = new P147_UpdateGenericValueInt.PacketSt147()
                                {
                                        ID1 = agentID,
                                        ValueID = (uint)GenericValues.FreezePlayer,
                                        Value = 1
                                };
                                QueuingService.PostProcessingQueue.Enqueue(freeze);

                                // Note: INVALIDATE AGENT MODEL
                                var imod = new NetworkMessage(netID);
                                imod.PacketTemplate = new P022_FIXMEInvalidateAgentModel.PacketSt22()
                                {
                                        AgentID = agentID,
                                };
                                QueuingService.PostProcessingQueue.Enqueue(imod);

                                // Note: UPDATE AGENT POSITION
                                var upos = new NetworkMessage(netID);
                                upos.PacketTemplate = new P033_UpdateAgentPosition.PacketSt33()
                                {
                                        AgentID = agentID,
                                        PosX = chara.CharStats.Position.X,
                                        PosY = chara.CharStats.Position.Y,
                                        Plane = (ushort)chara.CharStats.Position.PlaneZ
                                };
                                QueuingService.PostProcessingQueue.Enqueue(upos);

                                // Note: UNFREEZE PLAYER
                                var unFreeze = new NetworkMessage(netID);
                                unFreeze.PacketTemplate = new P147_UpdateGenericValueInt.PacketSt147()
                                {
                                        ID1 = agentID,
                                        ValueID = (uint)GenericValues.FreezePlayer,
                                        Value = 0
                                };
                                QueuingService.PostProcessingQueue.Enqueue(unFreeze);
                        }
                }
        }
}
