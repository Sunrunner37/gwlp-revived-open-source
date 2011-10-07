using GameServer.Enums;

namespace GameServer.ServerData.DataInterfaces
{
        public interface IHasPlayStatusData
        {
                PlayStatus Player { get; set; }
        }
}