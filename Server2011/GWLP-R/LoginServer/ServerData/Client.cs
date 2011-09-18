using System;
using System.Collections.Generic;
using System.Diagnostics;
using LoginServer.Enums;
using ServerEngine.Tools;

namespace LoginServer.ServerData
{
        public sealed class Client : IIdentifiable<Idents.Clients>
        {

                private readonly object objLock = new object();

                private SyncState status;
                private byte[] initCryptSeed;
                private int syncCount;
                private int mapID;
                private string email;
                private string password;
                private DateTime lastStatusChange;
                private byte[][] securityKeys = { new byte[4], new byte[4] };
                private readonly Dictionary<Idents.Clients, object> identifierKeyEnumeration;

                /// <summary>
                ///   Create a new instance of the class, without any indentifiers.
                /// </summary>
                public Client()
                {
                        identifierKeyEnumeration = new Dictionary<Idents.Clients, object>();
                        LastStatusChange = DateTime.Now;

                        Debug.WriteLine("Created new client");
                }

                /// <summary>
                ///   Create a new instance of the class.
                /// </summary>
                public Client(int netID)
                {
                        var tmp = new Dictionary<Idents.Clients, object>();
                        tmp.Add(Idents.Clients.NetID, netID);

                        identifierKeyEnumeration = tmp;
                        LastStatusChange = DateTime.Now;

                        Debug.WriteLine("Created new client");
                }

                /// <summary>
                ///   Create a new instance of the class.
                /// </summary>
                public Client(int netID, int accountID, int charID)
                {
                        var tmp = new Dictionary<Idents.Clients, object>();
                        tmp.Add(Idents.Clients.NetID, netID);
                        tmp.Add(Idents.Clients.AccID, accountID);
                        tmp.Add(Idents.Clients.CharID, charID);

                        identifierKeyEnumeration = tmp;
                        LastStatusChange = DateTime.Now;

                        Debug.WriteLine("Created new client");
                }

                /// <summary>
                ///   This property contains the syncronization state of the client.
                /// </summary>
                public SyncState Status
                {
                        get
                        {
                                lock (objLock)
                                {
                                        return status;
                                }
                        }
                        set
                        {
                                lock (objLock)
                                {
                                        status = value;
                                        // get netID
                                        object netID;
                                        identifierKeyEnumeration.TryGetValue(Idents.Clients.NetID, out netID);
                                        // write acknowledgement
                                        Debug.WriteLine("Clients[{0}] changed Status to: {1}", (int) netID, Status);

                                        LastStatusChange = DateTime.Now;
                                }
                        }
                }

                /// <summary>
                ///   This indexer returns the identifier of the given type.
                /// </summary>
                /// <param name="identType"></param>
                /// <returns></returns>
                public object this[Idents.Clients identType]
                {
                        get
                        {
                                lock (objLock)
                                {
                                        object id;

                                        identifierKeyEnumeration.TryGetValue(identType, out id);

                                        return id;
                                }
                        }
                }

                /// <summary>
                ///   This property holds the initial encryption seed.
                /// </summary>
                public byte[] InitCryptSeed
                {
                        get
                        {
                                lock (objLock)
                                {
                                        return initCryptSeed;
                                }
                        }
                        set
                        {
                                lock (objLock)
                                {
                                        initCryptSeed = value;
                                }
                        }
                }

                /// <summary>
                ///   This property contains the login counter, a.k.a. Client Synchronization State Counter
                /// </summary>
                public int LoginCount
                {
                        get
                        {
                                lock (objLock)
                                {
                                        return syncCount;
                                }
                        }
                        set
                        {
                                lock (objLock)
                                {
                                        syncCount = value;
                                }
                                
                        }
                }

                /// <summary>
                ///   This property contains the MapID of the map that the clients currently plays on.
                /// </summary>
                public int MapID
                {
                        get
                        {
                                lock (objLock)
                                {
                                        return mapID;
                                }
                        }
                        set
                        {
                                lock (objLock)
                                {
                                        mapID = value;
                                }

                        }
                }

                /// <summary>
                ///   This property contains the client's email.
                /// </summary>
                public string Email
                {
                        get
                        {
                                lock (objLock)
                                {
                                        return email;
                                }
                        }
                        set
                        {
                                lock (objLock)
                                {
                                        email = value;
                                }

                        }
                }

                /// <summary>
                ///   This property contains the client's password.
                /// </summary>
                public string Password
                {
                        get
                        {
                                lock (objLock)
                                {
                                        return password;
                                }
                        }
                        set
                        {
                                lock (objLock)
                                {
                                        password = value;
                                }

                        }
                }

                /// <summary>
                ///   This property contains the time of the last change of Status.
                /// </summary>
                public DateTime LastStatusChange
                {
                        get
                        {
                                lock (objLock)
                                {
                                        return lastStatusChange;
                                }
                        }
                        private set
                        {
                                lock (objLock)
                                {
                                        lastStatusChange = value;
                                }

                        }
                }

                /// <summary>
                ///   This property contains the security keys that are passed to the game server.
                /// </summary>
                public byte[][] SecurityKeys
                {
                        get
                        {
                                lock (objLock)
                                {
                                        return securityKeys;
                                }
                        }
                        set
                        {
                                lock (objLock)
                                {
                                        securityKeys = value;
                                }

                        }
                }

                /// <summary>
                ///   This property contains the identifier - key enumeration of the client.
                /// </summary>
                public IEnumerable<KeyValuePair<Idents.Clients, object>> IdentifierKeyEnumeration
                {
                        get
                        {
                                lock (objLock)
                                {
                                        return identifierKeyEnumeration;
                                }
                        }
                }
                

        }
}
