# NBT Benchmarks

This project contains performance benchmarks for the NBT library using BenchmarkDotNet.

## Running Benchmarks

### Full Benchmark Suite

Run the complete BenchmarkDotNet suite with detailed statistical analysis:

```bash
dotnet run -c Release
```

This will:
- Run multiple warmup iterations
- Execute each benchmark multiple times for statistical accuracy
- Generate detailed reports in `BenchmarkDotNet.Artifacts/results/`
- Take approximately 5-10 minutes to complete

### Quick Benchmark

For rapid iteration during development, use the quick benchmark mode:

```bash
dotnet run -c Release -- --quick
```

This provides:
- Faster execution (under 1 minute)
- Good enough accuracy for comparing changes
- Memory allocation statistics
- Suitable for development workflow

## Benchmark Scenarios

The benchmarks test the following scenarios:

### Simple Tag
A basic compound tag with primitive types:
- StringTag
- IntegerTag  
- BooleanTag
- DoubleTag

### Complex Tag
More complex structures with:
- Nested CompoundTag
- ListTag with multiple elements
- ArrayTag (byte array)
- Mixed data types

### Deeply Nested Tag
Tests performance with:
- 5 levels of nested CompoundTags
- Stress tests the recursive serialization/deserialization

### Large Array Tag
Tests handling of bulk data:
- 1,000 element int array
- 5,000 element byte array
- Validates performance at scale

## Benchmark Operations

Each scenario tests:
1. **Serialization** - Converting tag objects to byte arrays
2. **Deserialization** - Parsing byte arrays into tag objects
3. **Round-trip** - Full serialize + deserialize cycle

## Understanding Results

### Key Metrics

- **Mean** - Average execution time per operation
- **Error** - Standard error of the mean
- **StdDev** - Standard deviation of measurements
- **Allocated** - Bytes allocated per operation (important for GC pressure)
- **Gen0/Gen1/Gen2** - Garbage collection statistics

### Reading the Output

Example output:
```
| Method                         | Mean      | Error    | Allocated |
|------------------------------- |----------:|---------:|----------:|
| Serialize Simple Tag           | 703.5 ns  | 4.07 ns  | 1544 B    |
| Deserialize Simple Tag         | 395.7 ns  | 4.72 ns  | 960 B     |
```

- Lower `Mean` = faster performance
- Lower `Allocated` = less GC pressure
- Lower `Gen0/1/2` = fewer garbage collections

## Viewing Detailed Results

After running full benchmarks, view results in:
- `BenchmarkDotNet.Artifacts/results/*.html` - Interactive HTML report
- `BenchmarkDotNet.Artifacts/results/*.md` - Markdown table
- `BenchmarkDotNet.Artifacts/results/*.csv` - CSV for Excel/analysis

## Performance Testing Tips

1. **Close other applications** - Minimize background interference
2. **Run in Release mode** - Always use `-c Release`
3. **Disable CPU frequency scaling** - For most consistent results
4. **Let it complete** - Full benchmarks take time for accuracy
5. **Compare similar runs** - System state affects results

## Development Workflow

When optimizing code:

1. Run quick benchmark to get baseline: `dotnet run -c Release -- --quick`
2. Make your optimization changes
3. Run quick benchmark again to verify improvement
4. Run full benchmark suite before committing: `dotnet run -c Release`
5. Compare `Allocated` column carefully - memory is often more important than raw speed

## Troubleshooting

### Benchmarks take too long
Use the `--quick` flag for faster results during development.

### Results seem inconsistent
- Ensure system is idle (no heavy background processes)
- Run multiple times and look for trends
- Consider rebooting for cleanest baseline

### "Failed to set up high priority" warning
This is normal and doesn't affect results accuracy significantly. It just means the process can't run at elevated priority.

## See Also

- [PERFORMANCE.md](../PERFORMANCE.md) - Detailed performance analysis and optimization guide
- [BenchmarkDotNet Documentation](https://benchmarkdotnet.org/)
