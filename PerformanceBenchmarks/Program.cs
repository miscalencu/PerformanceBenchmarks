using BenchmarkDotNet.Running;
using PerformanceBenchmarks.SpanBenchmarks;

var summary1 = BenchmarkRunner.Run<StringProcessingBenchmark>();
Console.WriteLine("\n");

var summary2 = BenchmarkRunner.Run<EmailParsingBenchmark>();
Console.WriteLine("\n");

var summary3 = BenchmarkRunner.Run<LogParsingBenchmark>();
Console.WriteLine("\n");