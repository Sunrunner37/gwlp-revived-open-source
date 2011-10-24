using System.Linq;
using GameServer.Interfaces;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.GuildWars.DataWrappers.Clients;
using ServerEngine.NetworkManagement;
using GameServer.ServerData.Items;

namespace GameServer.Actions
{
        public class DespawnItem : IAction
        {
                private readonly Item item;

                public DespawnItem(Item item)
                {
                        this.item = item;
                }

                public void Execute(DataMap map)
                {
                        // send message to all available players
                        // the following linq expression returns an IEnumerable<CharID> of all characters on that map
                        foreach (var charID in map.GetAll<DataCharacter>().Select(x => x.Data.CharID))
                        {
                                CreatePackets(item, charID);
                        }
                }

                private static void CreatePackets(Item item, CharID recipientCharID)
                {
                        // get the recipient of all those packets
                        var reNetID = GameServerWorld.Instance.Get<DataClient>(recipientCharID).Data.NetID;

                        // Note: REMOVE ITEM
                        var despawnItem = new NetworkMessage(reNetID)
                        {
                                PacketTemplate = new P022_DespawnObject.PacketSt22
                                {
                                        AgentID = (uint)item.Data.ItemLocalID,
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(despawnItem);
                }
        }
}
