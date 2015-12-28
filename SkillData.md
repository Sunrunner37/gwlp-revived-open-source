# Introduction #


---

**Note that this was a suggestion and we'll probably use a scripting language like IronRuby or C# directly**

---


We'll use a skill system based on an XAML structure,
see this:
(...keep this up to date so we have a valid example)
```
<Skill Id="99" Name="ParasiticBond" Type="SpellHex" Attribute="NecromancerCurses" CastTime="1000" Recharge="2000" RequiredTarget="Foes"
       xmlns="xaml-SkillEngine" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Skill.Effects>
        <!--Cost (doesnt need an extra section)-->
        <EnergyModification Target="Self" Trigger="OnCast" ValueType="Static" StaticValue="5"/>
        <!--Main effects-->
        <HealthRecharge Target="Selected" Trigger="OnCast" ValueType="Static" StaticValue="-1" Duration="20000">
            <HealthModification Target="Self" Trigger="OnBaseEnd" ValueType="Variable" BaseValue="30" MaxValue="120"/>
        </HealthRecharge>
    </Skill.Effects>
</Skill>
```

Keep in mind:
  * We want it to be easy enough to quickly change the skills.
  * We want it to be complete enough to support a huge amount of skills, with a minimum of code.
  * We want it to be as clean as possible. Remember: scalability.

<br />
## Skill data anatomy ##

### Harboe's suggestions ###

```
<skill id="x" name="Backfire" recharge="20000" casttime="3000" type="SPELL" subtype="HEX" attribute="DOMINATION_MAGIC">
        <usage>
                <energy>15</energy>
        </usage>
        <targetting>
                <foe /> <!-- could also have <ally />, <friendly />, whatever -->
        </targetting>
        <effects>
                <hex trigger="CASTSPELL" duration="10000">
                        <dmg base="35" max="140" />
                </hex>
        </effects>
</skill>
 
<skill id="x" name="Immolate" recharge="5000" casttime="1000" type="SPELL" attribute="FIRE_MAGIC"> <!-- notice how there's no subtype -->
        <usage>
                <energy>10</energy>
        </usage>
        <targetting>
                <foe />
        </targetting>
        <effects>
                <dmg base="20" max="75" />
                <burn base="1" max="3" /> <!-- don't need to specify duration as BurnEffect uses the attributes base and max to calculate its duration -->
        </effects>
</skill>
```

### XAML by Fealty ###

```
<Skill Id="99" Name="Parasitic Bond" Type="Spell" Subtype="Hex" Attribute="7" CastTime="1000" Recharge="2000">
  <Skill.Cost>
    <Energy>5</Energy>
  </Skill.Cost>
  <Skill.Effects>
    <HpDegen Target="Foe" Rate="-1" Duration="0:0:20">
      <HpGain Target="Self" Trigger="ParentEnd" Base="30" Max="120" />
    </HpDegen>
  </Skill.Effects>
</Skill>
```

<br />


## Skill: ##
  * Id
  * Name
  * Type
  * Attribute
  * CastTime
  * Recharge
  * RequierdTarget
  * CastRange
<br />
## Skill.Conditions: ##
  * Condition //hex remove or whatev
  * PvP
  * PvE
  * Energy
  * CanMove
  * MinAttribute
<br />
## Skill.Effects: ##
  * Enchant //like Protective Bond so you can stop it
  * useCorp
  * Teleport
  * DeepWound
  * Weakness
  * Cripple
  * Snare
  * Blind
  * Block
  * Evade
  * Knockback
  * KnockDown
  * CostReduction
  * onDrop
  * onEnd
  * Disarm
  * AoE
  * Exhaust
  * SoftInterrupt //on hit interrupt
  * HardInterrupt
  * StatChange //armor/attributes/health/energy
  * Trap
  * Spirit
  * HealthRegen //positiv or negativ including bleed,burn,etc
  * HealthDrain
  * EnergyRegen //positiv or negativ
  * EnergyDrain
  * Rezz
<br />



## Class:Skill Fields ##
  * SkillID
  * Name
  * Type
  * Attribute
  * Cast
  * Recharge
  * RequiredTarget

<br />
## Class:SkillEffect Fields ##
  * Target: Enum, could be "Self" or "Selected"
  * Trigger: Enum, could have various things such a "OnCast", "OnBaseEffectEnd"
  * ValueType: Enum, "Static" or "Variable"
  * StaticValue: some kind of value, used when ValueType == Static
  * BaseValue: some kind of value, at attribute lvl 0, used when ValueType == Variable
  * MaxValue: some kind of value, at attribute lvl 15, used when ValueType == Variable

<br />
## OBSOLETE: ##
  * SkillID
  * Name
  * Profession
  * Attribute
  * Type (->[1](http://www.guildwiki.org/Skill_type), [2](http://www.guildwiki.org/Skill_prefixes_and_suffixes), determines functionality)
  * IsElite
  * IsPvP
  * SkillTargetFlags (self or group or allies or foes, e.g. Energy Tap: foes)
  * EffectTargetFlags (self or group or allies or foes, e.g. Energy Tap: self)
  * ReqWeaponType (bows, daggers etc.)
  * CostEnergy
  * CostAdrenalin
  * CostHealth
  * CostUpkeep
  * Exhaustion
  * TimeCast
  * TimeAfterCast
  * TimeRecharge
  * DurationMin
  * DurationMax
  * DamageType
  * DamageModifier (changes dmg dealt apart from skill)
  * DamageMin
  * DamageMax
  * DamageBonusMin (?)
  * DamageBonusMax (?)
  * HealMin
  * HealMax
  * HealBonusMin
  * HealBonusMax
  * Range
  * AoeSelf (->[3](http://www.guildwiki.org/Range), following in order, e.g. Shockwave)
  * AoeAdjacentFoe
  * AoeAdjacentMelee
  * AoeNearby
  * AoeArea
  * AoeHalf
  * AoeEarshot
  * AoeShortbow
  * AoeRecurvebow
  * AoeLongbow
  * AoeSpiritEffect
  * AoeCompass
  * VitalStatFlags (conditions, hexes)
  * AdditionalScript (in the db or local?)
  * (...?)