using BenchmarkDotNet.Attributes;

namespace PerformanceBenchmarks.SpanBenchmarks;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class StringProcessingBenchmark
{
    private const string SampleData = "2026-03-07,ACME Corp,Invoice,1250.75,EUR,Paid";

    /// <summary>
    /// Classic approach: Multiple Substring calls create new string allocations
    /// Each Substring creates a new string object on the heap
    /// </summary>
    [Benchmark(Baseline = true)]
    public string ClassicSubstring()
    {
        var parts = SampleData.Split(',');

        // Extract year from date (creates allocation)
        var year = parts[0].Substring(0, 4);

        // Extract company name and trim (creates allocation)
        var company = parts[1].Trim();

        // Extract first word of company (creates allocation)
        var companyShort = company.Substring(0, company.IndexOf(' '));

        // Extract amount (creates allocation)
        var amount = parts[3];

        // Format result (creates final allocation)
        return $"{year}-{companyShort}-${amount}";
    }

    /// <summary>
    /// Span-based approach: Zero intermediate allocations
    /// Works directly with memory views, only allocates final result
    /// </summary>
    [Benchmark]
    public string SpanBased()
    {
        ReadOnlySpan<char> data = SampleData.AsSpan();

        // Find positions without allocating strings
        int firstComma = data.IndexOf(',');
        int secondComma = data[(firstComma + 1)..].IndexOf(',') + firstComma + 1;
        int thirdComma = data[(secondComma + 1)..].IndexOf(',') + secondComma + 1;
        int fourthComma = data[(thirdComma + 1)..].IndexOf(',') + thirdComma + 1;

        // Extract year (no allocation, just a view)
        ReadOnlySpan<char> year = data.Slice(0, 4);

        // Extract company and trim (no allocation)
        ReadOnlySpan<char> company = data.Slice(firstComma + 1, secondComma - firstComma - 1).Trim();

        // Extract first word of company (no allocation)
        int spaceIndex = company.IndexOf(' ');
        ReadOnlySpan<char> companyShort = company.Slice(0, spaceIndex);

        // Extract amount (no allocation)
        ReadOnlySpan<char> amount = data.Slice(thirdComma + 1, fourthComma - thirdComma - 1);

        // Only allocation: final string construction using string interpolation
        return $"{year}-{companyShort}-${amount}";
    }
}
