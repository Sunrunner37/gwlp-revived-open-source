using ServerEngine.GuildWars.DataWrappers.Maps;

namespace ServerEngine.GuildWars.DataInterfaces
{
        public interface IHasMapData
        {
                GameFileID GameFileID { get; set; }
                GameMapID GameMapID { get; set; }
                MapID MapID { get; set; }
        }
}