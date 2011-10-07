using System;
using System.Collections.Concurrent;

namespace GameServer.ServerData.DataInterfaces
{
        public interface IHasActionQueueData<T>
        {
                ConcurrentQueue<Action<T>> ActionQueue { get; set; }
        }
}