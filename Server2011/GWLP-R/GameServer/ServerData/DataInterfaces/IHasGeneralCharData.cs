namespace GameServer.ServerData.DataInterfaces
{
        public interface IHasGeneralCharData
        {
                byte ProfessionPrimary { get; set; }
                byte ProfessionSecondary { get; set; }

                int Level { get; set; }

                int Morale { get; set; }
        }
}