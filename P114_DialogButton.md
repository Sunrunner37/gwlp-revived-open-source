## P114\_DialogButton ##
| **Datatype** | **Field** | **Comment** |
|:-------------|:----------|:------------|
| **UInt16**   | Header    |             |
| **byte**     | Icon      |             |
| **string**   | Text      | _(ConstSize = false, MaxSize = 128)_ |
| **UInt32**   | ButtonId  | _client will sent this when button is clicked_  |
| **UInt32**   | Flag      |             |