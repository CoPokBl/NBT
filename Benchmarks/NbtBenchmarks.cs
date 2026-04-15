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
        _simpleTag = new CompoundTag(
            ("name", new StringTag("Test")),
            ("age", new IntegerTag(30)),
            ("active", new BooleanTag(true)),
            ("score", new DoubleTag(98.5))
        );
        
        // Complex tag with nested structures
        _complexTag = new CompoundTag(
            ("name", new StringTag("ComplexTest")),
            ("level", new IntegerTag(15)),
            ("scores", new ListTag<IntegerTag>([
                new IntegerTag(100),
                new IntegerTag(200),
                new IntegerTag(300),
                new IntegerTag(400),
                new IntegerTag(500)
            ])),
            ("nested", new CompoundTag(
                ("field1", new StringTag("value1")),
                ("field2", new StringTag("value2")),
                ("field3", new IntegerTag(42))
            )),
            ("byteArray", new ArrayTag<sbyte>(1, 2, 3, 4, 5, 6, 7, 8, 9, 10))
        );
        
        // Deeply nested structure
        CompoundTag innerMost = new CompoundTag(
            ("data", new StringTag("innermost")),
            ("value", new IntegerTag(999))
        );
        
        for (int i = 5; i > 1; i--) {
            innerMost = new CompoundTag(($"level{i}", innerMost));
        }
        
        _deeplyNestedTag = new CompoundTag(
            ("name", new StringTag("DeeplyNested")),
            ("level1", innerMost)
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
        
        _largeArrayTag = new CompoundTag(
            ("name", new StringTag("LargeArrays")),
            ("intArray", new ArrayTag<int>(largeIntArray)),
            ("byteArray", new ArrayTag<sbyte>(largeByteArray))
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
