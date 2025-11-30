using System.Diagnostics;
using NBT;
using NBT.Tags;

namespace Benchmarks;

/// <summary>
/// Quick benchmark runner for rapid iteration and comparison
/// </summary>
public class QuickBenchmark {
    public static void Run() {
        Console.WriteLine("=== NBT Performance Benchmark ===\n");
        
        // Setup test data
        var simpleTag = new CompoundTag(null,
            new StringTag("name", "Test"),
            new IntegerTag("age", 30),
            new BooleanTag("active", true),
            new DoubleTag("score", 98.5)
        );
        
        var complexTag = new CompoundTag(null,
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
        
        int[] largeIntArray = new int[1000];
        for (int i = 0; i < largeIntArray.Length; i++) {
            largeIntArray[i] = i;
        }
        
        sbyte[] largeByteArray = new sbyte[5000];
        for (int i = 0; i < largeByteArray.Length; i++) {
            largeByteArray[i] = (sbyte)(i % 128);
        }
        
        var largeArrayTag = new CompoundTag(null,
            new StringTag("name", "LargeArrays"),
            new ArrayTag<int>("intArray", largeIntArray),
            new ArrayTag<sbyte>("byteArray", largeByteArray)
        );
        
        // Warmup
        for (int i = 0; i < 1000; i++) {
            _ = simpleTag.Serialise();
            _ = NbtReader.ReadNbt(simpleTag.Serialise());
        }
        
        // Benchmarks
        const int iterations = 100000;
        
        RunBenchmark("Simple Tag Serialization", iterations, () => {
            _ = simpleTag.Serialise();
        });
        
        byte[] simpleBytes = simpleTag.Serialise();
        RunBenchmark("Simple Tag Deserialization", iterations, () => {
            _ = NbtReader.ReadNbt(simpleBytes);
        });
        
        RunBenchmark("Complex Tag Serialization", iterations / 2, () => {
            _ = complexTag.Serialise();
        });
        
        byte[] complexBytes = complexTag.Serialise();
        RunBenchmark("Complex Tag Deserialization", iterations / 2, () => {
            _ = NbtReader.ReadNbt(complexBytes);
        });
        
        RunBenchmark("Large Array Serialization", iterations / 10, () => {
            _ = largeArrayTag.Serialise();
        });
        
        byte[] largeBytes = largeArrayTag.Serialise();
        RunBenchmark("Large Array Deserialization", iterations / 10, () => {
            _ = NbtReader.ReadNbt(largeBytes);
        });
        
        // Memory benchmark
        Console.WriteLine("\n=== Memory Allocation Test ===");
        long before = GC.GetTotalMemory(true);
        for (int i = 0; i < 10000; i++) {
            _ = simpleTag.Serialise();
        }
        long after = GC.GetTotalMemory(false);
        Console.WriteLine($"Simple Tag (10k iterations): {(after - before) / 10000.0:F2} bytes avg");
        
        before = GC.GetTotalMemory(true);
        for (int i = 0; i < 10000; i++) {
            _ = complexTag.Serialise();
        }
        after = GC.GetTotalMemory(false);
        Console.WriteLine($"Complex Tag (10k iterations): {(after - before) / 10000.0:F2} bytes avg");
    }
    
    private static void RunBenchmark(string name, int iterations, Action action) {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++) {
            action();
        }
        sw.Stop();
        
        double avgNs = (sw.Elapsed.TotalNanoseconds / iterations);
        Console.WriteLine($"{name,-40} {avgNs,8:F2} ns/op ({iterations} iterations)");
    }
}
