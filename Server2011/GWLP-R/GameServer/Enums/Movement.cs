using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer.Enums
{
        public enum MovementState
        {
                NotMoving,
                MoveChangeDir,
                MoveKeepDir,
                NotMovingUnhandled
        }

        public enum MovementType
        {
                Forward = 1,
                DiagFwLeft = 2,
                DiagFwRight = 3,
                Backward = 4,
                DiagBwLeft = 5,
                DiagBwRight = 6,
                SideLeft = 7,
                SideRight = 8,
                Stop = 9,
                Collision = 10,
        }
}
