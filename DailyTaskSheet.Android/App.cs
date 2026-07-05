using System;
using Android.App;
using Android.Runtime;
using DailyTaskSheet.App.Constants;
using DailyTaskSheet.App.Interfaces;
using DailyTaskSheet.App.Managers;
using DailyTaskSheet.App.Repositories;
using DailyTaskSheet.App.Services;
using DailyTaskSheet.App.SQLite;
using Microsoft.Extensions.DependencyInjection;

namespace DailyTaskSheet.App
{
    /// <summary>
    /// Custom Android Application class.
    /// Serves as the entry point for the application lifecycle,
    /// initializes Dependency Injection, the SQLite database, and background services.
    /// </summary>
    [Application(Label = "@string/app_name", Theme = "@style/AppTheme", AllowBackup = true, NetworkSecurityConfig = "@xml/network_security_config")]
    public class App : Application
    {
        private static IServiceProvider? _serviceProvider;

        /// <summary>
        /// Gets the global ServiceProvider for resolving dependencies.
        /// </summary>
        public static IServiceProvider ServiceProvider => _serviceProvider ?? throw new InvalidOperationException("ServiceProvider is not initialized.");

        /// <summary>
        /// Default constructor required by the Android OS for reflection.
        /// </summary>
        public App()
        {
        }

        /// <summary>
        /// Required constructor for Android Application subclasses.
        /// </summary>
        public App(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        /// <inheritdoc />
        public override void OnCreate()
        {
            base.OnCreate();

            try
            {
                // 1. Initialize Dependency Injection
                ConfigureServices();

                // 2. Database is now initialized asynchronously in SplashActivity to prevent main thread deadlocks

                // 3. Initialize Notification Channels (required for Android 8+)
                var notificationService = GetService<INotificationService>();
                notificationService?.CreateNotificationChannels();

                // 4. Initialize WorkManager for background tasks
                SyncManager.InitializeWorkManager(this);

                // 5. Check if background sync should be running
                var authService = GetService<IAuthenticationService>();
                var prefService = GetService<IPreferenceService>();
                
                if (authService != null && authService.IsAuthenticated)
                {
                    bool isBgSyncEnabled = prefService?.GetBool(AppConstants.PrefKeyBackgroundSyncEnabled, true) ?? true;
                    if (isBgSyncEnabled)
                    {
                        int interval = prefService?.GetInt(AppConstants.PrefKeySyncInterval, AppConstants.DefaultSyncIntervalMinutes) ?? AppConstants.DefaultSyncIntervalMinutes;
                        SyncManager.SchedulePeriodicSync(this, interval);
                    }
                }

                Android.Util.Log.Info("DailyTaskSheetApp", "Application initialization complete.");
            }
            catch (Exception ex)
            {
                Android.Util.Log.Error("DailyTaskSheetApp", $"Critical initialization error: {ex.Message}");
            }
        }

        /// <summary>
        /// Configures all dependency injection services and repositories.
        /// </summary>
        private void ConfigureServices()
        {
            var services = new ServiceCollection();

            // Core / Platform
            services.AddSingleton<Android.Content.Context>(this);
            
            // Database
            services.AddSingleton<DatabaseService>();

            // Repositories
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<ICallLogRepository, CallLogRepository>();
            services.AddSingleton<ISyncRepository, SyncRepository>();
            services.AddSingleton<ISettingsRepository, SettingsRepository>();
            services.AddSingleton<ILogRepository, LogRepository>();

            // Services
            services.AddSingleton<IPreferenceService, PreferenceService>();
            
            // Note: Use Android ID or similar stable ID for encryption key material in production
            string deviceId = Android.Provider.Settings.Secure.GetString(ContentResolver, Android.Provider.Settings.Secure.AndroidId) ?? "default_secure_key_123";
            services.AddSingleton<IEncryptionService>(new EncryptionService(deviceId));
            
            services.AddSingleton<ILoggerService, LoggerService>();
            services.AddSingleton<IDeviceService, DeviceService>();
            services.AddSingleton<INetworkService, NetworkService>();
            services.AddSingleton<IPermissionService, PermissionService>();
            services.AddSingleton<INotificationService, NotificationService>();
            
            services.AddSingleton<System.Net.Http.HttpClient>();
            services.AddSingleton<IApiClient, ApiClient>();
            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<ICallLogReaderService, CallLogReaderService>();
            services.AddSingleton<INativeRecordingScannerService, NativeRecordingScannerService>();
            services.AddSingleton<ISyncService, SyncService>();

            // ViewModels
            services.AddTransient<ViewModels.LoginViewModel>();
            services.AddTransient<ViewModels.DashboardViewModel>();
            services.AddTransient<ViewModels.CallHistoryViewModel>();
            services.AddTransient<ViewModels.SettingsViewModel>();
            services.AddTransient<ViewModels.SyncHistoryViewModel>();

            _serviceProvider = services.BuildServiceProvider();
        }

        /// <summary>
        /// Helper method to retrieve a registered service.
        /// </summary>
        public T? GetService<T>() where T : class
        {
            return _serviceProvider?.GetService<T>();
        }
    }
}
