namespace LoginServer.ServerData.DataInterfaces
{
        public interface IHasAccountData
        {
                string Email { get; set; }
                string Password { get; set; }
        }
}