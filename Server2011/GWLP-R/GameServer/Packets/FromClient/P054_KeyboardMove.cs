using System;
using GameServer.Enums;
using GameServer.ServerData;
using ServerEngine.ProcessorQueues;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;
using ServerEngine.Tools;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 54)]
        public class P054_KeyboardMove : IPacket
        {
                public class PacketSt54 : IPacketTemplate
                {
                        public UInt16 Header { get { return 54; } }
                        public Single X;
                        public Single Y;
                        public UInt32 Plane;
                        public Single DirX;
                        public Single DirY;
                        public UInt32 Type;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt54>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        message.PacketTemplate = new PacketSt54();
                        pParser((PacketSt54)message.PacketTemplate, message.PacketData);

                        var chara = World.GetCharacter(Chars.NetID, message.NetID);
                        
                        chara.CharStats.Position = new GWVector(
                                ((PacketSt54) message.PacketTemplate).X,
                                ((PacketSt54) message.PacketTemplate).Y,
                                (int)((PacketSt54) message.PacketTemplate).Plane);

                        var dir = new GWVector(
                                ((PacketSt54)message.PacketTemplate).DirX,
                                ((PacketSt54)message.PacketTemplate).DirY,
                                0);

                        chara.CharStats.Direction = dir.UnitVector;

                        chara.CharStats.MoveType = (int)((PacketSt54) message.PacketTemplate).Type;

                        chara.CharStats.MoveState = MovementState.MoveChangeDir;
                        

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt54> pParser;
        }
}
