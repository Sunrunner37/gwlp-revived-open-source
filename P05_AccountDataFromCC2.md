## P05\_AccountDataFromCC2 ##
| **Datatype** | **Field** | **Comment** |
|:-------------|:----------|:------------|
| **UInt16**   | Header    |             |
| **UInt32**   | LoginCount |             |
| **UInt32**   | Data2     |             |
| **byte[.md](.md)** | Data3     | _(ConstSize = true, MaxSize = 20)_ |
| **byte[.md](.md)** | Data4     | _(ConstSize = true, MaxSize = 20)_ |
| **string**   | Data5     | _(ConstSize = false, MaxSize = 64)_ |