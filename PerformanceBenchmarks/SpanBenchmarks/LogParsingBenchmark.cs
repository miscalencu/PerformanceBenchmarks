using BenchmarkDotNet.Attributes;

namespace PerformanceBenchmarks.SpanBenchmarks;

/// <summary>
/// Benchmarks for parsing log entries
/// Scenario: Extract timestamp, level, and message from structured log
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class LogParsingBenchmark
{
    private const string LogEntry = "[2024-03-15 14:32:18] ERROR: Database connection failed - Timeout after 30s";

    [Benchmark(Baseline = true)]
    public string ClassicLogParsing()
    {
        // Multiple Substring and IndexOf calls = multiple allocations
        var timestampEnd = LogEntry.IndexOf(']');
        var timestamp = LogEntry.Substring(1, timestampEnd - 1);

        var levelStart = timestampEnd + 2;
        var levelEnd = LogEntry.IndexOf(':', levelStart);
        var level = LogEntry.Substring(levelStart, levelEnd - levelStart);

        var messageStart = levelEnd + 2;
        var dashIndex = LogEntry.IndexOf('-', messageStart);
        var message = LogEntry.Substring(messageStart, dashIndex - messageStart).Trim();

        // Extract just time from timestamp
        var timeStart = timestamp.IndexOf(' ') + 1;
        var time = timestamp.Substring(timeStart);

        return $"[{time}] {level}: {message}";
    }

    [Benchmark]
    public string SpanLogParsing()
    {
        ReadOnlySpan<char> log = LogEntry.AsSpan();

        // All slicing operations are zero-allocation views
        int timestampEnd = log.IndexOf(']');
        ReadOnlySpan<char> timestamp = log.Slice(1, timestampEnd - 1);

        int levelStart = timestampEnd + 2;
        ReadOnlySpan<char> afterTimestamp = log.Slice(levelStart);
        int levelEnd = levelStart + afterTimestamp.IndexOf(':');
        ReadOnlySpan<char> level = log.Slice(levelStart, levelEnd - levelStart);

        int messageStart = levelEnd + 2;
        ReadOnlySpan<char> afterLevel = log.Slice(messageStart);
        int dashIndex = messageStart + afterLevel.IndexOf('-');
        ReadOnlySpan<char> message = log.Slice(messageStart, dashIndex - messageStart).Trim();

        // Extract time from timestamp (still zero allocations)
        int timeStart = timestamp.IndexOf(' ') + 1;
        ReadOnlySpan<char> time = timestamp.Slice(timeStart);

        // Only allocation happens here with interpolation
        return $"[{time}] {level}: {message}";
    }
}
