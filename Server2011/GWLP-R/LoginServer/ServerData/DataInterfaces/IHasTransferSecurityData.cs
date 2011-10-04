namespace LoginServer.ServerData.DataInterfaces
{
        public interface IHasTransferSecurityData
        {
                byte[][] SecurityKeys { get; set; }
        }
}