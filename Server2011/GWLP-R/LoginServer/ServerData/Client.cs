using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using LoginServer.Enums;
using ServerEngine.Tools;

namespace LoginServer.ServerData
{
        public class Client : IIdentifiable<Idents.Clients>
        {
                public Client()
                {
                        identifierKeyEnumeration = new Dictionary<Idents.Clients, object>();
                        LastStatusChange = DateTime.Now;

                        Debug.WriteLine("Created new client");
                }

                public Client(int netID)
                {
                        var tmp = new Dictionary<Idents.Clients, object>();
                        tmp.Add(Idents.Clients.NetID, netID);

                        identifierKeyEnumeration = tmp;
                        LastStatusChange = DateTime.Now;

                        Debug.WriteLine("Created new client");
                }

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

                private SyncState status;
                public SyncState Status
                {
                        get { return status; }
                        set
                        {
                                status = value;
                                // get netID
                                object netID;
                                identifierKeyEnumeration.TryGetValue(Idents.Clients.NetID,out netID);
                                // write acknowledgement
                                Debug.WriteLine("Clients[{0}] changed Status to: {1}", (int)netID, Status);

                                LastStatusChange = DateTime.Now;
                        }
                }

                public object this[Idents.Clients identType]
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

                public string Email { get; set; }
                public string Password { get; set; }

                public DateTime LastStatusChange { get; private set; }

                public byte[][] SecurityKeys = { new byte[4], new byte[4] };

                private readonly Dictionary<Idents.Clients, object> identifierKeyEnumeration;
                public IEnumerable<KeyValuePair<Idents.Clients, object>> IdentifierKeyEnumeration { get { return identifierKeyEnumeration; } }
                

        }
}
