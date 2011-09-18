using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer.ServerData
{
        public class NonPlayerChar
        {
                private readonly object objLock = new object();

                private NonPlayerCharStats stats;
                private int agentID;

                /// <summary>
                ///   Creates a new instance of the class
                /// </summary>
                public NonPlayerChar()
                {
                        Stats = new NonPlayerCharStats();
                }

                /// <summary>
                ///   This property contains the general NPC stats
                /// </summary>
                public NonPlayerCharStats Stats
                {
                        get
                        {
                                lock (objLock)
                                {
                                        return stats;
                                }
                        }
                        set
                        {
                                lock (objLock)
                                {
                                        stats = value;
                                }
                        }
                }

                /// <summary>
                ///   This property contains the AgentID of the NPC
                /// </summary>
                public int AgentID
                {
                        get
                        {
                                lock (objLock)
                                {
                                        return agentID;
                                }
                        }
                        set
                        {
                                lock (objLock)
                                {
                                        agentID = value;
                                }
                        }
                }
        }
}
