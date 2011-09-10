using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Interfaces;
using GameServer.ServerData;

namespace GameServer.Modules
{
        public class ActionQueue : IModule
        {
                public void Execute()
                {
                        World.GetMaps().AsParallel().ForAll(ProcessMapActions);
                }

                private static void ProcessMapActions(Map map)
                {
                       
                        for (int i = 0; i < map.ActionQueue.Count; i++)
                        {
                                Action<Map> act;
                                if (map.ActionQueue.TryDequeue(out act))
                                {
                                        act(map);
                                }
                        }
                }
        }
}
