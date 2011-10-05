namespace GameServer.ServerData.DataInterfaces
{
        public interface IHasDistrictData
        {
                bool IsOutpost { get; set; }
                bool IsPvP { get; set; }
                int DistrictCountry { get; set; }
                int DistrictNumber { get; set; }
        }
}