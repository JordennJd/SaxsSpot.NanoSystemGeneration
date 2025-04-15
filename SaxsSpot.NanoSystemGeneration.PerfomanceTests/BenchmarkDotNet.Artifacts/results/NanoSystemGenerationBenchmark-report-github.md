```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22621.1105/22H2/2022Update/SunValley2)
AMD Ryzen 5 3600, 1 CPU, 12 logical and 6 physical cores
.NET SDK 9.0.201
  [Host]     : .NET 9.0.3 (9.0.325.11113), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.3 (9.0.325.11113), X64 RyuJIT AVX2


```
| Method                | Mean    | Error   | StdDev  | Gen0         | Gen1      | Gen2      | Allocated |
|---------------------- |--------:|--------:|--------:|-------------:|----------:|----------:|----------:|
| GenerateAndDistribute | 10.95 s | 0.206 s | 0.182 s | 2010000.0000 | 2000.0000 | 1000.0000 |  15.65 GB |
