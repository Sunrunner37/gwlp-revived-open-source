using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.Enums;
using GameServer.Interfaces;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine.ProcessorQueues;
using ServerEngine.Tools;

namespace GameServer.Actions
{
        public class MovePlayer : IAction
        {
                private int newCharID;
                private static GWVector newAim;

                public MovePlayer(int charID, GWVector aim)
                {
                        newCharID = charID;
                        newAim = aim;
                }

                public void Execute(Map map)
                {
                        // send message to all available players
                        foreach (var charID in map.CharIDs)
                        {
                                CreatePackets(newCharID, charID);
                        }
                }

                private static void CreatePackets(int charID, int recipientCharID)
                {
                        Character chara;
                        lock (chara = World.GetCharacter(Chars.CharID, charID))
                        {
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

                                var moveType = (byte) chara.CharStats.MoveType;
                                float speed = 1F;

                                if (moveType != (byte)MovementType.Stop)
                                {
                                        // Note: KEYBOARD MOVE START
                                        var moveStart = new NetworkMessage(reNetID);
                                        moveStart.PacketTemplate = new P026_KeyboardMoveStart.PacketSt26()
                                        {
                                                AgentID = (ushort)(int)chara[Chars.AgentID],
                                                DirX = chara.CharStats.Direction.X,
                                                DirY = chara.CharStats.Direction.Y,
                                                MoveType = moveType
                                        };
                                        QueuingService.PostProcessingQueue.Enqueue(moveStart);
                                }


                                if (moveType == (byte)MovementType.Backward || moveType == (byte)MovementType.DiagBwLeft ||
                                moveType == (byte)MovementType.DiagBwRight)
                                {
                                        speed = 0.66F;
                                }
                                else if (moveType == (byte)MovementType.SideLeft || moveType == (byte)MovementType.SideRight)
                                {
                                        speed = 0.75F;
                                }

                                // Note: KEYBOARD MOVE SPEED
                                var moveSpeed = new NetworkMessage(reNetID);
                                moveSpeed.PacketTemplate = new P032_KeyboardMoveSpeed.PacketSt32()
                                {
                                        AgentID = (ushort)(int)chara[Chars.AgentID],
                                        Speed = speed,
                                        MoveType = moveType
                                };
                                QueuingService.PostProcessingQueue.Enqueue(moveSpeed);

                                // Note: GOTO LOCATION
                                var gotoLoc = new NetworkMessage(reNetID);
                                gotoLoc.PacketTemplate = new P030_GotoLocation.PacketSt30()
                                {
                                        AgentID = (ushort)(int)chara[Chars.AgentID],
                                        X = newAim.X,
                                        Y = newAim.Y,
                                        PlaneZ = (ushort)newAim.PlaneZ,
                                        Data1 = 0
                                };
                                QueuingService.PostProcessingQueue.Enqueue(gotoLoc);
                        }
                }
        }
}
