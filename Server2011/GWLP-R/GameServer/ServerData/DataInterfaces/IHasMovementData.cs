using System;
using GameServer.Enums;
using ServerEngine.GuildWars.Tools;

namespace GameServer.ServerData.DataInterfaces
{
        public interface IHasMovementData
        {
                GWVector Position { get; set; }
                GWVector Direction { get; set; }

                uint TrapezoidIndex { get; set; }
                DateTime LastMovement { get; set; }
                bool AtBorder { get; set; }

                MovementState MoveState { get; set; }
                MovementType MoveType { get; set; }

                float Speed { get; set; }
                float SpeedModifier { get; set; }
                
                float Rotation { get; set; }
                bool IsRotating { get; set; }
        }
}