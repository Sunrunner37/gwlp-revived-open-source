namespace LoginServer.ServerData.DataInterfaces
{
        public interface IHasEncryptionData
        {
                byte[] EncryptionSeed { get; set; }
        }
}