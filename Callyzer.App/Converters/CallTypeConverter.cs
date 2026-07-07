using Callyzer.App.Enums;

namespace Callyzer.App.Converters
{
    /// <summary>
    /// Converts between platform call type integers and the application's CallTypeEnum.
    /// </summary>
    public static class CallTypeConverter
    {
        /// <summary>
        /// Converts an Android CallLog.Calls type integer to a CallTypeEnum.
        /// </summary>
        public static CallTypeEnum FromAndroidType(int androidCallType)
        {
            return androidCallType switch
            {
                1 => CallTypeEnum.Incoming,
                2 => CallTypeEnum.Outgoing,
                3 => CallTypeEnum.Missed,
                4 => CallTypeEnum.Voicemail,
                5 => CallTypeEnum.Rejected,
                6 => CallTypeEnum.Blocked,
                7 => CallTypeEnum.AnsweredExternally,
                _ => CallTypeEnum.Unknown
            };
        }

        /// <summary>
        /// Converts a CallTypeEnum to its display string.
        /// </summary>
        public static string ToDisplayString(CallTypeEnum callType)
        {
            return callType switch
            {
                CallTypeEnum.Incoming => "Incoming",
                CallTypeEnum.Outgoing => "Outgoing",
                CallTypeEnum.Missed => "Missed",
                CallTypeEnum.Voicemail => "Voicemail",
                CallTypeEnum.Rejected => "Rejected",
                CallTypeEnum.Blocked => "Blocked",
                CallTypeEnum.AnsweredExternally => "Answered Externally",
                _ => "Unknown"
            };
        }

        /// <summary>
        /// Converts a CallTypeEnum to its API string representation.
        /// </summary>
        public static string ToApiString(CallTypeEnum callType) => callType.ToString();

        /// <summary>
        /// Gets the icon resource name for a call type.
        /// </summary>
        public static string GetIconResourceName(CallTypeEnum callType)
        {
            return callType switch
            {
                CallTypeEnum.Incoming => "ic_call_incoming.svg",
                CallTypeEnum.Outgoing => "ic_call_outgoing.svg",
                CallTypeEnum.Missed => "ic_call_missed.svg",
                CallTypeEnum.Rejected => "ic_call_rejected.svg",
                CallTypeEnum.Blocked => "ic_call_blocked.svg",
                _ => "ic_call_default.svg"
            };
        }

        /// <summary>
        /// Gets a color hex code for a call type (for chart rendering).
        /// </summary>
        public static string GetColor(CallTypeEnum callType)
        {
            return callType switch
            {
                CallTypeEnum.Incoming => "#4CAF50",    // Green
                CallTypeEnum.Outgoing => "#2196F3",    // Blue
                CallTypeEnum.Missed => "#F44336",      // Red
                CallTypeEnum.Rejected => "#FF9800",     // Orange
                CallTypeEnum.Blocked => "#9E9E9E",     // Grey
                CallTypeEnum.Voicemail => "#9C27B0",   // Purple
                _ => "#607D8B"                          // Blue Grey
            };
        }
    }
}
