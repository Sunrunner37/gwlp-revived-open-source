using ServerEngine;

namespace GameServer.ServerData.DataInterfaces
{
        public interface IHasObjectIDManagerData
        {
                IDManager AgentIDs { get; }
                IDManager LocalIDs { get; }
        }
}