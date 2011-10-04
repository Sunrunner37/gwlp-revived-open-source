using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using LoginServer.Enums;
using LoginServer.ServerData.DataInterfaces;
using ServerEngine.DataManagement;
using ServerEngine.DataManagement.DataInterfaces;
using ServerEngine.DataManagement.DataWrappers;
using ServerEngine.GuildWars.DataInterfaces;
using ServerEngine.GuildWars.DataWrappers.Clients;
using ServerEngine.GuildWars.DataWrappers.Maps;

namespace LoginServer.ServerData
{
        public sealed class DataClient : IIdentifiableData<ClientData>
        {
                private readonly object objLock = new object();

                private ClientData data;

                /// <summary>
                ///   Create a new instance of the class, with forced data
                /// </summary>
                public DataClient(ClientData data)
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

                #region Implementation of IIdentifiableData<ClientData>

                public ClientData Data
                {
                        get 
                        { lock (objLock) return data; }
                        set 
                        { lock (objLock) data = value; }
                }

                #endregion
        }

        public class ClientData :
                IHasClientData,
                IHasNetworkData,
                IHasMapData,
                IHasAccountData,
                IHasEncryptionData,
                IHasSyncStatusData,
                IHasTransferSecurityData
        {
                /// <summary>
                ///   Creates a new instance of the class
                /// </summary>
                public ClientData()
                {
                        Email = "";
                        Password = "";
                        EncryptionSeed = new byte[20];
                        Status = SyncStatus.ConnectionEstablished;
                        LastStatusChange = DateTime.Now;
                        SecurityKeys = new[] { new byte[4], new byte[4] };
                }

                #region Implementation of IHasClientData

                public AccID AccID { get; set; }
                public CharID CharID { get; set; }

                #endregion

                #region Implementation of IHasNetworkData

                public NetID NetID { get; set; }
                public IPAddress IPAddress { get; set; }
                public Port Port { get; set; }

                #endregion

                #region Implementation of IHasMapData

                public GameFileID GameFileID { get; set; }
                public GameMapID GameMapID { get; set; }
                public MapID MapID { get; set; }

                #endregion

                #region Implementation of IHasAccountData

                public string Email { get; set; }
                public string Password { get; set; }

                #endregion

                #region Implementation of IHasEncryptionData

                public byte[] EncryptionSeed { get; set; }

                #endregion

                #region Implementation of IHasSyncStatusData

                public SyncStatus Status { get; set; }
                public DateTime LastStatusChange { get; set; }
                public uint SyncCount { get; set; }

                #endregion

                #region Implementation of IHasTransferSecurityData

                public byte[][] SecurityKeys { get; set; }

                #endregion
        }
}
