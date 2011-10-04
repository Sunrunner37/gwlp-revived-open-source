using ServerEngine.GuildWars.DataWrappers.Clients;

namespace ServerEngine.GuildWars.DataInterfaces
{
        public interface IHasClientData
        {
                AccID AccID { get; set; }
                CharID CharID { get; set; }
        }
}