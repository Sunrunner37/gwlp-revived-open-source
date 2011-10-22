namespace GameServer.Enums
{
        public enum ItemStatEnums
        {
                #region CoreStats
                Attribute = 0x2798,
                AttributeBoost = 0x21F8,
                AdditionalEnergy = 0x62C8,
                BaseArmorArmor = 0xA3C8,
                EnergyRegeneration = 0x62E8,
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
                BaseArmor = 0xA7B8,
                Slots = 0x2448,
                #endregion

                UnknownInDye1 = 0x24D8,
                UnknownInDye2 = 0x24E8

        }
}