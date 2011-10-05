using System;
using System.Collections;
using System.Collections.Generic;
using GameServer.Enums;
using GameServer.ServerData.DataInterfaces;
using ServerEngine.DataManagement;
using ServerEngine.DataManagement.DataInterfaces;
using ServerEngine.DataManagement.DataWrappers;
using ServerEngine.GuildWars.DataInterfaces;
using ServerEngine.GuildWars.DataWrappers.Chars;
using ServerEngine.GuildWars.DataWrappers.Clients;
using ServerEngine.GuildWars.DataWrappers.Maps;
using ServerEngine.GuildWars.Tools;

namespace GameServer.ServerData
{
        public class DataCharacter : IIdentifiableData<CharacterData>
        {
                private readonly object objLock = new object();

                private CharacterData data;

                /// <summary>
                ///   Create a new instance of the class
                /// </summary>
                public DataCharacter(CharacterData data)
                {
                        this.data = data;
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
                                        // IHasClientData
                                        data.AccID,
                                        data.CharID,
                                        // IHasNetworkData
                                        data.NetID,
                                        // exclude the following, as clients may play from the same
                                        // network access point
                                        //data.IPAddress,
                                        //data.Port
                                }).GetEnumerator();
                        }
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                        return GetEnumerator();
                }

                #endregion

                #region Implementation of IIdentifiableData<CharacterData>

                public CharacterData Data
                {
                        get { lock (objLock) return data; }
                        set { lock (objLock) data = value; }
                }

                #endregion
        }

        public class CharacterData :
                IHasNetworkData,
                IHasClientData,
                IHasCharData,
                IHasAttributeData,
                IHasChatData,
                IHasCharAppearanceData,
                IHasDistrictData,
                IHasGeneralCharData,
                IHasGenericValueData,
                IHasHeartbeatData,
                IHasMapData,
                IHasMovementData,
                IHasPingData,
                IHasSkillData,
                IHasVitalStatusData
        {
                #region Implementation of IHasNetworkData

                public NetID NetID { get; set; }
                public IPAddress IPAddress { get; set; }
                public Port Port { get; set; }

                #endregion

                #region Implementation of IHasClientData

                public AccID AccID { get; set; }
                public CharID CharID { get; set; }

                #endregion

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
                public string ChatPrefix { get; set; }
                public byte ChatColor { get; set; }

                #endregion

                #region Implementation of IHasCharAppearanceData

                public byte[] Appearance { get; set; }

                #endregion

                #region Implementation of IHasDistrictData

                public bool IsOutpost { get; set; }
                public bool IsPvP { get; set; }
                public int DistrictCountry { get; set; }
                public int DistrictNumber { get; set; }

                #endregion

                #region Implementation of IHasGeneralCharData

                public byte ProfessionPrimary { get; set; }
                public byte ProfessionSecondary { get; set; }
                public int Level { get; set; }
                public int Morale { get; set; }

                #endregion

                #region Implementation of IHasGenericValueData

                public bool IsFrozen { get; set; }
                public int Energy { get; set; }
                public float EnergyRegen { get; set; }
                public int Health { get; set; }
                public float HealthRegen { get; set; }

                #endregion

                #region Implementation of IHasHeartbeatData

                public bool EnabledHartBeat { get; set; }
                public DateTime LastHeartBeat { get; set; }

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
                public int MoveType { get; set; }
                public float Speed { get; set; }
                public float SpeedModifier { get; set; }
                public float Rotation { get; set; }
                public bool IsRotating { get; set; }

                #endregion

                #region Implementation of IHasPingData

                public DateTime PingTime { get; set; }

                #endregion

                #region Implementation of IHasSkillData

                public int SkillPtsFree { get; set; }
                public int SkillPtsTotal { get; set; }
                public byte[] SkillBar { get; set; }
                public byte[] UnlockedSkills { get; set; }

                #endregion

                #region Implementation of IHasVitalStatusData

                public int VitalStatus { get; set; }

                #endregion
        }
}
