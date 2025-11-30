using BenchmarkDotNet.Running;
using Benchmarks;

// Check if quick benchmark mode is requested
if (args.Length > 0 && args[0] == "--quick") {
    QuickBenchmark.Run();
} else {
    BenchmarkRunner.Run<NbtBenchmarks>();
}
