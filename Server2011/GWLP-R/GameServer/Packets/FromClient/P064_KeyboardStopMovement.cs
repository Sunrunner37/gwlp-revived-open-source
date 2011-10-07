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
                        var pack = new PacketSt64();
                        pParser(pack, message.PacketData);

                        // get the character
                        var chara = GameServerWorld.Instance.Get<DataClient>(message.NetID).Character;
                        
                        // update the position of it
                        chara.Data.Position = new GWVector(pack.X, pack.Y, (int)pack.Plane);

                        // update the movestate: the following will let Movement send a packet to all clients
                        // that this client has stopped moving
                        chara.Data.MoveState = MovementState.NotMovingUnhandled;
                        chara.Data.MoveType = MovementType.Stop;

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt64> pParser;
        }
}
