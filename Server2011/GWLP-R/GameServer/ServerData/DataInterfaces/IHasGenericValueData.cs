namespace GameServer.ServerData.DataInterfaces
{
        public interface IHasGenericValueData
        {
                bool IsFrozen { get; set; }
                int Energy { get; set; }
                float EnergyRegen { get; set; }
                int Health { get; set; }
                float HealthRegen { get; set; }
        }
}