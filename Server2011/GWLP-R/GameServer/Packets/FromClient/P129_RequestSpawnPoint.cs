using System;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 129)]
        public class P129_RequestSpawnPoint : IPacket
        {
                public class PacketSt129 : IPacketTemplate
                {
                        public UInt16 Header { get { return 129; } }
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt129>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // nothing to parse here ;)

                        var chara = GameServerWorld.Instance.Get<DataClient>(message.NetID).Character;

                        // Note: IL SPAWN POINT
                        var terminator = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new P391_InstanceLoadSpawnPoint.PacketSt391
                                {
                                        GameMapFileID = chara.Data.GameFileID.Value,
                                        SpawnX = chara.Data.Position.X,
                                        SpawnY = chara.Data.Position.Y,
                                        SpawnPlane = (ushort)chara.Data.Position.PlaneZ,
                                        Data5 = 0,
                                        Data6 = 0,
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(terminator);
                        

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt129> pParser;
        }
}
