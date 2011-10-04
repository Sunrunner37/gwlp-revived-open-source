using System;
using System.IO;
using System.Linq;
using GameServer.Enums;
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

                        var client = World.GetClient(Clients.NetID, message.NetID);
                        var map = World.GetMap(Maps.MapID, client.MapID);
                        
#warning Redundant spawn search here!
                        // execute this directly
                        MapSpawn spawn;
                        var spawnEnum = from s in map.Spawns.Values
                                        where s.IsOutpost && s.IsPvE
                                        select s;

                        if (spawnEnum.Count() == 0)
                        {
                                spawn = new MapSpawn
                                {
                                        SpawnX = 1F,
                                        SpawnY = 1F,
                                        SpawnPlane = 0
                                };
                        }
                        else
                        {
                                spawn = spawnEnum.First();
                        }

                        // Note: IL SPAWN POINT
                        var terminator = new NetworkMessage(message.NetID);
                        terminator.PacketTemplate = new P391_InstanceLoadSpawnPoint.PacketSt391();
                        ((P391_InstanceLoadSpawnPoint.PacketSt391)terminator.PacketTemplate).GameMapFileID = (uint)(int)map[Maps.GameFileID];
#warning FIXME: Include SpawnRadius here
                        ((P391_InstanceLoadSpawnPoint.PacketSt391)terminator.PacketTemplate).SpawnX = spawn.SpawnX;
                        ((P391_InstanceLoadSpawnPoint.PacketSt391)terminator.PacketTemplate).SpawnY = spawn.SpawnY;
                        ((P391_InstanceLoadSpawnPoint.PacketSt391)terminator.PacketTemplate).SpawnPlane = (ushort)spawn.SpawnPlane;
                        ((P391_InstanceLoadSpawnPoint.PacketSt391)terminator.PacketTemplate).Data5 = 0;
                        ((P391_InstanceLoadSpawnPoint.PacketSt391)terminator.PacketTemplate).Data6 = 0;
                        QueuingService.PostProcessingQueue.Enqueue(terminator);
                        

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt129> pParser;
        }
}
