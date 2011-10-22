using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer.Enums
{
        public enum AgentEquipment
        {
                Leadhand = 0,
                Offhand = 1,
                Chest = 2,
                Feet = 5,
                Legs = 3,
                Arms = 6,
                Head = 4,
                Costume = 7, // ??
                CostumeHead = 8,

                // these are selfdefined (9 saved for... who knows what anet comes up with next =P)
                Backpack = 10,
                Beltpouch = 11,
                Bag1 = 12,
                Bag2 = 13,
                EquipmentPack = 14
        }
}