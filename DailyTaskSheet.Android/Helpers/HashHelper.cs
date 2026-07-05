using System;
using System.Security.Cryptography;
using System.Text;

namespace DailyTaskSheet.App.Helpers
{
    /// <summary>
    /// Provides SHA-256 hashing for call log duplicate detection.
    /// Creates a deterministic hash from call log fields to prevent re-uploading.
    /// </summary>
    public static class HashHelper
    {
        /// <summary>
        /// Generates a SHA-256 hash string from a call log's unique identifying fields.
        /// The combination of phone number, call date, duration, and type uniquely identifies a call.
        /// </summary>
        /// <param name="phoneNumber">The phone number.</param>
        /// <param name="callDate">The call date/time.</param>
        /// <param name="duration">The call duration in seconds.</param>
        /// <param name="callType">The call type integer value.</param>
        /// <param name="rawCallLogId">The raw Android call log ID.</param>
        /// <returns>A 64-character lowercase hex SHA-256 hash string.</returns>
        public static string GenerateCallLogHash(
            string phoneNumber,
            DateTime callDate,
            int duration,
            int callType,
            long rawCallLogId)
        {
            var input = $"{phoneNumber}|{callDate:O}|{duration}|{callType}|{rawCallLogId}";
            return ComputeSha256(input);
        }

        /// <summary>
        /// Computes the SHA-256 hash of a string input.
        /// </summary>
        /// <param name="input">The input string to hash.</param>
        /// <returns>A 64-character lowercase hex hash string.</returns>
        public static string ComputeSha256(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                byte[] hash = sha256.ComputeHash(bytes);
                var builder = new StringBuilder(64);
                for (int i = 0; i < hash.Length; i++)
                {
                    builder.Append(hash[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
