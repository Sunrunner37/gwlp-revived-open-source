## P17\_AccountPermissions ##
| **Datatype** | **Field** | **Comment** |
|:-------------|:----------|:------------|
| **UInt16**   | Header    |             |
| **UInt32**   | LoginCount |             |
| **UInt32**   | Territory | _2 europe, 0 america_  |
| **UInt32**   | TerritoryChanges |             |
| **byte[.md](.md)** | Data1     | _(ConstSize = true, MaxSize = 8)_ |
| **byte[.md](.md)** | Data2     | _(ConstSize = true, MaxSize = 8)_ |
| **byte[.md](.md)** | Data3     | _(ConstSize = true, MaxSize = 16)_ |
| **byte[.md](.md)** | Data4     | _(ConstSize = true, MaxSize = 16)_ |
| **UInt32**   | ChangeAccSettings | _2 = change: email, pw, mail add_  |
| **UInt16**   | ArraySize1 |             |
| **byte[.md](.md)** | AddedKeys | _short: keyID, short: flag_ _(ConstSize = false, MaxSize = 200)_ |
| **byte**     | EulaAccepted |             |
| **UInt32**   | Data5     |             |