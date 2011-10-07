using System;
using System.Linq;
using GameServer.Interfaces;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.NetworkManagement;

namespace GameServer.Modules
{
        public class Ping : IModule
        {
                public void Execute()
                {
                        GameServerWorld.Instance.GetAll<DataClient>().AsParallel().ForAll(ProcessPing);
                }

                private static void ProcessPing(DataClient client)
                {
                        var diff = DateTime.Now.Subtract(client.Data.PingTime).TotalMilliseconds;

                        // time interval check
                        if (diff <= 5000) return;

                        // Note: PING
                        var ping = new NetworkMessage(client.Data.NetID)
                        {
                                PacketTemplate = new P001_PingRequest.PacketSt1()
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ping);

                        // reset the interval time check
                        client.Data.PingTime = DateTime.Now;
                }
        }
}
