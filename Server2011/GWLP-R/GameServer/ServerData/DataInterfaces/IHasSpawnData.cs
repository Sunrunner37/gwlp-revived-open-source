using System.Collections.Generic;
using ServerEngine.GuildWars.Tools;

namespace GameServer.ServerData.DataInterfaces
{
        public interface IHasSpawnData
        {
                List<GWVector> PossibleSpawns { get; set; }
        }
}