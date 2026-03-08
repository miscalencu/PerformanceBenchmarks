using BenchmarkDotNet.Attributes;

namespace PerformanceBenchmarks.SpanBenchmarks;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class OneLineParseBenchmark
{
    private const string line = "12345,John,Doe,42,Developer";

    [Benchmark(Baseline = true)]
    public (int Id, string FirstName, string LastName, int Age, string Job) OneLineParseClassic()
    {
        var parts = line.Split(',');

        return (
            int.Parse(parts[0]),
            parts[1],
            parts[2],
            int.Parse(parts[3]),
            parts[4]
        );
    }

    [Benchmark]
    public (int Id, string FirstName, string LastName, int Age, string Job) OneLineParseSpan()
    {
        ReadOnlySpan<char> span = line.AsSpan();

        int c1 = span.IndexOf(',');
        int id = int.Parse(span[..c1]);

        span = span[(c1 + 1)..];
        int c2 = span.IndexOf(',');
        string firstName = new(span[..c2]);

        span = span[(c2 + 1)..];
        int c3 = span.IndexOf(',');
        string lastName = new(span[..c3]);

        span = span[(c3 + 1)..];
        int c4 = span.IndexOf(',');
        int age = int.Parse(span[..c4]);

        string job = new(span[(c4 + 1)..]);

        return (id, firstName, lastName, age, job);
    }
}
