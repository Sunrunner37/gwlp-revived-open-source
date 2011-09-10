using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using GameServer.Enums;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine.NetworkManagement;
using ServerEngine.ProcessorQueues;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 2)]
        public class P002_ClientExitGame : IPacket
        {
                public class PacketSt2 : IPacketTemplate
                {
                        public UInt16 Header { get { return 2; } }
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt2>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // nothing to parse here

                        bool kick = true;

                        Character chara;
                        lock (chara = World.GetCharacter(Chars.NetID, message.NetID))
                        {
                                if (World.GetClient(Clients.NetID, message.NetID).Status == SyncState.Dispatching)
                                {
                                        kick = false;
#warning DEBUG
                                        // send dispatch connection close
                                        var ilChar = new NetworkMessage(message.NetID);
                                        ilChar.PacketTemplate = new P141_DispatchConnectionTermination.PacketSt141()
                                        {
                                                GameMapID = (ushort)(int)World.GetMap(Maps.MapID, chara.MapID)[Maps.GameMapID],
                                                Data1 = 0
                                        };
                                        QueuingService.PostProcessingQueue.Enqueue(ilChar);

                                        // Note: HEARTBEAT
                                        var heartBeat = new NetworkMessage(message.NetID);
                                        heartBeat.PacketTemplate = new P019_Heartbeat.PacketSt19()
                                        {
                                                Data1 = 250
                                        };
                                        QueuingService.PostProcessingQueue.Enqueue(heartBeat);
                                }
                        }

                        if (kick)
                                // does everything necessary for us:
                                World.KickClient(Clients.NetID, message.NetID);

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt2> pParser;
        }
}
