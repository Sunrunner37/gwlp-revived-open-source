using System;

namespace GameServer.Enums
{
        [Flags]
        public enum VitalStatus : uint
        {
                // First byte
                /// <summary>
                ///   Sets the char "alive"
                /// </summary>
                Alive = 0,

                /// <summary>
                ///   Shows the enchantment indicator
                /// </summary>
                Enchanted = 0x00000080,

                /// <summary>
                ///   Shows the poisoned health bar
                /// </summary>
                Poisoned = 0x00000040,

                /// <summary>
                ///   Shows the deep wound health bar
                /// </summary>
                Wounded = 0x00000020,

                /// <summary>
                ///   Sets the char "dead"
                /// </summary>
                Dead = 0x00000010,

                /// <summary>
                ///   Cripples the char
                /// </summary>
                Crippled = 0x00000008,

                /// <summary>
                ///   Indicator for exploited corpse
                /// </summary>
                ExploitedCorpse = 0x00000004,

                /// <summary>
                ///   Shows condition indicator
                /// </summary>
                Condition = 0x00000002,

                /// <summary>
                ///   Shows the bleeding health bar
                /// </summary>
                Bleeding = 0x00000001,

                // Second byte
                //HPBarLeaf = 0x00008000, // doesnt work somehow

                /// <summary>
                ///   Forces the char to sit, e.g. if drunk
                /// </summary>
                DrunkSit = 0x00002000,

                /// <summary>
                ///   Forces the char to jump
                /// </summary>
                ForceJump = 0x00001000,

                /// <summary>
                ///   Shows the hex indicator
                /// </summary>
                Hexed = 0x00000800,

                /// <summary>
                ///   Shows the hexed health bar
                /// </summary>
                HexedHealthBar = 0x00000400,

                /// <summary>
                ///   Shows a game master sign
                /// </summary>
                GameMaster = 0x00000200,

                /// <summary>
                ///   Freezes a player's movement
                /// </summary>
                FreezePlayer = 0x00000100,
        }
}
