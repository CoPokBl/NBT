# NBT
An NBT (Named Binary Tag) parser and writer for C#. Fully compatible with Minecraft's NBT format.

## Tags

All tags implement `INbtTag`. Names are not stored on tags themselves — they are used as keys inside a `CompoundTag`.

| Tag | C# Class | Type ID | C# Value Type |
|-----|----------|---------|---------------|
| `TAG_End` | `EmptyTag` | 0 | *(no value)* |
| `TAG_Byte` | `ByteTag` | 1 | `sbyte` |
| `TAG_Short` | `ShortTag` | 2 | `short` |
| `TAG_Int` | `IntegerTag` | 3 | `int` |
| `TAG_Long` | `LongTag` | 4 | `long` |
| `TAG_Float` | `FloatTag` | 5 | `float` |
| `TAG_Double` | `DoubleTag` | 6 | `double` |
| `TAG_Byte_Array` | `ArrayTag<sbyte>` | 7 | `sbyte[]` |
| `TAG_String` | `StringTag` | 8 | `string` |
| `TAG_List` | `ListTag` / `ListTag<T>` | 9 | `INbtTag[]` |
| `TAG_Compound` | `CompoundTag` | 10 | keyed children |
| `TAG_Int_Array` | `ArrayTag<int>` | 11 | `int[]` |
| `TAG_Long_Array` | `ArrayTag<long>` | 12 | `long[]` |

> **Note:** `BooleanTag` is a convenience subclass of `ByteTag` (stores `true` as `0x01`, `false` as `0x00`). It is not a separate NBT type.

### Primitive tags

```csharp
new ByteTag((sbyte)127)
new ShortTag((short)1000)
new IntegerTag(42)
new LongTag(123456789L)
new FloatTag(3.14f)
new DoubleTag(2.718281828)
new StringTag("hello")
new BooleanTag(true)
new EmptyTag()           // represents TAG_End
```

### Array tags

`ArrayTag<T>` supports `sbyte`, `int`, and `long`:

```csharp
new ArrayTag<sbyte>((sbyte)0, (sbyte)1, (sbyte)2)  // TAG_Byte_Array
new ArrayTag<int>(1, 2, 3)                           // TAG_Int_Array
new ArrayTag<long>(100L, 200L, 300L)                 // TAG_Long_Array
```

### List tag

All elements must be the same tag type. The generic `ListTag<T>` preserves the element type:

```csharp
new ListTag<IntegerTag>(new IntegerTag(1), new IntegerTag(2), new IntegerTag(3))
new ListTag<StringTag>(new StringTag("a"), new StringTag("b"))
```

### Compound tag

`CompoundTag` holds named children as ordered key-value pairs:

```csharp
CompoundTag tag = new(
    ("name", new StringTag("Steve")),
    ("health", new FloatTag(20.0f)),
    ("score", new IntegerTag(100)),
    ("tags", new ListTag<StringTag>(new StringTag("player"))),
    ("inventory", new ArrayTag<int>(1, 2, 3))
);

// Access children by name
INbtTag? health = tag["health"];

// Add a child (returns a new CompoundTag — tags are immutable)
CompoundTag updated = tag.WithChild("level", new IntegerTag(5));
```

You can also construct a `CompoundTag` from a dictionary:

```csharp
CompoundTag tag = new(new Dictionary<string, INbtTag?> {
    ["x"] = new DoubleTag(128.5),
    ["y"] = new DoubleTag(64.0),
    ["z"] = new DoubleTag(-32.0)
});
```

## Serialising

```csharp
CompoundTag tag = new(
    ("name", new StringTag("Test")),
    ("age", new IntegerTag(30)),
    ("scores", new ListTag<IntegerTag>(new IntegerTag(1), new IntegerTag(2))),
    ("data", new ArrayTag<sbyte>((sbyte)0, (sbyte)1, (sbyte)2))
);

byte[] bytes = tag.Serialise();
```

## Deserialising

```csharp
byte[] data = ...; // raw NBT bytes
INbtTag tag = NbtReader.ReadNbt(data);

// Cast to the expected type
CompoundTag compound = (CompoundTag)tag;
string name = compound["name"].GetString();
int age = compound["age"].GetInteger();
```

For NBT files that lack a root type prefix (implied root compound), pass `impliedRoot: true`:

```csharp
INbtTag tag = NbtReader.ReadNbt(data, impliedRoot: true);
```

### Compression

```csharp
INbtTag tag = NbtReader.ReadNbt(data, compression: NbtCompressionType.ZLib);
// NbtCompressionType.None (default), NbtCompressionType.ZLib, NbtCompressionType.GZip
```

You can also construct an `NbtReader` directly from a `Stream`:

```csharp
using FileStream fs = File.OpenRead("level.dat");
NbtReader reader = new(fs, NbtCompressionType.ZLib);
INbtTag tag = reader.ToTag(impliedRoot: true);
```

## JSON conversion

Tags can be converted to/from JSON:

```csharp
// Tag → JSON string
string json = tag.ToJsonString();

// Tag → JToken (Newtonsoft.Json)
JToken jtoken = tag.ToJson();

// JSON string → Tag
INbtTag tag = INbtTag.FromJson(json);

// JToken → Tag
INbtTag tag = INbtTag.FromJson(jtoken);
```
But be careful when expecting certain types because JSON types don't map 1-to-1 to NBT types,
when reading NBT that was deserialised from JSON try to use the helper methods in `NbtTagExtensions` instead.
These methods will try to convert the type to the closest equivalent NBT type.

## Custom serialisable types

Implement `CompoundTagSerialisable` to make your own classes usable as NBT tags:

```csharp
public class PlayerData : CompoundTagSerialisable {
    public string Name { get; init; }
    public int Health { get; init; }

    public override CompoundTag SerialiseToTag() => new(
        ("name", new StringTag(Name)),
        ("health", new IntegerTag(Health))
    );
}

// Use directly as an INbtTag
PlayerData player = new() { Name = "Steve", Health = 20 };
byte[] bytes = player.Serialise();
```

If you only need the interface without inheriting a base class, implement `ICompoundTagSerialisable` instead.
