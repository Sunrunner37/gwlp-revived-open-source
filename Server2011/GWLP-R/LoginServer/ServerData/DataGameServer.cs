
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using LoginServer.ServerData.DataInterfaces;
using ServerEngine.DataManagement;
using ServerEngine.DataManagement.DataInterfaces;
using ServerEngine.DataManagement.DataWrappers;

namespace LoginServer.ServerData
{
        public class DataGameServer : IIdentifiableData<GameServerData>
        {
                private readonly object objLock = new object();

                private GameServerData data;

                /// <summary>
                ///   Create a new instance of the class, with forced data
                /// </summary>
                public DataGameServer(GameServerData data)
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
                                                        // IHasNetworkData
                                                        data.NetID,
                                                        // exclude the following as game servers may connect
                                                        // from the same access point
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

                #region Implementation of IIdentifiableData<GameServerData>

                public GameServerData Data
                {
                        get
                        { lock (objLock) return data; }
                        set
                        { lock (objLock) data = value; }
                }

                #endregion
        }

        public class GameServerData :
                IHasNetworkData,
                IHasGameServerData
        {
                #region Implementation of IHasNetworkData

                public NetID NetID { get; set; }
                public IPAddress IPAddress { get; set; }
                public Port Port { get; set; }

                #endregion

                #region Implementation of IHasGameServerData

                public byte Utilization { get; set; }
                public ushort[] AvailableMaps { get; set; }

                #endregion
        }
}
