using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer.ServerData
{
        public class MapSpawn
        {
                public int SpawnID { get; set; }
                public float SpawnX { get; set; }
                public float SpawnY { get; set; }
                public int SpawnPlane { get; set; }
                public float SpawnRadius { get; set; }
                public bool IsOutpost { get; set; }
                public bool IsPvE { get; set; }
                public int TeamSpawnNumber { get; set; }
        }
}
