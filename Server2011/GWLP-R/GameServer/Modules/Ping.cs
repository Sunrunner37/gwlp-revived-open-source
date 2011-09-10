using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.Enums;
using GameServer.Interfaces;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine.ProcessorQueues;

namespace GameServer.Modules
{
        public class Ping : IModule
        {
                public void Execute()
                {
                        World.GetMaps().AsParallel().ForAll(ProcessPing);
                }

                private static void ProcessPing(Map map)
                {
                        foreach (int charID in map.CharIDs)
                        {
                                Character chara;
                                lock (chara = World.GetCharacter(Chars.CharID, charID))
                                {
                                        var diff = DateTime.Now.Subtract(chara.PingTime).TotalMilliseconds;

                                        if (diff > 5000)
                                        {
                                                // Note: PING
                                                var ping = new NetworkMessage((int)chara[Chars.NetID]);
                                                ping.PacketTemplate = new P001_PingRequest.PacketSt1();
                                                QueuingService.PostProcessingQueue.Enqueue(ping);

                                                chara.PingTime = DateTime.Now;
                                        }
                                }
                        }
                }
        }
}
