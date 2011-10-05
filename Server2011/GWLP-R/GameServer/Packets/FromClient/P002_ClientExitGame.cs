using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using GameServer.Enums;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.NetworkManagement;
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

                        var chara = GameServerWorld.Instance.Get<DataCharacter>(Chars.NetID, message.NetID);
                        
                        if (GameServerWorld.Instance.Get<DataClient>(Clients.NetID, message.NetID).Status == SyncStatus.Dispatching)
                        {
                                kick = false;
#warning DEBUG
                                // send dispatch connection close
                                var ilChar = new NetworkMessage(message.NetID);
                                ilChar.PacketTemplate = new P141_DispatchConnectionTermination.PacketSt141()
                                {
                                        GameMapID = (ushort)(int)GameServerWorld.Instance.Get<DataMap>(Maps.MapID, chara.MapID)[Maps.GameMapID],
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

                                // also, set the client to 'paused'
                                // this should prevent us from loosing data because of the network manager deleting stuff.
                                // (at least till the client rejoined, then we can safely terminate it)
                                NetworkManager.Instance.PauseClient(message.NetID);
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
