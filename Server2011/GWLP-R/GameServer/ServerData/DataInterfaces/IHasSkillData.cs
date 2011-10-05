namespace GameServer.ServerData.DataInterfaces
{
        public interface IHasSkillData
        {
                int SkillPtsFree { get; set; }
                int SkillPtsTotal { get; set; }

                byte[] SkillBar { get; set; }
                byte[] UnlockedSkills { get; set; }
        }
}