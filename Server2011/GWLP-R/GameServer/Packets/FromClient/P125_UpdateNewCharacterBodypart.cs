using System;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.GuildWars.Tools;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 125)]
        public class P125_UpdateNewCharacterBodypart : IPacket
        {
                public class PacketSt125 : IPacketTemplate
                {
                        public UInt16 Header { get { return 125; } }
                        public byte EquipmentSlot;
                        public byte Color;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt125>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        var pack = new P125_UpdateNewCharacterBodypart.PacketSt125();
                        pParser(pack, message.PacketData);

                        //equip char here

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt125> pParser;
        }
}
