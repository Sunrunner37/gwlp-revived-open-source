using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GameServer.Enums;
using ServerEngine.Tools;

namespace GameServer.ServerData
{
        public class Character : IIdentifiable<Chars>
        {
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
                        Commands = new Dictionary<string, bool>();

                        Debug.WriteLine("Created new character");
                }

                public object this[Chars identType]
                {
                        get
                        {
                                object id;

                                identifierKeyEnumeration.TryGetValue(identType, out id);

                                return id;
                        }
                }
                public DateTime PingTime { get; set; }

                public int MapID { get; set; }
                public bool IsAtOutpost { get; set; }

                public int AttPtsFree { get; set; }
                public int AttPtsTotal { get; set; }
                public int SkillPtsFree { get; set; }
                public int SkillPtsTotal { get; set; }

                public CharacterStats CharStats { get; set; }

                public DateTime LastHeartBeat { get; set; }

                public Dictionary<string, bool> Commands { get; set; }

                public string ChatPrefix { get; set; }
                public byte ChatColor { get; set; }

                private readonly Dictionary<Chars, object> identifierKeyEnumeration;
                public IEnumerable<KeyValuePair<Chars, object>> IdentifierKeyEnumeration { get { return identifierKeyEnumeration; } }
        }
}
