using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.Enums;
using ServerEngine.Tools;

namespace GameServer.ServerData
{
        public class NonPlayerCharStats
        {
                public NonPlayerCharStats()
                {
                        Position = new GWVector(0, 0, 0);
                        Direction = new GWVector(0, 0, 0);
                }

                public int NpcID { get; set; } // localID

                public int FileID { get; set; }
                public byte[] ModelHash { get; set; }
                public byte[] Appearance { get; set; }
                public int Scale { get; set; }

                public bool HasNameHash { get; set; }
                public byte[] NameHash { get; set; } // this is actually local name

                public int ProfessionFlags { get; set; }
                public int Profession { get; set; }

                public int Level { get; set; }

                //public byte[] SkillBar { get; set; }
                //public byte[] UnlockedSkills { get; set; }

                public int Energy { get; set; }
                public float EnergyRegen { get; set; }
                public int Health { get; set; }
                public float HealthRegen { get; set; }

                public int Morale { get; set; }

                public int VitalStats { get; set; }

                public GWVector Position { get; set; }
                public GWVector Direction { get; set; }
                public MovementState MoveState { get; set; }
                public MovementType MoveType { get; set; }

                public float Speed { get; set; }
                public float Rotation { get; set; }
                public bool IsRotating { get; set; }
        }
}
