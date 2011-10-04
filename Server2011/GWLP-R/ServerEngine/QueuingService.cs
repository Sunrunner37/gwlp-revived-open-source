using System.Collections.Concurrent;
using ServerEngine.NetworkManagement;

namespace ServerEngine
{
        public class QueuingService
        {
                static QueuingService()
                {
                        NetInQueue = new ConcurrentQueue<NetworkMessage>();
                        PostProcessingQueue = new ConcurrentQueue<NetworkMessage>();
                        NetOutQueue = new ConcurrentQueue<NetworkMessage>();
                }

                /// <summary>
                ///   This property contains the global incoming network message queue
                /// </summary>
                public static ConcurrentQueue<NetworkMessage> NetInQueue { get; private set; }

                /// <summary>
                ///   This property contains the post processing network message queue
                ///   thats used by for packets that will be translated to byte array and enqueued in the 
                ///   NetOutQueue
                /// </summary>
                public static ConcurrentQueue<NetworkMessage> PostProcessingQueue { get; private set; }

                /// <summary>
                ///   This property contains the global outgoing network message queue
                /// </summary>
                public static ConcurrentQueue<NetworkMessage> NetOutQueue { get; private set; }

        }
}
