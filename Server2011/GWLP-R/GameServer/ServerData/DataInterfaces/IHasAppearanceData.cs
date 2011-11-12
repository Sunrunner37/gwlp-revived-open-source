namespace GameServer.ServerData.DataInterfaces
{
        public interface IHasAppearanceData
        {
                byte[] Appearance { get; set; }
                byte LookHeight { get; set; }
                byte LookSex { get; set; }
                byte LookFace { get; set; }
                byte LookHairstyle { get; set; }
                byte LookHaircolor { get; set; }
                byte LookCampaign { get; set; }
                byte LookSkinColor { get; set; }
                byte LookProfession { get; set; }
                byte LookShowHelm { get; set; }
        }
}