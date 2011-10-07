using System.Linq;
using GameServer.Interfaces;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.GuildWars.DataWrappers.Clients;
using ServerEngine.NetworkManagement;

namespace GameServer.Actions
{
        public class RotatePlayer : IAction
        {
                private readonly CharID newCharID;

                public RotatePlayer(CharID charID)
                {
                        newCharID = charID;
                }

                public void Execute(DataMap map)
                {
                        // send message to all available players
                        // the following linq expression returns an IEnumerable<CharID> of all characters on that map
                        foreach (var charID in map.GetAll<DataCharacter>().Select(x => x.Data.CharID))
                        {
                                CreatePackets(newCharID, charID);
                        }
                }

                private static void CreatePackets(CharID senderCharID, CharID recipientCharID)
                {
                        var chara = GameServerWorld.Instance.Get<DataClient>(senderCharID).Character;

                        // get the recipient of all those packets
                        var reNetID = recipientCharID.Value != senderCharID.Value ?
                                GameServerWorld.Instance.Get<DataClient>(recipientCharID).Data.NetID :
                                chara.Data.NetID;

                        // Note: ROTATE AGENT
                        var rotAgent = new NetworkMessage(reNetID)
                        {
                                PacketTemplate = new P035_RotateAgent.PacketSt35
                                {
                                        AgentID = (ushort)chara.Data.AgentID.Value,
                                        Rotation = chara.Data.Rotation,
                                        Data1 = 0x40060A92
                                }
                        };
                        QueuingService.PostProcessingQueue.Enqueue(rotAgent);
                }
        }
}
