using NBT;
using NBT.Tags;

namespace Tests;

/// <summary>
/// Single source of truth for all binary blob test cases.
/// Used by both BlobGenerator (to produce GeneratedBlobs.cs) and
/// BinaryBlobTest (to run the snapshot tests).
///
/// To add a new test case, add an entry here then run:
///   dotnet run --project BlobGenerator
/// </summary>
internal static class BlobTestCases {
    public static (string Name, INbtTag Tag)[] All => [
        ("ByteTagPositive",    new ByteTag(0x56)),
        ("ByteTagNegative",    new ByteTag(-1)),
        ("ShortTag",           new ShortTag(1000)),
        ("IntegerTag",         new IntegerTag(42)),
        ("LongTag",            new LongTag(0x0102030405060708L)),
        ("FloatTag",           new FloatTag(1.0f)),
        ("DoubleTag",          new DoubleTag(1.0)),
        ("StringTagAscii",     new StringTag("hi")),
        ("BooleanTagTrue",     new BooleanTag(true)),
        ("BooleanTagFalse",    new BooleanTag(false)),
        ("ByteArrayTag",       new ArrayTag<sbyte>(0, 1, -1)),
        ("IntArrayTag",        new ArrayTag<int>(1, 2, 3)),
        ("LongArrayTag",       new ArrayTag<long>(1L, 2L)),
        ("ListTagIntegers",    new ListTag<IntegerTag>([new IntegerTag(1), new IntegerTag(2)])),
        ("CompoundTagEmpty",   new CompoundTag(new Dictionary<string, INbtTag?>())),
        ("CompoundTagComplex", new CompoundTag(
            ("name", new StringTag("Steve")),
            ("level", new IntegerTag(10)),
            ("scores", new ListTag<IntegerTag>([new IntegerTag(100), new IntegerTag(200)])),
            ("flags", new ArrayTag<sbyte>(1, 0, 1))
        ))
    ];
}
