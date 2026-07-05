using System;
using System.Collections.Generic;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Net;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using DailyTaskSheet.App.Interfaces;

namespace DailyTaskSheet.App.Services
{
    /// <summary>
    /// Manages Android runtime permissions for call log access, notifications, and other features.
    /// Handles permission state checking, rationale display, and app settings navigation.
    /// </summary>
    public class PermissionService : IPermissionService
    {
        private readonly Context _context;
        private readonly ILoggerService _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="PermissionService"/>.
        /// </summary>
        public PermissionService(Context context, ILoggerService logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <inheritdoc />
        public bool IsPermissionGranted(string permission)
        {
            return ContextCompat.CheckSelfPermission(_context, permission) == Permission.Granted;
        }

        /// <inheritdoc />
        public bool AreCallLogPermissionsGranted()
        {
            return IsPermissionGranted(Manifest.Permission.ReadCallLog) &&
                   IsPermissionGranted(Manifest.Permission.ReadPhoneState);
        }

        /// <inheritdoc />
        public bool IsNotificationPermissionGranted()
        {
            // POST_NOTIFICATIONS permission is only required on Android 13+ (API 33)
            if (Build.VERSION.SdkInt < BuildVersionCodes.Tiramisu) return true;
            return IsPermissionGranted(Manifest.Permission.PostNotifications);
        }

        /// <inheritdoc />
        public List<string> GetPendingPermissions()
        {
            var pending = new List<string>();
            foreach (var permission in GetAllRequiredPermissions())
            {
                if (!IsPermissionGranted(permission))
                {
                    pending.Add(permission);
                }
            }
            return pending;
        }

        /// <inheritdoc />
        public List<string> GetAllRequiredPermissions()
        {
            var permissions = new List<string>
            {
                Manifest.Permission.ReadCallLog,
                Manifest.Permission.ReadPhoneState,
                Manifest.Permission.ReadContacts
            };

            // READ_PHONE_NUMBERS requires API 26+
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                permissions.Add(Manifest.Permission.ReadPhoneNumbers);
            }

            // POST_NOTIFICATIONS and READ_MEDIA_AUDIO require API 33+
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
            {
                permissions.Add(Manifest.Permission.PostNotifications);
                permissions.Add(Manifest.Permission.ReadMediaAudio);
            }
            else
            {
                permissions.Add(Manifest.Permission.ReadExternalStorage);
            }

            return permissions;
        }

        /// <inheritdoc />
        public bool ShouldShowRationale(Activity activity, string permission)
        {
            return ActivityCompat.ShouldShowRequestPermissionRationale(activity, permission);
        }

        /// <inheritdoc />
        public void OpenAppSettings()
        {
            try
            {
                var intent = new Intent(Android.Provider.Settings.ActionApplicationDetailsSettings);
                intent.SetData(Android.Net.Uri.Parse($"package:{_context.PackageName}"));
                intent.AddFlags(ActivityFlags.NewTask);
                _context.StartActivity(intent);
            }
            catch (Exception ex)
            {
                _logger.Error("PermissionService", "Failed to open app settings", ex);
                // Fallback: open general settings
                try
                {
                    var fallbackIntent = new Intent(Android.Provider.Settings.ActionSettings);
                    fallbackIntent.AddFlags(ActivityFlags.NewTask);
                    _context.StartActivity(fallbackIntent);
                }
                catch (Exception fallbackEx)
                {
                    _logger.Error("PermissionService", "Failed to open settings", fallbackEx);
                }
            }
        }
    }
}
