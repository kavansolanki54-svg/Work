namespace DailyTaskSheet.App.Enums
{
    /// <summary>
    /// Represents the type of a phone call as recorded in the Android call log.
    /// Maps directly to <see cref="Android.Provider.CallLog.Calls"/> type constants.
    /// </summary>
    public enum CallTypeEnum
    {
        /// <summary>Unknown or unrecognized call type.</summary>
        Unknown = 0,

        /// <summary>An incoming call that was answered.</summary>
        Incoming = 1,

        /// <summary>An outgoing call placed by the user.</summary>
        Outgoing = 2,

        /// <summary>A missed incoming call.</summary>
        Missed = 3,

        /// <summary>A voicemail message.</summary>
        Voicemail = 4,

        /// <summary>A call that was rejected by the user.</summary>
        Rejected = 5,

        /// <summary>A call that was blocked by the user or system.</summary>
        Blocked = 6,

        /// <summary>A call that was answered on another device (multi-device).</summary>
        AnsweredExternally = 7
    }
}
