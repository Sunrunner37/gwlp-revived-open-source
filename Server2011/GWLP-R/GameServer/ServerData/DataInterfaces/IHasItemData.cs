using GameServer.ServerData.Items;

namespace GameServer.ServerData.DataInterfaces
{
        public interface IHasItemData
        {
                ItemDictionary Items { get; set; }
        }
}