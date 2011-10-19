using System.Linq;
using GameServer.Interfaces;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.GuildWars.DataWrappers.Clients;
using ServerEngine.NetworkManagement;

namespace GameServer.Actions
{
        public class DespawnPlayer : IAction
        {
                private readonly DataCharacter chara;

                public DespawnPlayer(DataCharacter chara)
                {
                        this.chara = chara;
                }

                public void Execute(DataMap map)
                {
                        // send message to all available players
                        // the following linq expression returns an IEnumerable<CharID> of all characters on that map
                        foreach (var charID in map.GetAll<DataCharacter>().Select(x => x.Data.CharID))
                        {
                                if (chara.Data.CharID.Value != charID.Value)
                                {
                                        CreatePackets(chara, charID);
                                }
                        }
                }

                private static void CreatePackets(DataCharacter senderChara, CharID recipientCharID)
                {
                        // get the recipient of all those packets
                        var reNetID = GameServerWorld.Instance.Get<DataClient>(recipientCharID).Data.NetID;

                        // Note: REMOVE PLAYER
                        var despawnAgent = new NetworkMessage(reNetID)
                        {
                                PacketTemplate = new P022_DespawnObject.PacketSt22
                                {
                                        AgentID = senderChara.Data.AgentID.Value,
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(despawnAgent);
                }
        }
}
