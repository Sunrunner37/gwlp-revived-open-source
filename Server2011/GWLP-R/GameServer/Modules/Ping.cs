using System;
using System.Linq;
using GameServer.Enums;
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
                        GameServerWorld.Instance.GetAll<DataMap>().AsParallel().ForAll(ProcessPing);
                }

                private static void ProcessPing(DataMap map)
                {
                        // the following linq expression returns an IEnumerable<CharID> of all characters on that map
                        foreach (var chara in map.GetAll<DataCharacter>())
                        {
                                // failcheck
                                if (chara == null) continue;
                                if (chara.Data.Player != PlayStatus.ReadyToPlay) continue;

                                var diff = DateTime.Now.Subtract(chara.Data.PingTime).TotalMilliseconds;

                                // time interval check
                                if (diff <= 5000) continue;

                                // Note: PING
                                var ping = new NetworkMessage(chara.Data.NetID)
                                {
                                        PacketTemplate = new P001_PingRequest.PacketSt1()
                                };
                                QueuingService.PostProcessingQueue.Enqueue(ping);

                                // reset the interval time check
                                chara.Data.PingTime = DateTime.Now;
                        }
                }
        }
}
