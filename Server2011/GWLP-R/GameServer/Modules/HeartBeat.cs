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
        public class HeartBeat : IModule
        {
                public void Execute()
                {
                        GameServerWorld.Instance.GetAll<DataMap>().AsParallel().ForAll(ProcessHeartBeatPackets);
                }

                private static void ProcessHeartBeatPackets(DataMap map)
                {
                        // the following linq expression returns an IEnumerable<CharID> of all characters on that map
                        foreach (var chara in map.GetAll<DataCharacter>())
                        {
                                // failcheck
                                if (chara == null) continue;
                                if (chara.Data.Player != PlayStatus.ReadyToPlay) continue;

                                var diff = DateTime.Now.Subtract(chara.Data.LastHeartBeat).TotalMilliseconds;

                                // time check
                                if (diff <= 250) continue;

                                // Note: HEARTBEAT
                                var heartBeat = new NetworkMessage(chara.Data.NetID)
                                {
                                        PacketTemplate = new P019_Heartbeat.PacketSt19
                                        {
                                                Data1 = (uint)diff
                                        }
                                };
                                QueuingService.PostProcessingQueue.Enqueue(heartBeat);

                                // update the char's heartbeat time
                                chara.Data.LastHeartBeat = DateTime.Now;
                        }
                }
        }
}
