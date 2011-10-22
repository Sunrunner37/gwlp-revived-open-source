using System.Collections.Generic;
using GameServer.ServerData.Items;

namespace GameServer.ServerData.DataInterfaces
{
        public interface IHasMapItemsData
        {
                Dictionary<int, Item> MapItems { get; set; }
        }
}