using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer.Enums
{
        public enum MovementState
        {
                NotMovingHandled,
                MovingUnhandled,
                MovingHandled,
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
                FwCollision = 10,
                DgCollision = 11,
        }
}
