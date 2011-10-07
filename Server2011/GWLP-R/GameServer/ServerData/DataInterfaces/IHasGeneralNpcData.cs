namespace GameServer.ServerData.DataInterfaces
{
        public interface IHasGeneralNpcData
        {
                int NpcFileID { get; set; }
                byte[] ModelHash { get; set; }
                int NpcFlags { get; set; }
                int Scale { get; set; }
        }
}