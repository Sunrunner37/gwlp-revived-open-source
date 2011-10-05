using System.Collections.Generic;

namespace GameServer.ServerData.DataInterfaces
{
        public interface IHasAttributeData
        {
                Dictionary<int, int> Attributes { get; set; }

                int AttPtsFree { get; set; }
                int AttPtsTotal { get; set; }
        }
}