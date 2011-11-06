namespace GameServer.Enums
{
        public enum ItemFlagEnums
        {
                Identifiable = 0x1,
                ArmorPlayerOrMonster = 0x2,
                ArmorPlayer = 0x4,
                // missing 4
                RarityGreen = 0x10,
                CommonMaterial = 0x20,
                DropsFromMonster = 0x40,
                // missing 8
                Untradable = 0x100,
                Undyeable = 0x200,
                // missing 11
                // missing 12
                // missing 13
                // missing 14
                FixedPrefix = 0x4000,
                FixedSuffix = 0x8000,
                HasPrefix = 0x10000,
                RarityGold = 0x20000,
                // missing 19
                UncustomizedObject = 0x80000,
                HasSuffix = 0x100000,
                TwoHanded = 0x200000,
                RarityPurple = 0x400000,
                Unidentified = 0x800000,
                Usable = 0x1000000,
                Leadhand = 0x2000000,
                HasInscription = 0x4000000,
                Inscriptable = 0x8000000,
                BonusWeapon = 0x10000000,
                AlwaysSet = 0x20000000
                // missing 31
                // missing 32
        }
}