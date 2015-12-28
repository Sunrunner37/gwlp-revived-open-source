## P65281\_HandshakeRequest ##
| **Datatype** | **Field** | **Comment** |
|:-------------|:----------|:------------|
| **UInt16**   | Header    |             |
| **byte[.md](.md)** | SecurityKey1 |             |
| **byte[.md](.md)** | SecurityKey2 | _(ConstSize = true, MaxSize = 8)_ |
| **UInt32**   | Port      |             |