## P04\_AccountLogin ##
| **Datatype** | **Field** | **Comment** |
|:-------------|:----------|:------------|
| **UInt16**   | Header    |             |
| **UInt32**   | LoginCount |             |
| **byte[.md](.md)** | ClPassword | _(ConstSize = true, MaxSize = 24)_ |
| **string**   | ClEmail   | _(ConstSize = false, MaxSize = 64)_ |
| **string**   | Data2     | _(ConstSize = false, MaxSize = 20)_ |
| **string**   | CharName  | _(ConstSize = false, MaxSize = 20)_ |
| **#region**  | Implementation of IHasAccountData |             |
| **string**   | Email     |             |
|              | {         |             |
| **get**      | { return ClEmail } |             |
| **set**      | { ClEmail = value } |             |
|              | }         |             |
| **string**   | Password  |             |
|              | {         |             |
|              | get       |             |
|              | {         |             |
| **var**      | pw = ""   |             |
| **var**      | raw = (byte[.md](.md))ClPassword.Clone() |             |
| **raw[0](0.md)** | /= 2      | _because UTF16 has 2 bytes per character_  |
| **raw[1](1.md)** | = 0       | _because that was not set correctly by the client_  |
| **RawConverter.ReadUTF16(ref** | pw, new MemoryStream(raw)) |             |
| **return**   | pw        |             |
|              | }         |             |
| **set**      | { ClPassword =new byte[0](0.md) |             |