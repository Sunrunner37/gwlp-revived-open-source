## P074\_NpcGeneralStats ##
| **Datatype** | **Field** | **Comment** |
|:-------------|:----------|:------------|
| **UInt16**   | Header    |             |
| **UInt32**   | NpcID     |             |
| **UInt32**   | FileID    |             |
| **UInt32**   | Data1     |             |
| **UInt32**   | Scale     |             |
| **UInt32**   | Data2     |             |
| **UInt32**   | ProfessionFlags | _bitfield: 0-15 profession stuff, 16 show name byte Profession_  |
| **byte**     | Level     |             |
| **UInt16**   | ArraySize1 |             |
| **byte[.md](.md)** | Appearance | _(ConstSize = false, MaxSize = 8)_ |