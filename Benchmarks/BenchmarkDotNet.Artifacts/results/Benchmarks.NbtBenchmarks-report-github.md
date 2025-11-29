```

BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763 2.72GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 8.0.22 (8.0.22, 8.0.2225.52707), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 8.0.22 (8.0.22, 8.0.2225.52707), X64 RyuJIT x86-64-v3


```
| Method                          | Mean        | Error     | StdDev    | Rank | Gen0   | Gen1   | Allocated |
|-------------------------------- |------------:|----------:|----------:|-----:|-------:|-------:|----------:|
| &#39;Deserialize Simple Tag&#39;        |    395.7 ns |   4.72 ns |   4.18 ns |    1 | 0.0572 |      - |     960 B |
| &#39;Serialize Simple Tag&#39;          |    703.5 ns |   4.07 ns |   3.81 ns |    2 | 0.0916 |      - |    1544 B |
| &#39;Deserialize Deeply Nested Tag&#39; |    921.4 ns |  17.74 ns |  19.72 ns |    3 | 0.1450 |      - |    2440 B |
| &#39;Round-trip Simple Tag&#39;         |  1,230.4 ns |   4.77 ns |   4.46 ns |    4 | 0.1488 |      - |    2504 B |
| &#39;Deserialize Complex Tag&#39;       |  1,279.1 ns |  18.29 ns |  17.10 ns |    5 | 0.1793 |      - |    3008 B |
| &#39;Serialize Deeply Nested Tag&#39;   |  1,606.2 ns |  22.79 ns |  21.32 ns |    6 | 0.2518 |      - |    4240 B |
| &#39;Serialize Complex Tag&#39;         |  1,889.4 ns |  22.43 ns |  20.98 ns |    7 | 0.2689 |      - |    4512 B |
| &#39;Round-trip Complex Tag&#39;        |  3,599.6 ns |  39.96 ns |  37.38 ns |    8 | 0.4463 |      - |    7520 B |
| &#39;Deserialize Large Array Tag&#39;   | 26,819.4 ns | 158.82 ns | 148.56 ns |    9 | 2.5024 | 0.0610 |   42104 B |
| &#39;Serialize Large Array Tag&#39;     | 27,031.8 ns | 539.13 ns | 620.86 ns |    9 | 6.3477 | 0.4272 |  106368 B |
