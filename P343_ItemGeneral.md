## P343\_ItemGeneral ##
| **Datatype** | **Field** | **Comment** |
|:-------------|:----------|:------------|
| **UInt16**   | Header    |             |
| **UInt32**   | LocalID   |             |
| **UInt32**   | FileID    | _3d model file_  |
| **byte**     | ItemType  | _sword 1b, axe 2_  |
| **byte**     | Data2     | _sword 3, axe 6_  |
| **UInt16**   | DyeColor  |             |
| **UInt16**   | Data4     | _Standard Color?_  |
| **byte**     | CanBeDyed |             |
| **UInt32**   | Flags     |             |
| **UInt32**   | MerchantPrice |             |
| **UInt32**   | ItemID    | _sword 46C2, axe 45D4 icon?_  |
| **UInt32**   | Quantity  |             |
| **string**   | NameHash  | _(ConstSize = false, MaxSize = 64)_ |
| **byte**     | NumStats  |             |
| **UInt32[.md](.md)** | Stats     | _(ConstSize = false, MaxSize = 256)_ |