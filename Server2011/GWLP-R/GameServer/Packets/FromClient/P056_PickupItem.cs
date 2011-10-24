using System;
using GameServer.Actions;
using GameServer.ServerData.Items;
using ServerEngine.NetworkManagement;
using ServerEngine.PacketManagement.CustomAttributes;
using ServerEngine.PacketManagement.Definitions;
using GameServer.ServerData;
using GameServer.Enums;

namespace GameServer.Packets.FromClient
{
        [PacketAttributes(IsIncoming = true, Header = 56)]
        public class P056_PickupItem : IPacket
        {
                public class PacketSt56 : IPacketTemplate
                {
                        public UInt16 Header { get { return 56; } }
                        public UInt32 AgentID; //was ID!!
                        public byte Flag;//0
                }

                public void InitPacket(object parser)
                {
                        pParser = (PacketParser<PacketSt56>)parser;
                        IsInitialized = true;
                        IsInUse = false;
                }

                public bool Handler(ref NetworkMessage message)
                {
                        // parse the message
                        var pack = new PacketSt56();
                        pParser(pack, message.PacketData);

                        var chara = GameServerWorld.Instance.Get<DataClient>(message.NetID).Character;
                        var map = GameServerWorld.Instance.Get<DataMap>(chara.Data.MapID);

                        Item item;
                        int itemLocalID = (int) pack.AgentID;
                        
                        if (!map.Data.MapItems.TryGetValue(itemLocalID, out item)) return true; // someone was first

                        // TODO: check if can be picked up by this chara
                        ItemStorage storage;
                        byte slot;
                        if (!chara.Data.Items.GetFirstFreeSlot(out storage, out slot)) return true; // no inventory space

                        if (!map.Data.MapItems.Remove(itemLocalID)) return true; // someone was first

                        // Add item to charas inventory
                        item.Data.OwnerAccID = chara.Data.AccID;
                        item.Data.OwnerCharID = chara.Data.CharID;
                        item.Data.Storage = storage;
                        item.Data.Slot = slot;
                        chara.Data.Items.AddSave(itemLocalID, item);
                        item.SendLocation(chara.Data.NetID);

                        // remove item from ground
                        map.Data.ActionQueue.Enqueue(new DespawnItem(item).Execute);

                        return true;
                }

                public bool IsInitialized { get; set; }

                public bool IsInUse { get; set; }

                private PacketParser<PacketSt56> pParser;
        }
}
