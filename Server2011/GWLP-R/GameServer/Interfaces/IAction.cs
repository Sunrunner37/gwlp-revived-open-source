using GameServer.ServerData;

namespace GameServer.Interfaces
{
        public interface IAction
        {
                void Execute(DataMap map);
        }
}
