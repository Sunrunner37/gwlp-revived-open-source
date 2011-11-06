using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GameServer.Enums;
using GameServer.ServerData.DataInterfaces;
using GameServer.ServerData.Items;
using ServerEngine;
using ServerEngine.DataManagement;
using ServerEngine.DataManagement.DataInterfaces;
using ServerEngine.DataManagement.DataWrappers;
using ServerEngine.GuildWars.DataBase;
using ServerEngine.GuildWars.DataInterfaces;
using ServerEngine.GuildWars.DataWrappers.Chars;
using ServerEngine.GuildWars.DataWrappers.Clients;
using ServerEngine.GuildWars.DataWrappers.Maps;
using ServerEngine.GuildWars.Tools;

namespace GameServer.ServerData
{
        public sealed class DataCharacter : IIdentifiableData<CharacterData>
        {
                private readonly object objLock = new object();

                private CharacterData data;

                /// <summary>
                ///   Create a new instance of the class
                /// </summary>
                public DataCharacter(CharacterData data)
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
                IHasAppearanceData,
                IHasDistrictData,
                IHasGeneralCharData,
                IHasGenericValueData,
                IHasHeartbeatData,
                IHasMapData,
                IHasMovementData,
                IHasPlayStatusData,
                IHasSkillData,
                IHasTeamData,
                IHasVitalStatusData
        {
                public CharacterData()
                {
                        Attributes = new Dictionary<int, int>();
                        
                        ChatCommands = new Dictionary<string, bool>();
                        
                        Appearance = new byte[0];
                        
                        Items = new CharacterItems();
                        
                        LastHeartBeat = DateTime.Now;
                        
                        Position = new GWVector(0,0,0);
                        Direction = new GWVector(0,0,0);
                        LastMovement = DateTime.Now;
                        MoveState = MovementState.NotMoving;
                        
                        SkillBar = new byte[8];
                        UnlockedSkills = new byte[4];
                }

                public void SaveToDB()
                {
                        using (var db = (MySQL)DataBaseProvider.GetDataBase())
                        {
                                // get the db item
                                var chara = db.charsMasterData.Where(im => im.charID == CharID.Value).First();

                                chara.activeWeaponset = (sbyte)Items.ActiveWeaponset.Number;
                                chara.leadhandWeaponSet1 = (int)Items.Weaponsets[0].LeadHand.Data.PersonalItemID;
                                chara.leadhandWeaponSet2 = (int)Items.Weaponsets[1].LeadHand.Data.PersonalItemID;
                                chara.leadhandWeaponSet3 = (int)Items.Weaponsets[2].LeadHand.Data.PersonalItemID;
                                chara.leadhandWeaponSet4 = (int)Items.Weaponsets[3].LeadHand.Data.PersonalItemID;
                                chara.offhandWeaponSet1 = (int)Items.Weaponsets[0].OffHand.Data.PersonalItemID;
                                chara.offhandWeaponSet2 = (int)Items.Weaponsets[1].OffHand.Data.PersonalItemID;
                                chara.offhandWeaponSet3 = (int)Items.Weaponsets[2].OffHand.Data.PersonalItemID;
                                chara.offhandWeaponSet4 = (int)Items.Weaponsets[3].OffHand.Data.PersonalItemID;

                                db.SubmitChanges();
                        }
                }

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
                public bool ShowPrefix { get; set; }
                public string ChatPrefix { get; set; }
                public bool ShowColor { get; set; }
                public byte ChatColor { get; set; }

                #endregion

                #region Implementation of IHasAppearanceData

                public byte[] Appearance { get; set; }

                #endregion

                #region Implementation of IHasDistrictData

                public bool IsOutpost { get; set; }
                public bool IsPvE { get; set; }
                public int DistrictCountry { get; set; }
                public int DistrictNumber { get; set; }

                #endregion

                #region Implementation of IHasGeneralCharData

                public byte ProfessionPrimary { get; set; }
                public byte ProfessionSecondary { get; set; }
                public uint Level { get; set; }
                public uint Morale { get; set; }

                public CharacterItems Items { get; set; }

                #endregion

                #region Implementation of IHasGenericValueData

                public bool IsFrozen { get; set; }
                public int Energy { get; set; }
                public float EnergyRegen { get; set; }
                public int Health { get; set; }
                public float HealthRegen { get; set; }

                #endregion

                #region Implementation of IHasHeartbeatData

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
                public MovementType  MoveType { get; set; }
                public float Speed { get; set; }
                public float SpeedModifier { get; set; }
                public float Rotation { get; set; }
                public bool IsRotating { get; set; }

                #endregion

                #region Implementation of IHasPlayStatusData

                public PlayStatus Player { get; set; }

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
