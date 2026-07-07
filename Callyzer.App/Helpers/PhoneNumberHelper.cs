using System;

namespace Callyzer.App.Helpers
{
    /// <summary>
    /// Provides phone number normalization and formatting utilities.
    /// Uses libphonenumber-csharp for robust parsing when available.
    /// </summary>
    public static class PhoneNumberHelper
    {
        /// <summary>
        /// Normalizes a phone number by stripping non-digit characters (except leading +).
        /// </summary>
        public static string Normalize(string? phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber)) return string.Empty;

            var sb = new System.Text.StringBuilder(phoneNumber.Length);
            for (int i = 0; i < phoneNumber.Length; i++)
            {
                char c = phoneNumber[i];
                if (char.IsDigit(c) || (c == '+' && i == 0))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Extracts the last N digits of a phone number for comparison.
        /// Useful when country codes may differ.
        /// </summary>
        public static string GetLastNDigits(string? phoneNumber, int n = 10)
        {
            var normalized = Normalize(phoneNumber);
            if (normalized.Length <= n) return normalized;
            return normalized.Substring(normalized.Length - n);
        }

        /// <summary>
        /// Checks if two phone numbers match (ignoring country code differences).
        /// </summary>
        public static bool AreNumbersEqual(string? number1, string? number2, int compareDigits = 10)
        {
            var n1 = GetLastNDigits(number1, compareDigits);
            var n2 = GetLastNDigits(number2, compareDigits);
            return string.Equals(n1, n2, StringComparison.Ordinal);
        }

        /// <summary>
        /// Formats a phone number for display.
        /// </summary>
        public static string FormatForDisplay(string? phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber)) return "Unknown";
            var normalized = Normalize(phoneNumber);
            if (normalized.Length == 10)
            {
                return $"{normalized[..5]} {normalized[5..]}";
            }
            if (normalized.Length > 10 && normalized.StartsWith("+"))
            {
                var countryCode = normalized[..^10];
                var local = normalized[^10..];
                return $"{countryCode} {local[..5]} {local[5..]}";
            }
            return phoneNumber;
        }

        /// <summary>
        /// Gets the initials from a contact name for avatar display.
        /// </summary>
        public static string GetInitials(string? name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "?";
            var parts = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
                return $"{char.ToUpper(parts[0][0])}{char.ToUpper(parts[1][0])}";
            return char.ToUpper(parts[0][0]).ToString();
        }
    }
}
