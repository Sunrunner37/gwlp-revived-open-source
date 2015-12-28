## P09\_ReferToGameServer ##
| **Datatype** | **Field** | **Comment** |
|:-------------|:----------|:------------|
| **UInt16**   | Header    |             |
| **UInt32**   | LoginCount |             |
| **byte[.md](.md)** | SecurityKey1 | _(ConstSize = true, MaxSize = 4)_ |
| **UInt32**   | GameMapID |             |
| **byte[.md](.md)** | ServerConnectionInfo | _(ConstSize = true, MaxSize = 24)_ |
| **byte[.md](.md)** | SecurityKey2 | _(ConstSize = true, MaxSize = 4)_ |