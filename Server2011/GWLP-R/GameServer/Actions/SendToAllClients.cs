using System.Linq;
using GameServer.Interfaces;
using GameServer.Packets.ToClient;
using GameServer.ServerData;
using ServerEngine;
using ServerEngine.GuildWars.DataWrappers.Clients;
using ServerEngine.NetworkManagement;
using GameServer.ServerData.Items;
using ServerEngine.PacketManagement.Definitions;

namespace GameServer.Actions
{
        public class SendToAllClients : IAction
        {
                private readonly IPacketTemplate packet;

                public SendToAllClients(IPacketTemplate packet)
                {
                        this.packet = packet;
                }

                public void Execute(DataMap map)
                {
                        // send message to all available players
                        // the following linq expression returns an IEnumerable<CharID> of all characters on that map
                        foreach (var charID in map.GetAll<DataCharacter>().Select(x => x.Data.CharID))
                        {
                                var reNetID = GameServerWorld.Instance.Get<DataClient>(charID).Data.NetID;

                                var generalPacket = new NetworkMessage(reNetID)
                                {
                                        PacketTemplate = packet
                                };
                                QueuingService.PostProcessingQueue.Enqueue(generalPacket);
                        }
                }
        }
}
