using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer.ServerData
{
        public class NonPlayerChar
        {
                public NonPlayerChar()
                {
                        Stats = new NonPlayerCharStats();
                }

                public NonPlayerCharStats Stats { get; set; }
                public int AgentID { get; set; }
        }
}
