using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer.Enums
{
        public enum EquipmentDisplayStatus
        {
                HideAlways = 0,
                HideInTownsAndOutpost = 1,
                HideInCombatAreasExceptPvp = 2,
                ShowAlways = 3
        }

        public enum EquipmentDisplayPart
        {
            Cape = 1,
            Headgear = 4,
            Costume = 16,
            CostumeHead = 64
        }
}