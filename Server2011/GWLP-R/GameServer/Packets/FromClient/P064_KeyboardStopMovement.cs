using System;
using GameServer.Enums;
using GameServer.ServerData;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;
using ServerEngine.GuildWars.Tools;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 64)]
        public class P064_KeyboardStopMovement : IPacket
        {
                public class PacketSt64 : IPacketTemplate
                {
                        public UInt16 Header { get { return 64; } }
                        public Single X;
                        public Single Y;
                        public UInt32 Plane;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt64>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        message.PacketTemplate = new PacketSt64();
                        pParser((PacketSt64)message.PacketTemplate, message.PacketData);

                        var chara = World.GetCharacter(Chars.NetID, message.NetID);
                        
                        chara.CharStats.Position = new GWVector(
                                ((PacketSt64)message.PacketTemplate).X,
                                ((PacketSt64)message.PacketTemplate).Y,
                                (int)((PacketSt64)message.PacketTemplate).Plane);

                        chara.CharStats.MoveState = MovementState.NotMovingUnhandled;

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt64> pParser;
        }
}
