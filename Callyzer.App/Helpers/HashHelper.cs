using System;
using System.Security.Cryptography;
using System.Text;

namespace Callyzer.App.Helpers
{
    /// <summary>
    /// Provides SHA-256 hashing for call log duplicate detection.
    /// Creates a deterministic hash from call log fields to prevent re-uploading.
    /// </summary>
    public static class HashHelper
    {
        /// <summary>
        /// Generates a SHA-256 hash string from a call log's unique identifying fields.
        /// </summary>
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
        public static string ComputeSha256(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = SHA256.HashData(bytes);
            return Convert.ToHexStringLower(hash);
        }
    }
}
