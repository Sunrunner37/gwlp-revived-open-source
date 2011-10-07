using System;
using System.Linq;
using GameServer.Interfaces;
using GameServer.ServerData;

namespace GameServer.Modules
{
        public class ActionQueue : IModule
        {
                public void Execute()
                {
                        GameServerWorld.Instance.GetAll<DataMap>().AsParallel().ForAll(ProcessMapActions);
                }

                private static void ProcessMapActions(DataMap map)
                {
                        // iterate trough the action queue, execute all actions
                        for (var i = 0; i < map.Data.ActionQueue.Count; i++)
                        {
                                Action<DataMap> act;
                                if (map.Data.ActionQueue.TryDequeue(out act))
                                {
                                        act(map);
                                }
                        }
                }
        }
}
