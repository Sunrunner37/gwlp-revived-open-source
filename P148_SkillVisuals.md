## P148\_SkillVisuals ##
| **Datatype** | **Field** | **Comment** |
|:-------------|:----------|:------------|
| **UInt16**   | Header    |             |
| **UInt32**   | PacketUsage | _defines how the packet is used: 0x14 = [VisualID=EffectID on AffectedAgentID like the lightning surge after invoke] , 0x3C = [VisualID=SkillID of the Skill casted by AffectedAgentID applied on AffectedAgentID]_  |
| **UInt32**   | AffectedAgentID |             |
| **UInt32**   | OtherInvolvedAgentID |             |
| **UInt32**   | VisualID  |             |