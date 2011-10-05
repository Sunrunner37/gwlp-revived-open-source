namespace GameServer.ServerData.DataInterfaces
{
        public interface IHasTransferSecurityData
        {
                byte[][] SecurityKeys { get; set; }
        }
}