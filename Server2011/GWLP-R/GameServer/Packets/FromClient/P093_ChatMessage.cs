using System;
using GameServer.Actions;
using GameServer.Enums;
using GameServer.ServerData;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 93)]
        public class P093_ChatMessage : IPacket
        {
                public class PacketSt93 : IPacketTemplate
                {
                        public UInt16 Header { get { return 93; } }
                        [PacketFieldType(ConstSize = false, MaxSize = 138)]
                        public string Message;
                        public UInt32 ID1;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt93>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        var pack = new PacketSt93();
                        pParser(pack, message.PacketData);

                        // get the character
                        var chara = GameServerWorld.Instance.Get<DataClient>(message.NetID).Character;
                        
                        // enqueue the chat message as an action
                        var action = new ChatMessage(chara.Data.CharID, pack.Message);
                        GameServerWorld.Instance.Get<DataMap>(chara.Data.MapID).Data.ActionQueue.Enqueue(action.Execute);
                        
                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt93> pParser;
        }
}
