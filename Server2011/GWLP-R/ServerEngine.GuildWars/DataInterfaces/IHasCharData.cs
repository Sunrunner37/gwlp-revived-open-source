using ServerEngine.GuildWars.DataWrappers.Chars;

namespace ServerEngine.GuildWars.DataInterfaces
{
        public interface IHasCharData
        {
                AgentID AgentID { get; set; }
                LocalID LocalID { get; set; }
                Name Name { get; set; }
        }
}