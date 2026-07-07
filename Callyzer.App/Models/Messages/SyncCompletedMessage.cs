namespace Callyzer.App.Models.Messages
{
    public class SyncCompletedMessage
    {
        public bool Success { get; set; }
        public int SyncedCount { get; set; }
    }
}
