using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using NBT;
using NBT.Tags;

namespace Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class NbtBenchmarks {
    private CompoundTag _simpleTag = null!;
    private CompoundTag _complexTag = null!;
    private CompoundTag _deeplyNestedTag = null!;
    private CompoundTag _largeArrayTag = null!;
    
    private byte[] _simpleTagBytes = null!;
    private byte[] _complexTagBytes = null!;
    private byte[] _deeplyNestedTagBytes = null!;
    private byte[] _largeArrayTagBytes = null!;

    [GlobalSetup]
    public void Setup() {
        // Simple tag with basic types
        _simpleTag = new CompoundTag(null,
            new StringTag("name", "Test"),
            new IntegerTag("age", 30),
            new BooleanTag("active", true),
            new DoubleTag("score", 98.5)
        );
        
        // Complex tag with nested structures
        _complexTag = new CompoundTag(null,
            new StringTag("name", "ComplexTest"),
            new IntegerTag("level", 15),
            new ListTag<IntegerTag>("scores", [
                new IntegerTag(null, 100),
                new IntegerTag(null, 200),
                new IntegerTag(null, 300),
                new IntegerTag(null, 400),
                new IntegerTag(null, 500)
            ]),
            new CompoundTag("nested",
                new StringTag("field1", "value1"),
                new StringTag("field2", "value2"),
                new IntegerTag("field3", 42)
            ),
            new ArrayTag<sbyte>("byteArray", 1, 2, 3, 4, 5, 6, 7, 8, 9, 10)
        );
        
        // Deeply nested structure
        CompoundTag innerMost = new CompoundTag("level5",
            new StringTag("data", "innermost"),
            new IntegerTag("value", 999)
        );
        
        for (int i = 4; i > 0; i--) {
            innerMost = new CompoundTag($"level{i}", innerMost);
        }
        
        _deeplyNestedTag = new CompoundTag(null,
            new StringTag("name", "DeeplyNested"),
            innerMost
        );
        
        // Tag with large arrays
        int[] largeIntArray = new int[1000];
        for (int i = 0; i < largeIntArray.Length; i++) {
            largeIntArray[i] = i;
        }
        
        sbyte[] largeByteArray = new sbyte[5000];
        for (int i = 0; i < largeByteArray.Length; i++) {
            largeByteArray[i] = (sbyte)(i % 128);
        }
        
        _largeArrayTag = new CompoundTag(null,
            new StringTag("name", "LargeArrays"),
            new ArrayTag<int>("intArray", largeIntArray),
            new ArrayTag<sbyte>("byteArray", largeByteArray)
        );
        
        // Pre-serialize for deserialization benchmarks
        _simpleTagBytes = _simpleTag.Serialise();
        _complexTagBytes = _complexTag.Serialise();
        _deeplyNestedTagBytes = _deeplyNestedTag.Serialise();
        _largeArrayTagBytes = _largeArrayTag.Serialise();
    }

    // ===== Serialization Benchmarks =====
    
    [Benchmark(Description = "Serialize Simple Tag")]
    public byte[] SerializeSimpleTag() {
        return _simpleTag.Serialise();
    }

    [Benchmark(Description = "Serialize Complex Tag")]
    public byte[] SerializeComplexTag() {
        return _complexTag.Serialise();
    }

    [Benchmark(Description = "Serialize Deeply Nested Tag")]
    public byte[] SerializeDeeplyNestedTag() {
        return _deeplyNestedTag.Serialise();
    }

    [Benchmark(Description = "Serialize Large Array Tag")]
    public byte[] SerializeLargeArrayTag() {
        return _largeArrayTag.Serialise();
    }

    // ===== Deserialization Benchmarks =====
    
    [Benchmark(Description = "Deserialize Simple Tag")]
    public INbtTag DeserializeSimpleTag() {
        return NbtReader.ReadNbt(_simpleTagBytes);
    }

    [Benchmark(Description = "Deserialize Complex Tag")]
    public INbtTag DeserializeComplexTag() {
        return NbtReader.ReadNbt(_complexTagBytes);
    }

    [Benchmark(Description = "Deserialize Deeply Nested Tag")]
    public INbtTag DeserializeDeeplyNestedTag() {
        return NbtReader.ReadNbt(_deeplyNestedTagBytes);
    }

    [Benchmark(Description = "Deserialize Large Array Tag")]
    public INbtTag DeserializeLargeArrayTag() {
        return NbtReader.ReadNbt(_largeArrayTagBytes);
    }

    // ===== Round-trip Benchmarks =====
    
    [Benchmark(Description = "Round-trip Simple Tag")]
    public INbtTag RoundTripSimpleTag() {
        byte[] serialized = _simpleTag.Serialise();
        return NbtReader.ReadNbt(serialized);
    }

    [Benchmark(Description = "Round-trip Complex Tag")]
    public INbtTag RoundTripComplexTag() {
        byte[] serialized = _complexTag.Serialise();
        return NbtReader.ReadNbt(serialized);
    }
}
