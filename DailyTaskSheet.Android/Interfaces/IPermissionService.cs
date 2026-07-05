using System.Collections.Generic;
using System.Threading.Tasks;

namespace DailyTaskSheet.App.Interfaces
{
    /// <summary>
    /// Service interface for managing Android runtime permissions.
    /// Handles permission requests, rationale dialogs, and settings redirects.
    /// </summary>
    public interface IPermissionService
    {
        /// <summary>Checks if a specific permission is granted.</summary>
        /// <param name="permission">The Android permission string.</param>
        /// <returns>True if the permission is granted.</returns>
        bool IsPermissionGranted(string permission);

        /// <summary>Checks if all required permissions for call log access are granted.</summary>
        /// <returns>True if all required permissions are granted.</returns>
        bool AreCallLogPermissionsGranted();

        /// <summary>Checks if the notification permission is granted (Android 13+).</summary>
        /// <returns>True if the notification permission is granted or not required.</returns>
        bool IsNotificationPermissionGranted();

        /// <summary>Gets a list of all required permissions that haven't been granted yet.</summary>
        /// <returns>List of pending permission strings.</returns>
        List<string> GetPendingPermissions();

        /// <summary>Gets a list of all required permissions for the application.</summary>
        /// <returns>List of all required permission strings.</returns>
        List<string> GetAllRequiredPermissions();

        /// <summary>Checks whether a rationale should be shown for a specific permission.</summary>
        /// <param name="activity">The current activity.</param>
        /// <param name="permission">The Android permission string.</param>
        /// <returns>True if a rationale dialog should be shown.</returns>
        bool ShouldShowRationale(Android.App.Activity activity, string permission);

        /// <summary>Opens the app settings page for manual permission grants.</summary>
        void OpenAppSettings();
    }
}
