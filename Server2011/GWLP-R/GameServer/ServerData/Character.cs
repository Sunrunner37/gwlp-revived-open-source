using System;
using System.Collections.Generic;
using System.Diagnostics;
using GameServer.Enums;

namespace GameServer.ServerData
{
        public class Character : IIdentifiable<Chars>
        {
                private readonly object objLock = new object();

                private DateTime pingTime;
                private int mapID;
                private bool isAtOutpost;
                private CharacterStats charStats;
                private DateTime lastHeartBeat;
                private readonly Dictionary<Chars, object> identifierKeyEnumeration;

                /// <summary>
                ///   Create a new instance of the class
                /// </summary>
                public Character(int charID, int accID, int netID, int localID, int agentID, string name)
                {
                        var tmp = new Dictionary<Chars, object>();
                        tmp.Add(Chars.CharID, charID);
                        tmp.Add(Chars.AccID, accID);
                        tmp.Add(Chars.NetID, netID);
                        tmp.Add(Chars.LocalID, localID);
                        tmp.Add(Chars.AgentID, agentID);
                        tmp.Add(Chars.Name, name);
                       
                        identifierKeyEnumeration = tmp;

                        CharStats = new CharacterStats();
                        
                        Debug.WriteLine("Created new character");
                }

                /// <summary>
                ///   This indexer returns the identifier of the given type.
                /// </summary>
                /// <param name="identType"></param>
                /// <returns></returns>
                public object this[Chars identType]
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
                ///   This property contains the timestamp of the last Ping
                /// </summary>
                public DateTime PingTime
                {
                        get
                        {
                                lock (objLock)
                                {
                                        return pingTime;
                                }
                        }
                        set
                        {
                                lock (objLock)
                                {
                                        pingTime = value;
                                }
                        }
                }

                /// <summary>
                ///   This property contains the MapID of the char
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
                ///   This property determines whether the char is at an outpost
                /// </summary>
                public bool IsAtOutpost
                {
                        get
                        {
                                lock (objLock)
                                {
                                        return isAtOutpost;
                                }
                        }
                        set
                        {
                                lock (objLock)
                                {
                                        isAtOutpost = value;
                                }
                        }
                }

                /// <summary>
                ///   This property contains the general char stats object
                /// </summary>
                public CharacterStats CharStats
                {
                        get
                        {
                                lock (objLock)
                                {
                                        return charStats;
                                }
                        }
                        set
                        {
                                lock (objLock)
                                {
                                        charStats = value;
                                }
                        }
                }

                /// <summary>
                ///   This property contains the timestamp of the last heartbeat
                /// </summary>
                public DateTime LastHeartBeat
                {
                        get
                        {
                                lock (objLock)
                                {
                                        return lastHeartBeat;
                                }
                        }
                        set
                        {
                                lock (objLock)
                                {
                                        lastHeartBeat = value;
                                }
                        }
                }

                /// <summary>
                ///   This property contains the identifier - key enumeration of the client.
                /// </summary>
                public IEnumerable<KeyValuePair<Chars, object>> IdentifierKeyEnumeration
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
