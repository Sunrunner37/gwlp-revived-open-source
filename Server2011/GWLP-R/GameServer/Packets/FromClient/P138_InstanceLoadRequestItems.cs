using System;
using System.IO;
using GameServer.Enums;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 138)]
        public class P138_InstanceLoadRequestItems : IPacket
        {
                public class PacketSt138 : IPacketTemplate
                {
                        public UInt16 Header { get { return 138; } }
                        public byte Data1;
                        public byte Data2;
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt138>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        message.PacketTemplate = new PacketSt138();
                        pParser((PacketSt138)message.PacketTemplate, message.PacketData);

#warning FIXME: Item stuff is not implemented!
                        ushort itemStreamID = 1;

                        // Note: ITEM STREAM HEAD1 
                        var head1 = new NetworkMessage(message.NetID);
                        head1.PacketTemplate = new P314_ItemStreamHead1.PacketSt314();
                        ((P314_ItemStreamHead1.PacketSt314)head1.PacketTemplate).Data1 = itemStreamID;
                        ((P314_ItemStreamHead1.PacketSt314)head1.PacketTemplate).Data2 = 0;
                        QueuingService.PostProcessingQueue.Enqueue(head1);

                        // Note: ITEM STREAM HEAD2
                        var head2 = new NetworkMessage(message.NetID);
                        head2.PacketTemplate = new P318_ItemStreamHead2.PacketSt318();
                        ((P318_ItemStreamHead2.PacketSt318)head2.PacketTemplate).Data1 = itemStreamID;
                        ((P318_ItemStreamHead2.PacketSt318)head2.PacketTemplate).Data2 = 0;
                        QueuingService.PostProcessingQueue.Enqueue(head2);

                        //// Note: ITEM STREAM WEAPON BAR SLOTS
                        //for (byte i = 0; i < 4; i++)
                        //{
                        //        var wbSlot = new NetworkMessage(message.NetID);
                        //        wbSlot.PacketTemplate = new P317_ItemStreamWeaponBarSlot.PacketSt317();
                        //        ((P317_ItemStreamWeaponBarSlot.PacketSt317)wbSlot.PacketTemplate).ItemStreamID = itemStreamID;
                        //        ((P317_ItemStreamWeaponBarSlot.PacketSt317)wbSlot.PacketTemplate).SlotNumber = i;
                        //        ((P317_ItemStreamWeaponBarSlot.PacketSt317)wbSlot.PacketTemplate).Data3 = 0;
                        //        ((P317_ItemStreamWeaponBarSlot.PacketSt317)wbSlot.PacketTemplate).Data4 = 0;
                        //        QueuingService.PostProcessingQueue.Enqueue(wbSlot);
                        //}

                        //// Note: ITEM STREAM END
                        //var end = new NetworkMessage(message.NetID);
                        //end.PacketTemplate = new P311_ItemStreamEnd.PacketSt311();
                        //((P311_ItemStreamEnd.PacketSt311)end.PacketTemplate).Data1 = itemStreamID;
                        //((P311_ItemStreamEnd.PacketSt311)end.PacketTemplate).GameMapID = 0;
                        //QueuingService.PostProcessingQueue.Enqueue(end);

                        var chara = World.GetCharacter(Chars.NetID, message.NetID);
                        
                        // Note: ITEM STREAM Terminator
                        var terminator = new NetworkMessage(message.NetID);
                        terminator.PacketTemplate = new P393_ItemStreamTerminator.PacketSt393();
                        // only works as terminator when this is 0
                        ((P393_ItemStreamTerminator.PacketSt393)terminator.PacketTemplate).Data1 = 0;
                        ((P393_ItemStreamTerminator.PacketSt393)terminator.PacketTemplate).GameMapID = (ushort)(int)World.GetMap(Maps.MapID, chara.MapID)[Maps.GameMapID];
                        ((P393_ItemStreamTerminator.PacketSt393)terminator.PacketTemplate).Data3 = 0;
                        QueuingService.PostProcessingQueue.Enqueue(terminator);
                        

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt138> pParser;
        }
}
