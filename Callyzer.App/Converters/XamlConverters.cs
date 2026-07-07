using System;
using System.Globalization;
using Callyzer.App.Enums;
using Callyzer.App.Extensions;

namespace Callyzer.App.Converters
{
    /// <summary>
    /// XAML binding converter: converts call duration in seconds to a human-readable string.
    /// </summary>
    public class DurationDisplayConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int seconds)
                return DateTimeExtensions.FormatDuration(seconds);
            if (value is long longSeconds)
                return DateTimeExtensions.FormatDurationCompact(longSeconds);
            return "0s";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    /// <summary>
    /// XAML binding converter: converts a CallTypeEnum integer to a color for UI display.
    /// </summary>
    public class CallTypeToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int typeInt)
            {
                var callType = (CallTypeEnum)typeInt;
                return Color.FromArgb(CallTypeConverter.GetColor(callType));
            }
            return Colors.Grey;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    /// <summary>
    /// XAML binding converter: converts a CallTypeEnum integer to the icon filename.
    /// </summary>
    public class CallTypeToIconConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int typeInt)
            {
                var callType = (CallTypeEnum)typeInt;
                return CallTypeConverter.GetIconResourceName(callType);
            }
            return "ic_call_default.svg";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    /// <summary>
    /// XAML binding converter: converts a boolean to a Color (e.g., synced → green, pending → orange).
    /// </summary>
    public class BoolToColorConverter : IValueConverter
    {
        public Color TrueColor { get; set; } = Colors.Green;
        public Color FalseColor { get; set; } = Colors.Orange;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool b)
                return b ? TrueColor : FalseColor;
            return FalseColor;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
