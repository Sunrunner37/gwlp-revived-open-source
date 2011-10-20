namespace GameServer.Enums
{
        public enum ItemStatEnums
        {
                #region CoreStats
                Attribute = 0x2798,
                #endregion

                #region LeadhandStats
                HealthDegeneration = 0x20E8,
                AdditionalArmor = 0x2108,
                AdditionalDamageWhileHealthOverPercent = 0x2278,
                AdditionalHealthLeadHand = 0x2348,
                DamageType = 0x24B8,
                LifeSteal = 0x2528,
                BowType = 0x2618,
                WeaponType = 0x2768, //???

                BaseDamageWithoutReq = 0xA488,
                BaseDamageModifier = 0xA498,
                BaseDamageWithReq = 0xA7A8,
                #endregion

                #region OffhandStats
                AdditionalArmorVsDamageType = 0x2118,
                AdditionalHealthOffHand = 0x6898,
                BaseArmor = 0xA7B8
                #endregion
        }
}