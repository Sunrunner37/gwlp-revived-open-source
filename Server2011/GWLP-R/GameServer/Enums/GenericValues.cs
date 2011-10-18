
namespace GameServer.Enums
{
        public enum GenericValues
        {
                /// <summary>
                ///   Freezes the player. Values: 
                ///   1-freeze
                ///   0-unfreeze
                /// </summary>
                FreezePlayer = 8,

                /// <summary>
                ///   Shakes the screen. Value:
                ///   1-effect
                /// </summary>
                //Earthquake = 8,

                /// <summary>
                ///   Show skill damage and put player into fight stance. Values:
                ///   SkillID
                /// </summary>
                //SkillDamage = 9,

                /// <summary>
                ///   Show a marker over players head. Values:
                ///   0-exclamation mark
                ///   1-gem
                ///   2-shield&sword
                ///   3-arrow
                ///   4-arrow
                ///   5-exclamation mark
                /// </summary>
                //ShowMarker = 10,

                /// <summary>
                ///   Remove any marker from player. Value:
                ///   0-effect
                /// </summary>
                //RemoveMarker = 11,

                /// <summary>
                ///   Apply any effect to player (auras etc.). Values:
                ///   EffectID
                /// </summary>
                //ApplyEffect1 = 17,

                /// <summary>
                ///   Apply any effect to player (auras etc.). Values:
                ///   EffectID
                /// </summary>
                //ApplyEffect2 = 18,

                /// <summary>
                ///   Apply any effect to player (auras etc.). Values:
                ///   EffectID
                ///   !COULD BE APPLY EFFECT 2!
                /// </summary>
                //ApplyEffect3 = 19,

                /// <summary>
                ///   Show the aion wings. Values:
                ///   12-white
                ///   16-black
                ///   Requires ApplyEffect2/EffectID 0xB9403536
                /// </summary>
                //ShowWings = 22,

                /// <summary>
                ///   Show the /rank animation. Values:
                ///   (3-12)-rank
                ///   Requires ApplyEffect2/EffectID 0xB9403536
                /// </summary>
                //ShowRank = 23,

                /// <summary>
                ///   Show the /zrank animation. Values:
                ///   (1-12)-rank
                ///   Requires ApplyEffect2/EffectID 0xB9403536
                /// </summary>
                //ShowZaishen = 24,

                /// <summary>
                ///   Apply a boss like glow to the player. Values:
                ///   BossGlowColors
                /// </summary>
                //BossGlow = 26,

                /// <summary>
                ///   Knocksdown player. Value:
                ///   1-effect
                /// </summary>
                //Knockdown = 32,

                /// <summary>
                ///   The players publicly visible level. Value:
                ///   (1-?)-level
                /// </summary>
                PublicLevel = 36,

                ///// <summary>
                /////   Applies level up animation. Value:
                /////   1-effect
                ///// </summary>
                //LvlUp = 34,

                ///// <summary>
                /////   Puts player into fight stance. Value:
                /////   1-effect
                ///// </summary>
                //FightStance = 35,

                ///// <summary>
                /////   Pickup animation. Value:
                /////   1-effect
                ///// </summary>
                //PickUp = 36,

                /// <summary>
                ///   Set player energy. Values:
                ///   (0-?)-energy
                /// </summary>
                Energy = 41,

                /// <summary>
                ///   Set player health. Values:
                ///   (1-?)-health
                /// </summary>
                Health = 42,

                /// <summary>
                ///   Set player energy regeneration. Values:
                ///   0,016-reg+1
                ///   0,033-reg+2
                ///   0,066-reg+3
                ///   ... 
                ///   0,33-reg+20 (?)
                /// </summary>
                EnergyRegen = 43,

                /// <summary>
                ///   Set player health regeneration. Values:
                ///   0,016-reg+1
                ///   0,033-reg+2
                ///   0,066-reg+3
                ///   ... 
                ///   0,33-reg+20 (?)
                /// </summary>
                HealthRegen = 44,

                /// <summary>
                ///   Instantly cast a spell, character freezes afterwards. Value:
                ///   1-effect(?)
                /// </summary>
                //CastSpellInstantly = 43,

                /// <summary>
                ///   Interupted skill effect. Value:
                ///   1-effect
                /// </summary>
                //Interupted = 44,

                /// <summary>
                ///   Cast a spell, without animation. Value:
                ///   1-effect(?)
                /// </summary>
                //CastSpell = 45,

                /// <summary>
                ///   Give energy (soul reaping etc.). Value:
                ///   (1-?)-energy(?)
                /// </summary>
                GiveEnergy = 54,

                /// <summary>
                ///   Puts player into fight stance. Value:
                ///   0
                /// </summary>
                FightStance = 58,

                /// <summary>
                ///   Cast a spell like usual. Value:          
                ///   (1-?)-SkillID(?)
                /// </summary>
                CastSpellAnimated = 60,

                /// <summary>
                ///   Change skill activation time. Values:                 
                ///   float CastTimeModifier 0.5 = half casttime
                /// </summary>
                SkillActivation = 61,

                /// <summary>
                ///   Change player energy. Values:                 
                ///   float +- energy in percent
                /// </summary>
                EnergyChange = 62,

        }
}
