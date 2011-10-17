namespace GameServer.ServerData.Items
{
        public class ItemStats
        {
            private enum CoreStats
            {
                Attribute = 0x2798
            }

            private enum LeadhandStats
            {
                HealthDegeneration = 0x20E8,
                AdditionalArmor = 0x2108,
                AdditionalDamageWhileHealthOverPercent = 0x2278,
                AdditionalHealth = 0x2348,
                DamageType = 0x24B8,
                LifeSteal = 0x2528,
                BowType = 0x2618,
                WeaponType = 0x2768, //???

                BaseDamageWithoutReq = 0xA488,
                BaseDamageModifier = 0xA498,
                BaseDamageWithReq = 0xA7A8,
                
            }

            private enum OffhandStats
            {
                AdditionalArmorVsDamageType = 0x2118,
                AdditionalHealth = 0x6898,
                BaseArmor = 0xA7B8
            }
        }
}