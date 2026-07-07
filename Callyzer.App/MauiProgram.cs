using CommunityToolkit.Maui;
using Callyzer.App.Interfaces;
using Callyzer.App.Models;
using Callyzer.App.Pages;
using Callyzer.App.Repositories;
using Callyzer.App.Services;
using Callyzer.App.SQLite;
using Callyzer.App.ViewModels;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace Callyzer.App;

/// <summary>
/// MAUI application builder — replaces the old App.cs DI container.
/// Registers all services, repositories, view models, and pages.
/// </summary>
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseSkiaSharp()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // ═══════════════════════════════════════════════════════
        // Core Infrastructure
        // ═══════════════════════════════════════════════════════
        builder.Services.AddSingleton<ILoggerService, LoggerService>();
        builder.Services.AddSingleton<DatabaseService>(provider => 
        {
            var logger = provider.GetRequiredService<ILoggerService>();
            var db = new DatabaseService(logger);
            db.InitializeAsync().ConfigureAwait(false);
            return db;
        });

        // ═══════════════════════════════════════════════════════
        // Core Services
        // ═══════════════════════════════════════════════════════
        builder.Services.AddSingleton<IPreferenceService, PreferenceService>();
        builder.Services.AddSingleton<INetworkService, NetworkService>();
        builder.Services.AddSingleton<IAnalyticsService, AnalyticsService>();
        builder.Services.AddSingleton<IExportService, ExportService>();
        builder.Services.AddSingleton<IBackupService, BackupService>();

        // ═══════════════════════════════════════════════════════
        // Repositories
        // ═══════════════════════════════════════════════════════
        builder.Services.AddSingleton<IRepository<CallLogModel>, CallLogRepository>();
        builder.Services.AddSingleton<CallLogRepository>();
        builder.Services.AddSingleton<IAnalyticsRepository, AnalyticsRepository>();

        // ═══════════════════════════════════════════════════════
        // Services
        // ═══════════════════════════════════════════════════════
        builder.Services.AddSingleton<IAnalyticsService, AnalyticsService>();
        builder.Services.AddSingleton<ISyncService, SyncService>();

        // ═══════════════════════════════════════════════════════
        // Platform-Specific Services
        // (Registered via Platforms/Android or Platforms/iOS)
        // ═══════════════════════════════════════════════════════
        builder.Services.AddSingleton<IDeviceManagementService, DeviceManagementService>();

#if ANDROID
        builder.Services.AddSingleton<ICallLogPlatformService,
            Callyzer.App.Platforms.Android.Services.AndroidCallLogService>();
#elif IOS
        builder.Services.AddSingleton<ICallLogPlatformService,
            Callyzer.App.Platforms.iOS.Services.iOSCallLogService>();
#else
        // Fallback stub for Windows/Mac Catalyst during development
        builder.Services.AddSingleton<ICallLogPlatformService, StubCallLogPlatformService>();
#endif

        // ═══════════════════════════════════════════════════════
        // ViewModels
        // ═══════════════════════════════════════════════════════
        builder.Services.AddTransient<SplashViewModel>();
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<AnalyticsViewModel>();
        builder.Services.AddTransient<CallHistoryViewModel>();
        builder.Services.AddTransient<ContactDetailViewModel>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<SyncHistoryViewModel>();
        builder.Services.AddTransient<CompareContactsViewModel>();
        builder.Services.AddTransient<ExportViewModel>();
        builder.Services.AddTransient<BackupRestoreViewModel>();

        // ═══════════════════════════════════════════════════════
        // Pages
        // ═══════════════════════════════════════════════════════
        builder.Services.AddTransient<SplashPage>();
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<AnalyticsPage>();
        builder.Services.AddTransient<CallHistoryPage>();
        builder.Services.AddTransient<ContactDetailPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<SettingsPage>();
        builder.Services.AddTransient<SyncHistoryPage>();
        builder.Services.AddTransient<CompareContactsPage>();
        builder.Services.AddTransient<ExportPage>();
        builder.Services.AddTransient<BackupRestorePage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

/// <summary>
/// Stub platform service for desktop targets during development.
/// </summary>
internal class StubCallLogPlatformService : ICallLogPlatformService
{
    public bool IsCallLogAccessSupported => false;
    public bool HasRequiredPermissions => false;
    public Task<List<CallLogModel>> ReadNewCallLogsAsync(long lastProcessedId, CancellationToken ct = default)
        => Task.FromResult(new List<CallLogModel>());
    public Task<List<CallLogModel>> ReadCallLogsByDateAsync(long fromTs, long toTs, CancellationToken ct = default)
        => Task.FromResult(new List<CallLogModel>());
    public Task<int> GetTotalDeviceCallLogsAsync(CancellationToken ct = default) => Task.FromResult(0);
    public Task<bool> RequestPermissionsAsync() => Task.FromResult(false);
}
