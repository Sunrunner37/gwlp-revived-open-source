## P65286\_ClientDispatchConnectionUpdate ##
| **Datatype** | **Field** | **Comment** |
|:-------------|:----------|:------------|
| **UInt16**   | Header    |             |
| **byte[.md](.md)** | Key1      |             |
| **byte[.md](.md)** | Key2      | _(ConstSize = true, MaxSize = 4)_ |
| **byte[.md](.md)** | ConnectionInfo | _(ConstSize = true, MaxSize = 24)_ |
| **UInt32**   | AccID     |             |
| **UInt32**   | CharID    |             |
| **UInt32**   | MapID     |             |
| **UInt32**   | OldMapID  |             |
| **byte**     | IsOutpost |             |
| **byte**     | IsPvE     |             |