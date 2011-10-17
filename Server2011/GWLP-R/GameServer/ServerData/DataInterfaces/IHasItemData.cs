using GameServer.ServerData.Items;

namespace GameServer.ServerData.DataInterfaces
{
        public interface IHasItemData
        {
                DataInventory Inventory { get; set; }
                DataStorage Storage { get; set; }
        }
}