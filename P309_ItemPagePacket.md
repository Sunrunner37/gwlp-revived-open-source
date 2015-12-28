## P309\_ItemPagePacket ##
| **Datatype** | **Field** | **Comment** |
|:-------------|:----------|:------------|
| **UInt16**   | Header    |             |
| **UInt16**   | ItemStreamID |             |
| **byte**     | StorageType | _Bags=0x1,Equiped=0x2,NotCollected=0x3 ???,Storage=0x4,StorageMaterial=0x5_  |
| **byte**     | StorageID | _see GameServer.Enums.ItemStorage_  |
| **UInt16**   | PageID    |             |
| **byte**     | Slots     |             |
| **UInt32**   | ItemLocalID |             |