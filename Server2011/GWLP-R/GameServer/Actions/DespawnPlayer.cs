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
                private readonly CharID newCharID;

                public DespawnPlayer(CharID charID)
                {
                        newCharID = charID;
                }

                public void Execute(DataMap map)
                {
                        // send message to all available players
                        // the following linq expression returns an IEnumerable<CharID> of all characters on that map
                        foreach (var charID in map.GetAll<DataCharacter>().Select(x => x.Data.CharID))
                        {
                                if (newCharID.Value != charID.Value)
                                {
                                        CreatePackets(newCharID, charID);
                                }
                        }
                }

                private static void CreatePackets(CharID senderCharID, CharID recipientCharID)
                {
                        var chara = GameServerWorld.Instance.Get<DataClient>(senderCharID).Character;

                        // get the recipient of all those packets
                        var reNetID = GameServerWorld.Instance.Get<DataClient>(recipientCharID).Data.NetID;

                        // Note: REMOVE PLAYER
                        var despawnAgent = new NetworkMessage(chara.Data.NetID)
                        {
                                PacketTemplate = new P022_DespawnObject.PacketSt22
                                {
                                        AgentID = chara.Data.AgentID.Value,
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(despawnAgent);
                }
        }
}
