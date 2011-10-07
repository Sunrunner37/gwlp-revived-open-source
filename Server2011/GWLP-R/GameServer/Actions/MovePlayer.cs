using System.Linq;
using GameServer.Enums;
using GameServer.Interfaces;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.GuildWars.DataWrappers.Clients;
using ServerEngine.NetworkManagement;
using ServerEngine.GuildWars.Tools;

namespace GameServer.Actions
{
        public class MovePlayer : IAction
        {
                private readonly CharID newCharID;
                private static GWVector newAim;

                public MovePlayer(CharID charID, GWVector aim)
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

                        var moveType = chara.Data.MoveType;

                        if (moveType == MovementType.Forward || moveType == MovementType.DiagFwLeft || moveType == MovementType.DiagFwRight || moveType == MovementType.Stop)
                        {
                                chara.Data.SpeedModifier = 1F;
                        }
                        else if (moveType == MovementType.Backward || moveType == MovementType.DiagBwLeft || moveType == MovementType.DiagBwRight)
                        {
                                chara.Data.SpeedModifier = 0.66F;
                        }
                        else if (moveType == MovementType.SideLeft || moveType == MovementType.SideRight)
                        {
                                chara.Data.SpeedModifier = 0.75F;
                        }
                        else if (moveType == MovementType.Collision)
                        {
                                // dont change speed modifier, as it is dynamically set.

                                // reset movetype, cause 10 is non existant for the client
                                moveType = MovementType.Forward;
                        }

                        if (moveType != MovementType.Stop && chara.Data.MoveState == MovementState.MoveChangeDir)
                        {
                                // Note: KEYBOARD MOVE START
                                var moveStart = new NetworkMessage(reNetID)
                                {
                                        PacketTemplate = new P026_MovementDirection.PacketSt26
                                        {
                                                AgentID = (ushort)chara.Data.AgentID.Value,
                                                DirX = chara.Data.Direction.X,
                                                DirY = chara.Data.Direction.Y,
                                                MoveType = (byte)moveType
                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(moveStart);

                                // reset the moving state here
                                chara.Data.MoveState = MovementState.MoveKeepDir;
                        }

                        // Note: MOVE SPEED MODIFIER
                        var moveSpeed = new NetworkMessage(reNetID)
                        {
                                PacketTemplate = new P032_MovementSpeedModifier.PacketSt32
                                {
                                        AgentID = (ushort)chara.Data.AgentID.Value,
                                        Speed = chara.Data.SpeedModifier,
                                        MoveType = (byte)moveType
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(moveSpeed);

                        // Note: MOVEMENT AIM
                        var gotoLoc = new NetworkMessage(reNetID)
                        {
                                PacketTemplate = new P030_MovementAim.PacketSt30
                                {
                                        AgentID = (ushort)chara.Data.AgentID.Value,
                                        X = newAim.X,
                                        Y = newAim.Y,
                                        PlaneZ = (ushort)newAim.PlaneZ,
                                        Data1 = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(gotoLoc);
                        
                }
        }
}
