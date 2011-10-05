namespace GameServer.ServerData.DataInterfaces
{
        public interface IHasEncryptionData
        {
                byte[] EncryptionSeed { get; set; }
        }
}