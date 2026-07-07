namespace Callyzer.App.Enums
{
    /// <summary>
    /// Represents the synchronization status of a call log record.
    /// Tracks the lifecycle of a record from capture through upload.
    /// </summary>
    public enum SyncStatusEnum
    {
        /// <summary>Record captured but not yet attempted for sync.</summary>
        Pending = 0,

        /// <summary>Sync currently in progress for this record.</summary>
        Syncing = 1,

        /// <summary>Record successfully uploaded to the API.</summary>
        Synced = 2,

        /// <summary>Upload attempt failed; queued for retry.</summary>
        Failed = 3,

        /// <summary>Server reported this record as a duplicate.</summary>
        Duplicate = 4
    }
}
