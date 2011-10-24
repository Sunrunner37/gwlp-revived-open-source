using System;
using GameServer.Actions;
using GameServer.ServerData;
using ServerEngine.GuildWars.Tools;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 55)]
        public class P055_ClickLocation : IPacket
        {
                public class PacketSt55 : IPacketTemplate
                {
                        public UInt16 Header { get { return 55; } }
                        public Single PosX;
                        public Single PosY;
                        public UInt32 Plane;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt55>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        var pack = new PacketSt55();
                        pParser(pack, message.PacketData);

                        // get the client /map
                        var chara = GameServerWorld.Instance.Get<DataClient>(message.NetID).Character;
                        var map = GameServerWorld.Instance.Get<DataMap>(chara.Data.MapID);
                        
                        // do NOT update the char's values as it is not yet there!

                        // enqueue the action
                        map.Data.ActionQueue.Enqueue(new GotoLocation(chara.Data.CharID, new GWVector(pack.PosX, pack.PosY, (int)pack.Plane)).Execute);

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt55> pParser;
        }
}
