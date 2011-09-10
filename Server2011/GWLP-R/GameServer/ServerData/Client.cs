using System.Collections.Generic;
using System.Diagnostics;
using GameServer.Enums;
using ServerEngine.Tools;

namespace GameServer.ServerData
{
        public class Client : IIdentifiable<Clients>
        {
                public Client()
                {
                        identifierKeyEnumeration = new Dictionary<Clients, object>(); ;

                        Debug.WriteLine("Created new client");
                }

                public Client(int netID)
                {
                        var tmp = new Dictionary<Clients, object>();
                        tmp.Add(Clients.NetID, netID);

                        identifierKeyEnumeration = tmp;

                        Debug.WriteLine("Created new client");
                }

                public Client(int netID, int accountID, int charID)
                {
                        var tmp = new Dictionary<Clients, object>();
                        tmp.Add(Clients.NetID, netID);
                        tmp.Add(Clients.AccID, accountID);
                        tmp.Add(Clients.CharID, charID);

                        identifierKeyEnumeration = tmp;

                        Debug.WriteLine("Created new client");
                }

                private SyncState status;
                public SyncState Status
                {
                        get { return status; }
                        set
                        {
                                status = value;
                                // get netID
                                object netID;
                                identifierKeyEnumeration.TryGetValue(Clients.NetID, out netID);
                                // write acknowledgement
                                Debug.WriteLine("Clients[{0}] changed Status to: {1}", (int)netID, Status);
                        }
                }

                public object this[Clients identType]
                {
                        get
                        {
                                object id;

                                identifierKeyEnumeration.TryGetValue(identType, out id);

                                return id;
                        }
                }

                public byte[] InitCryptSeed { get; set; }

                public int LoginCount { get; set; }

                public int MapID { get; set; }


                public byte[][] SecurityKeys = { new byte[4], new byte[4] };

                private readonly Dictionary<Clients, object> identifierKeyEnumeration;
                public IEnumerable<KeyValuePair<Clients, object>> IdentifierKeyEnumeration { get { return identifierKeyEnumeration; } }
        }
}
