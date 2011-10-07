using System.Linq;
using GameServer.Enums;
using GameServer.Interfaces;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.GuildWars.DataWrappers.Clients;
using ServerEngine.NetworkManagement;

namespace GameServer.Commands
{
        [CommandAttribute(Description = "No Parameters. Updates the position of all players on the map (including yourself).")]
        class GWStuck : IAction
        {
                private readonly CharID newCharID;

                public GWStuck(CharID charID)
                {
                        newCharID = charID;
                }

                public void Execute(DataMap map)
                {
                        // get the net id of the recipient
                        var netID = map.Get<DataCharacter>(newCharID).Data.NetID;

                        // the following linq expression returns an IEnumerable<CharID> of all characters on that map
                        foreach (var chara in map.GetAll<DataCharacter>())
                        {
                                // Note: FREEZE PLAYER
                                var freeze = new NetworkMessage(netID)
                                {
                                        PacketTemplate = new P147_UpdateGenericValueInt.PacketSt147
                                        {
                                                ID1 = chara.Data.AgentID.Value,
                                                ValueID = (uint)GenericValues.FreezePlayer,
                                                Value = 1
                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(freeze);

                                // Note: INVALIDATE AGENT MODEL
                                var imod = new NetworkMessage(netID)
                                {
                                        PacketTemplate = new P022_FIXMEInvalidateAgentModel.PacketSt22
                                        {
                                                AgentID = chara.Data.AgentID.Value,
                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(imod);

                                // Note: UPDATE AGENT POSITION
                                var upos = new NetworkMessage(netID)
                                {
                                        PacketTemplate = new P033_UpdateAgentPosition.PacketSt33
                                        {
                                                AgentID = chara.Data.AgentID.Value,
                                                PosX = chara.Data.Position.X,
                                                PosY = chara.Data.Position.Y,
                                                Plane = (ushort)chara.Data.Position.PlaneZ
                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(upos);

                                // Note: UNFREEZE PLAYER
                                var unFreeze = new NetworkMessage(netID)
                                {
                                        PacketTemplate = new P147_UpdateGenericValueInt.PacketSt147
                                        {
                                                ID1 = chara.Data.AgentID.Value,
                                                ValueID = (uint)GenericValues.FreezePlayer,
                                                Value = 0
                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(unFreeze);
                        }
                }
        }
}
