using System;
using GameServer.Enums;
using GameServer.ServerData;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;
using ServerEngine.GuildWars.Tools;

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
                        var pack = new PacketSt54();
                        pParser(pack, message.PacketData);

                        // get the client
                        var chara = GameServerWorld.Instance.Get<DataClient>(message.NetID).Character;
                        
                        // update position
                        chara.Data.Position = new GWVector(pack.X, pack.Y, (int)pack.Plane);

                        // update direction
                        chara.Data.Direction = new GWVector(pack.DirX, pack.DirY, 0).UnitVector;

                        // update movement type
                        chara.Data.MoveType = (MovementType)Enum.ToObject(typeof(MovementType), pack.Type);

                        // update the movement status, because the client might have change movement direction
                        chara.Data.MoveState = MovementState.MoveChangeDir;
                        

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt54> pParser;
        }
}
