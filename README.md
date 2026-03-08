# Performance Benchmarks
Repository for different performance benchmarks

## Prerequisites

- .NET 10.0 SDK or later
- BenchmarkDotNet (included via NuGet)

## Running the Benchmarks

```bash
# Restore dependencies
dotnet restore

# Run benchmarks (Release mode required for accurate results)
dotnet run -c Release

# Alternative: Run with specific benchmark
dotnet run -c Release --filter *EmailParsing*
```

## Project Structure

- **PerformanceBenchmarks.csproj** - Project file with BenchmarkDotNet dependency
- **Program.cs** - Three benchmark suites comparing classic vs Span approaches

## Benchmarks

- Span<T> Performance Benchmarks - more details [here](PerformanceBenchmarks/SpanBenchmarks/README.md).
