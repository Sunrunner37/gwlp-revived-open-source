using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.Enums;
using ServerEngine.Tools;

namespace GameServer.ServerData
{
        public class CharacterStats
        {
                public CharacterStats()
                {
                        Position = new GWVector(0, 0, 0);
                        LastCollisionPosition = Position.Clone();
                        Direction = new GWVector(0, 0, 0);
                        SpeedModifier = 1F;
                        Commands = new Dictionary<string, bool>();
                }

                public Dictionary<string, bool> Commands { get; set; }

                public string ChatPrefix { get; set; }
                public byte ChatColor { get; set; }

                public int AttPtsFree { get; set; }
                public int AttPtsTotal { get; set; }
                public int SkillPtsFree { get; set; }
                public int SkillPtsTotal { get; set; }

                public byte[] Appearance { get; set; }

                public byte ProfessionPrimary { get; set; }
                public byte ProfessionSecondary { get; set; }

                public int Level { get; set; }

                public byte[] SkillBar { get; set; }
                public byte[] UnlockedSkills { get; set; }

                public int Energy { get; set; }
                public float EnergyRegen { get; set; }
                public int Health { get; set; }
                public float HealthRegen { get; set; }

                public int Morale { get; set; }

                public int VitalStats { get; set; }

                public GWVector Position { get; set; }
                public GWVector LastCollisionPosition { get; set; }
                public bool AtBorder { get; set; }
                /// <summary>
                ///   Only used with module: Movement
                /// </summary>
                public uint TrapezoidIndex { get; set; }
                public GWVector Direction { get; set; }
                public MovementState MoveState { get; set; }
                public int MoveType { get; set; }

                public float Speed { get; set; }
                /// <summary>
                ///  Used with movement
                /// </summary>
                public float SpeedModifier { get; set; }
                public float Rotation { get; set; }
                public bool IsRotating { get; set; }
        }
}
