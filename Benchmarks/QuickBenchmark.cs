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
        var simpleTag = new CompoundTag(
            ("name", new StringTag("Test")),
            ("age", new IntegerTag(30)),
            ("active", new BooleanTag(true)),
            ("score", new DoubleTag(98.5))
        );
        
        var complexTag = new CompoundTag(
            ("name", new StringTag("ComplexTest")),
            ("level", new IntegerTag(15)),
            ("scores", new ListTag<IntegerTag>([
                new IntegerTag(100),
                new IntegerTag(200),
                new IntegerTag( 300),
                new IntegerTag( 400),
                new IntegerTag(500)
            ])),
            ("nested", new CompoundTag(
                ("field1", new StringTag("value1")),
                ("field2", new StringTag("value2")),
                ("field3", new IntegerTag(42))
            )),
            ("byteArray", new ArrayTag<sbyte>(1, 2, 3, 4, 5, 6, 7, 8, 9, 10))
        );
        
        int[] largeIntArray = new int[1000];
        for (int i = 0; i < largeIntArray.Length; i++) {
            largeIntArray[i] = i;
        }
        
        sbyte[] largeByteArray = new sbyte[5000];
        for (int i = 0; i < largeByteArray.Length; i++) {
            largeByteArray[i] = (sbyte)(i % 128);
        }
        
        var largeArrayTag = new CompoundTag(
            ("name", new StringTag("LargeArrays")),
            ("intArray", new ArrayTag<int>(largeIntArray)),
            ("byteArray", new ArrayTag<sbyte>(largeByteArray))
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
