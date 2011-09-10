using System;
using GameServer.Actions;
using GameServer.Enums;
using System.Linq;
using GameServer.Interfaces;
using GameServer.ServerData;

namespace GameServer.Modules
{
        public class Movement : IModule
        {
                // add a constructor that loads all pmaps to an array
                // check for collision if needed

                private DateTime lastCheck;

                public void Execute()
                {
                        if (DateTime.Now.Subtract(lastCheck).Milliseconds > 10)
                        {
                                World.GetMaps().AsParallel().ForAll(ProcessMovePackets);

                                lastCheck = DateTime.Now;
                        }
                }

                private static void ProcessMovePackets(Map map)
                {
                        foreach (int charID in map.CharIDs)
                        {
                                Character chara;
                                lock (chara = World.GetCharacter(Chars.CharID, charID))
                                {
                                        switch (chara.CharStats.MoveState)
                                        {
                                                case MovementState.MovingUnhandled:
                                                        // do some collision calc
                                                        {
                                                                chara.CharStats.Direction = chara.CharStats.Direction.UnitVector;
                                                                var aim = chara.CharStats.Position + (chara.CharStats.Direction * 512);
                                                                var action = new MovePlayer((int)chara[Chars.CharID], aim);
                                                                map.ActionQueue.Enqueue(action.Execute);
                                                                // update movestate
                                                                chara.CharStats.MoveState = MovementState.MovingHandled;
                                                        }
                                                        break;
                                                case MovementState.NotMovingUnhandled:
                                                        // do some collision calc
                                                        {
                                                                var action = new MovePlayer((int)chara[Chars.CharID], chara.CharStats.Position);
                                                                map.ActionQueue.Enqueue(action.Execute);
                                                                // update movestate
                                                                chara.CharStats.MoveState = MovementState.MovingHandled;
                                                        }
                                                        break;
                                        }
                                }
                        }
                }
        }
}
