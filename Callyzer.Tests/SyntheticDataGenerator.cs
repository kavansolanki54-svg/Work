using System;
using System.Collections.Generic;
using Callyzer.App.Models;

namespace Callyzer.Tests
{
    public static class SyntheticDataGenerator
    {
        public static List<CallLogModel> GenerateLogs(int count, DateTime endDate, int daysBack)
        {
            var logs = new List<CallLogModel>(count);
            var random = new Random(42); // Fixed seed for reproducible tests
            
            var contacts = new[] { "Alice", "Bob", "Charlie", "David", "Eve" };
            var numbers = new[] { "+15551234567", "+15559876543", "+15555555555", "+15551112222", "+15559998888" };

            var startDate = endDate.AddDays(-daysBack);
            var rangeTicks = (endDate - startDate).Ticks;

            for (int i = 0; i < count; i++)
            {
                var contactIdx = random.Next(contacts.Length);
                var callType = random.Next(1, 6); // 1-5
                var duration = callType == 3 || callType == 5 ? 0 : random.Next(10, 3600); // Missed/Rejected = 0s
                
                var callDateTicks = startDate.Ticks + (long)(random.NextDouble() * rangeTicks);
                var callDate = new DateTime(callDateTicks);

                logs.Add(new CallLogModel
                {
                    RawCallLogId = i + 1,
                    PhoneNumber = numbers[contactIdx],
                    ContactName = contacts[contactIdx],
                    CallType = callType,
                    Duration = duration,
                    CallDate = callDate,
                    StartTime = callDate,
                    EndTime = callDate.AddSeconds(duration),
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                });
            }

            return logs;
        }
    }
}
