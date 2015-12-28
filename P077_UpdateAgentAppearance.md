## P077\_UpdateAgentAppearance ##
| **Datatype** | **Field** | **Comment** |
|:-------------|:----------|:------------|
| **UInt16**   | Header    |             |
| **UInt32**   | Data1     |             |
| **UInt32**   | ID1       |             |
| **byte[.md](.md)** | Appearance | _~_ _(ConstSize = true, MaxSize = 4)] // changed, was UInt3_ |
| **byte**     | Data2     |             |
| **UInt32**   | Data3     |             |
| **UInt32**   | Data4     |             |
| **string**   | Name      | _(ConstSize = false, MaxSize = 32)_ |