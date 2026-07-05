using DailyTaskSheet.App.Enums;

namespace DailyTaskSheet.App.Converters
{
    /// <summary>
    /// Converts between Android call type integer constants and the application's CallTypeEnum.
    /// Maps Android SDK values from CallLog.Calls to strongly-typed enum values.
    /// </summary>
    public static class CallTypeConverter
    {
        /// <summary>
        /// Converts an Android CallLog.Calls type integer to a CallTypeEnum.
        /// </summary>
        /// <param name="androidCallType">The integer type from the Android call log provider.</param>
        /// <returns>The corresponding CallTypeEnum value.</returns>
        public static CallTypeEnum FromAndroidType(int androidCallType)
        {
            return androidCallType switch
            {
                1 => CallTypeEnum.Incoming,       // CallLog.Calls.IncomingType
                2 => CallTypeEnum.Outgoing,       // CallLog.Calls.OutgoingType
                3 => CallTypeEnum.Missed,         // CallLog.Calls.MissedType
                4 => CallTypeEnum.Voicemail,      // CallLog.Calls.VoicemailType
                5 => CallTypeEnum.Rejected,       // CallLog.Calls.RejectedType
                6 => CallTypeEnum.Blocked,        // CallLog.Calls.BlockedType
                7 => CallTypeEnum.AnsweredExternally, // CallLog.Calls.AnsweredExternallyType
                _ => CallTypeEnum.Unknown
            };
        }

        /// <summary>
        /// Converts a CallTypeEnum to its display string.
        /// </summary>
        /// <param name="callType">The call type enum value.</param>
        /// <returns>A human-readable string representation.</returns>
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
        /// <param name="callType">The call type enum value.</param>
        /// <returns>The API-compatible string representation.</returns>
        public static string ToApiString(CallTypeEnum callType)
        {
            return callType.ToString();
        }

        /// <summary>
        /// Gets the icon resource name for a call type (for UI display).
        /// </summary>
        /// <param name="callType">The call type enum value.</param>
        /// <returns>The drawable resource name.</returns>
        public static string GetIconResourceName(CallTypeEnum callType)
        {
            return callType switch
            {
                CallTypeEnum.Incoming => "ic_call_incoming",
                CallTypeEnum.Outgoing => "ic_call_outgoing",
                CallTypeEnum.Missed => "ic_call_missed",
                CallTypeEnum.Rejected => "ic_call_rejected",
                CallTypeEnum.Blocked => "ic_call_blocked",
                _ => "ic_call_default"
            };
        }
    }
}
