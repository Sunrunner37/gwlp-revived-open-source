using System;
using GameServer.Actions;
using GameServer.Enums;
using GameServer.ServerData;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 57)]
        public class P057_KeyboardRotate : IPacket
        {
                public class PacketSt57 : IPacketTemplate
                {
                        public UInt16 Header { get { return 57; } }
                        public Single Rotation;
                        public Single Data1;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt57>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        var pack = new PacketSt57();
                        pParser(pack, message.PacketData);

                        // get the character
                        var chara = GameServerWorld.Instance.Get<DataClient>(message.NetID).Character;
                        
                        // update the rotation
                        chara.Data.Rotation = pack.Rotation;

                        // check whether the client has started or stopped rotating
                        chara.Data.IsRotating = pack.Rotation == float.PositiveInfinity;

                        // create a new action and add it to the chars map-actionqueue
                        var action = new RotatePlayer(chara.Data.CharID);
                        GameServerWorld.Instance.Get<DataMap>(chara.Data.MapID).Data.ActionQueue.Enqueue(action.Execute);
                        
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt57> pParser;
        }
}
