using BenchmarkDotNet.Attributes;

namespace PerformanceBenchmarks.SpanBenchmarks;

/// <summary>
/// Benchmarks for parsing and validating email addresses
/// Scenario: Extract username and domain, validate format
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class EmailParsingBenchmark
{
    private const string Email = "john.doe@company-example.com";

    [Benchmark(Baseline = true)]
    public string ClassicEmailParsing()
    {
        // Multiple allocations for validation and extraction
        if (!Email.Contains("@"))
            return "Invalid";

        var parts = Email.Split('@');
        var username = parts[0];
        var domain = parts[1];

        // Extract domain parts (more allocations)
        var domainParts = domain.Split('.');
        var domainName = domainParts[0];
        var tld = domainParts[domainParts.Length - 1];

        // Uppercase transformations (more allocations)
        var userUpper = username.ToUpper();
        var tldUpper = tld.ToUpper();

        return $"{userUpper}@{tldUpper}";
    }

    [Benchmark]
    public string SpanEmailParsing()
    {
        ReadOnlySpan<char> email = Email.AsSpan();

        // Find @ without allocation
        int atIndex = email.IndexOf('@');
        if (atIndex == -1)
            return "Invalid";

        ReadOnlySpan<char> username = email.Slice(0, atIndex);
        ReadOnlySpan<char> domain = email.Slice(atIndex + 1);

        // Find last dot in domain
        int lastDot = domain.LastIndexOf('.');
        ReadOnlySpan<char> tld = domain.Slice(lastDot + 1);

        // Use stackalloc for uppercase conversion (no heap allocation)
        Span<char> userUpper = stackalloc char[username.Length];
        Span<char> tldUpper = stackalloc char[tld.Length];

        username.ToUpperInvariant(userUpper);
        tld.ToUpperInvariant(tldUpper);

        return $"{userUpper}@{tldUpper}";
    }
}
