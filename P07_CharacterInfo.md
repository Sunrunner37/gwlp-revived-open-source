## P07\_CharacterInfo ##
| **Datatype** | **Field** | **Comment** |
|:-------------|:----------|:------------|
| **UInt16**   | Header    |             |
| **UInt32**   | LoginCount |             |
| **byte[.md](.md)** | StaticHash1 | _(ConstSize = true, MaxSize = 16)_ |
| **UInt32**   | StaticData1 |             |
| **string**   | CharName  | _(ConstSize = false, MaxSize = 20)_ |
| **UInt16**   | ArraySize1 |             |
| **byte[.md](.md)** | Appearance | _(ConstSize = false, MaxSize = 64)_ |