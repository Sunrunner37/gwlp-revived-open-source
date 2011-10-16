using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using GameServer.Enums;
using GameServer.ServerData.DataInterfaces;
using ServerEngine.DataManagement;
using ServerEngine.GuildWars.DataInterfaces;
using ServerEngine.GuildWars.DataWrappers.Chars;
using ServerEngine.GuildWars.DataWrappers.Maps;
using ServerEngine.GuildWars.Tools;

namespace GameServer.ServerData
{
        public sealed class DataNpc : IIdentifiableData<NpcData>
        {
                private readonly object objLock = new object();

                private NpcData data;

                /// <summary>
                ///   Creates a new instance of the class
                /// </summary>
                public DataNpc(NpcData data)
                {
                        lock (objLock)
                        {
                                this.data = data;

                                Debug.WriteLine(string.Format("Created new {0}", GetType().Name));
                        }
                }

                #region Implementation of IEnumerable

                public IEnumerator<IWrapper> GetEnumerator()
                {
                        lock (objLock)
                        {
                                return (new List<IWrapper>
                                {
                                        // IHasCharData
                                        data.AgentID,
                                        data.LocalID,
                                        data.Name,
                                }).GetEnumerator();
                        }
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                        return GetEnumerator();
                }

                #endregion

                #region Implementation of IIdentifiableData<NpcData>

                public NpcData Data
                {
                        get { lock (objLock) return data; }
                        set { lock (objLock) data = value; }
                }

                #endregion
        }

        public class NpcData:
                IHasCharData,
                IHasAttributeData,
                IHasChatData,
                IHasAppearanceData,
                IHasGeneralCharData,
                IHasGeneralNpcData,
                IHasGenericValueData,
                IHasMapData,
                IHasMovementData,
                IHasSkillData,
                IHasTeamData,
                IHasVitalStatusData
        {
                public NpcData()
                {
                        Attributes = new Dictionary<int, int>();
                        ChatCommands = new Dictionary<string, bool>();
                        Appearance = new byte[0];
                        ModelHash = new byte[0];
                        Position = new GWVector(0, 0, 0);
                        Direction = new GWVector(0, 0, 0);
                        LastMovement = DateTime.Now;
                        MoveState = MovementState.NotMoving;
                        SkillBar = new byte[8];
                        UnlockedSkills = new byte[4];
                }

                #region Implementation of IHasCharData

                public AgentID AgentID { get; set; }
                public LocalID LocalID { get; set; }
                public Name Name { get; set; }

                #endregion

                #region Implementation of IHasAttributeData

                public Dictionary<int, int> Attributes { get; set; }
                public int AttPtsFree { get; set; }
                public int AttPtsTotal { get; set; }

                #endregion

                #region Implementation of IHasChatData

                public Dictionary<string, bool> ChatCommands { get; set; }
                public bool ShowPrefix { get; set; }
                public string ChatPrefix { get; set; }
                public bool ShowColor { get; set; }
                public byte ChatColor { get; set; }

                #endregion

                #region Implementation of IHasAppearanceData

                public byte[] Appearance { get; set; }

                #endregion

                #region Implementation of IHasGeneralCharData

                public byte ProfessionPrimary { get; set; }
                public byte ProfessionSecondary { get; set; }
                public uint Level { get; set; }
                public uint Morale { get; set; }

                #endregion

                #region Implementation of IHasGeneralNpcData

                public int NpcFileID { get; set; }
                public byte[] ModelHash { get; set; }
                public int NpcFlags { get; set; }
                public int Scale { get; set; }

                #endregion

                #region Implementation of IHasGenericValueData

                public bool IsFrozen { get; set; }
                public int Energy { get; set; }
                public float EnergyRegen { get; set; }
                public int Health { get; set; }
                public float HealthRegen { get; set; }

                #endregion

                #region Implementation of IHasMapData

                public GameFileID GameFileID { get; set; }
                public GameMapID GameMapID { get; set; }
                public MapID MapID { get; set; }

                #endregion

                #region Implementation of IHasMovementData

                public GWVector Position { get; set; }
                public GWVector Direction { get; set; }
                public uint TrapezoidIndex { get; set; }
                public DateTime LastMovement { get; set; }
                public bool AtBorder { get; set; }
                public MovementState MoveState { get; set; }
                public MovementType MoveType { get; set; }
                public float Speed { get; set; }
                public float SpeedModifier { get; set; }
                public float Rotation { get; set; }
                public bool IsRotating { get; set; }

                #endregion

                #region Implementation of IHasSkillData

                public int SkillPtsFree { get; set; }
                public int SkillPtsTotal { get; set; }
                public byte[] SkillBar { get; set; }
                public byte[] UnlockedSkills { get; set; }

                #endregion

                #region Implementation of IHasTeamData

                public int TeamNumber { get; set; }

                #endregion

                #region Implementation of IHasVitalStatusData

                public int VitalStatus { get; set; }

                #endregion
        }
}
