using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Callyzer.App.Models;
using Callyzer.App.Repositories;
using Callyzer.App.SQLite;

namespace Callyzer.Tests
{
    public class PerformanceBenchmarks : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private readonly DatabaseService _dbService;
        private readonly AnalyticsRepository _repo;
        private readonly string _dbPath;

        public PerformanceBenchmarks(ITestOutputHelper output)
        {
            _output = output;
            
            // Use a temporary file for the benchmark database to avoid locking issues
            _dbPath = Path.Combine(Path.GetTempPath(), $"benchmark_{Guid.NewGuid()}.db3");
            
            // We need to inject the mock logger and set up the DatabaseService
            // Since DatabaseService creates the DB at LocalApplicationData by default,
            // we will use a reflection hack to override the _databasePath for testing,
            // or we could modify DatabaseService to accept a path. 
            // For this benchmark, we'll instantiate it normally, but since we cannot easily
            // override the path without modifying App code, we will just let it create a test db in local app data.
            // Wait, we can modify DatabaseService to accept an optional path in the constructor!
            
            var logger = new MockLogger();
            _dbService = new DatabaseService(logger, _dbPath); // Assuming we modify DatabaseService to accept a path
            _repo = new AnalyticsRepository(_dbService);
        }

        [Fact]
        public async Task Test_50K_Records_Summary_Is_Under_100ms()
        {
            // 1. Setup Database
            await _dbService.InitializeAsync();
            var db = _dbService.Database;
            await db.DeleteAllAsync<CallLogModel>(); // Ensure clean state

            // 2. Generate 50K logs
            _output.WriteLine("Generating 50,000 synthetic call logs...");
            var endDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var logs = SyntheticDataGenerator.GenerateLogs(50000, endDate, 365); // Over 1 year

            // 3. Insert Logs (in chunks)
            _output.WriteLine("Inserting logs into SQLite...");
            var sw = Stopwatch.StartNew();
            await db.InsertAllAsync(logs, runInTransaction: true);
            sw.Stop();
            _output.WriteLine($"Insertion took {sw.ElapsedMilliseconds} ms");

            // 4. Benchmark Analytics Summary Query (Monthly)
            var queryFrom = endDate.AddMonths(-1);
            var queryTo = endDate;

            // Warmup query (compile SQL, load pages into cache)
            await _repo.GetSummaryForRangeAsync(queryFrom, queryTo);

            // Actual benchmark
            sw.Restart();
            var summary = await _repo.GetSummaryForRangeAsync(queryFrom, queryTo);
            sw.Stop();

            _output.WriteLine($"Query returned {summary.TotalCalls} calls");
            _output.WriteLine($"GetSummaryForRangeAsync took {sw.ElapsedMilliseconds} ms");

            // 5. Assert performance
            Assert.True(sw.ElapsedMilliseconds < 100, $"Query took too long: {sw.ElapsedMilliseconds}ms (Expected < 100ms)");
        }

        public void Dispose()
        {
            _dbService.CloseAsync().GetAwaiter().GetResult();
            if (File.Exists(_dbPath))
            {
                try { File.Delete(_dbPath); } catch { }
            }
        }
    }
}
