using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                        World.GetMaps().AsParallel().ForAll(ProcessHeartBeatPackets);
                }

                private static void ProcessHeartBeatPackets(Map map)
                {
                        foreach (int charID in map.CharIDs)
                        {
                                var chara = GameServerWorld.Instance.Get<DataCharacter>(Chars.CharID, charID);
                                if (chara != null)
                                {

                                        var diff = DateTime.Now.Subtract(chara.LastHeartBeat).TotalMilliseconds;

                                        if (diff > 250)
                                        {
                                                // Note: HEARTBEAT
                                                var heartBeat = new NetworkMessage((int) chara[Chars.NetID]);
                                                heartBeat.PacketTemplate = new P019_Heartbeat.PacketSt19()
                                                {
                                                        Data1 = (uint) diff
                                                };
                                                QueuingService.PostProcessingQueue.Enqueue(heartBeat);

                                                chara.LastHeartBeat = DateTime.Now;
                                        }
                                        
                                }
                        }
                }
        }
}
