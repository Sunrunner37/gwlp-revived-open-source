using System;
using GameServer.Actions;
using GameServer.Enums;
using GameServer.ServerData;
using ServerEngine.ProcessorQueues;
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
                        message.PacketTemplate = new PacketSt57();
                        pParser((PacketSt57)message.PacketTemplate, message.PacketData);

                        var chara = World.GetCharacter(Chars.NetID, message.NetID);
                        
                        chara.CharStats.Rotation = ((PacketSt57)message.PacketTemplate).Rotation;

                        if (((PacketSt57) message.PacketTemplate).Rotation == float.PositiveInfinity)
                        {
                                chara.CharStats.IsRotating = true;
                        }
                        else
                        {
                                chara.CharStats.IsRotating = false;
                        }

                        var action = new RotatePlayer((int) chara[Chars.CharID]);
                        World.GetMap(Maps.MapID, chara.MapID).ActionQueue.Enqueue(action.Execute);
                        

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt57> pParser;
        }
}
