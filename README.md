# TLV Serializer

Library for encoding using optional informational elements in a given protocol. A TLV encoded data stream contains code related to the record type, the length of the record value, and finally the value itself.

## Version.

| Library          | Version   |
|------------------|-----------|
| .Net core        | 5.0.0     |

## How to use.

### Serialize.

Example of object serialization in TLV. It is necessary to define annotations with a unique identifier in byte eg `0x01`.

```c#
public class Authorization
{
    [TlvTag(Id = 0x01)]
    public string Id { get; set; }
    
    [TlvTag(Id = 0x02)]
    public string Authorizer { get; set; }
    
    [TlvTag(Id = 0x03)]
    public string Operation { get; set; }
}
```
>Obs: The type is defined in byte.

Serialization

```c#
var bufferRx = TlvSerialize.Serialize(new Authorization
{
    Id = "001",
    Authorizer = "CIELO",
    Operation = "CREDIT"
});
```

Output: `010330303102054349454C4F0306435245444954`

| Type   | Length | Value        |
|--------|--------|--------------|
| `0x01` | 3      | 303031       |
| `0x02` | 5      | 4349454C4F   |
| `0x03` | 6      | 435245444954 |

### Deserialize.

Example of deserialization from TLV to object.

```c#
var content = TlvSerialize
    .Deserialize<Authorization>("010330303102054349454C4F0306435245444954");
```

## Dump.

### Dump from object.

```c#
var dumpRx = TlvSerialize.Dump(new Authorization
{
    Id = "001",
    Authorizer = "CIELO",
    Operation = "CREDIT"
});
```

```shell
001(003): 001
002(005): CIELO
003(006): CREDIT
```

### Dump from Hex.

```c#
var dump = TlvSerialize
    .Dump("010330303102054349454C4F0306435245444954");
```

```shell
001(003): 001
002(005): CIELO
003(006): CREDIT
```

## References:

- [TLV](https://en.wikipedia.org/wiki/Type%E2%80%93length%E2%80%93value)