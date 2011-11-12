using System;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.GuildWars.Tools;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 130)]
        public class P130_CreateNewCharacter : IPacket
        {
                public class PacketSt130 : IPacketTemplate
                {
                        public UInt16 Header { get { return 130; } }
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt130>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        //ItemStreamStart

                        //Backpack

                        //EquipmentPage

                        //Storage

                        //Weaponset (reset)

                        /*var ppp17 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.Packet79.PacketSt79
                                {
                                        Data1 = 2
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp17);*/

                        //Max Factions

                        var updateAtt = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P044_UpdateAttribPts.PacketSt44
                                {
                                        ID1 = 50,
                                        FreePts = 0,
                                        MaxPts = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(updateAtt);

                        /*var ppp26 = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.Packet170.PacketSt170
                                {
                                        ID1 = 50,
                                        Data1 = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(ppp26);*/

                        //Update player data

                        //Update available skills

                        //Note: VERY IMPORTANT
                        var importantMessage = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P147_UpdateGenericValueInt.PacketSt147
                                {
                                        ValueID = 64,
                                        AgentID = 50,
                                        Value = 0
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(importantMessage);

                        var createCharaAck = new NetworkMessage(message.NetID)
                        {
                                PacketTemplate = new ToClient.P380_CreateCharacterAcknowledge.PacketSt380()
                        };
                        QueuingService.PostProcessingQueue.Enqueue(createCharaAck);

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt130> pParser;
        }
}
