using System;
using System.Threading.Tasks;

namespace DailyTaskSheet.App.Interfaces
{
    public interface INativeRecordingScannerService
    {
        /// <summary>
        /// Searches the device for an audio file that matches the provided phone number and start time.
        /// Native dialers usually include the phone number in the file name or create it around the call time.
        /// </summary>
        Task<string?> FindRecordingPathAsync(string phoneNumber, DateTime callStartTime);
    }
}
