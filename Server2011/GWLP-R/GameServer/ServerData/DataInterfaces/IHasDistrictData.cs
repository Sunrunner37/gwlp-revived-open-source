using System.Collections.Generic;
using GameServer.ServerData.Items;

namespace GameServer.ServerData.DataInterfaces
{
        public interface IHasDistrictData
        {
                bool IsOutpost { get; set; }
                bool IsPvE { get; set; }
                int DistrictCountry { get; set; }
                int DistrictNumber { get; set; }
        }
}