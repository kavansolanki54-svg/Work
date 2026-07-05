using System;

namespace DailyTaskSheet.App.Extensions
{
    /// <summary>
    /// Extension methods for string manipulation and formatting.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Returns a default value if the string is null or empty.
        /// </summary>
        public static string OrDefault(this string? value, string defaultValue = "")
        {
            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }

        /// <summary>
        /// Truncates a string to the specified maximum length, appending "..." if truncated.
        /// </summary>
        public static string Truncate(this string? value, int maxLength, string suffix = "...")
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength) return value ?? string.Empty;
            return value.Substring(0, maxLength - suffix.Length) + suffix;
        }

        /// <summary>
        /// Masks a phone number for logging (shows last 4 digits only).
        /// Example: "+919999999999" → "****9999"
        /// </summary>
        public static string MaskPhoneNumber(this string? phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber)) return "Unknown";
            if (phoneNumber.Length <= 4) return new string('*', phoneNumber.Length);
            return new string('*', phoneNumber.Length - 4) + phoneNumber.Substring(phoneNumber.Length - 4);
        }

        /// <summary>
        /// Masks an email address for logging.
        /// Example: "user@example.com" → "u***@example.com"
        /// </summary>
        public static string MaskEmail(this string? email)
        {
            if (string.IsNullOrEmpty(email)) return "Unknown";
            int atIndex = email.IndexOf('@');
            if (atIndex <= 1) return email;
            return email[0] + new string('*', atIndex - 1) + email.Substring(atIndex);
        }

        /// <summary>
        /// Converts the first character of a string to uppercase.
        /// </summary>
        public static string Capitalize(this string? value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            if (value.Length == 1) return value.ToUpper();
            return char.ToUpper(value[0]) + value.Substring(1);
        }

        /// <summary>
        /// Checks if a string contains only digits.
        /// </summary>
        public static bool IsNumeric(this string? value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            foreach (char c in value)
            {
                if (!char.IsDigit(c) && c != '+' && c != '-' && c != ' ') return false;
            }
            return true;
        }
    }
}
