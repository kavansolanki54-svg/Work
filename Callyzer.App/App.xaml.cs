using Callyzer.App.SQLite;
using Callyzer.App.Pages;

namespace Callyzer.App;

/// <summary>
/// MAUI Application entry point. Initializes the database on startup.
/// </summary>
public partial class App : Application
{
    private readonly DatabaseService _dbService;

    public App(DatabaseService dbService, SplashPage splashPage)
    {
        InitializeComponent();

        _dbService = dbService;
        MainPage = splashPage;
    }

    protected override void OnStart()
    {
        base.OnStart();
        
        // Run startup logic in background to not block UI
        _ = Task.Run(async () =>
        {
            try
            {
                await _dbService.InitializeAsync();
                
                var deviceService = Handler?.MauiContext?.Services.GetService<Callyzer.App.Interfaces.IDeviceManagementService>();
                if (deviceService != null)
                {
                    await deviceService.RegisterDeviceAsync();
                }
                System.Diagnostics.Debug.WriteLine("[App] Database initialized successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[App] Database initialization failed: {ex.Message}");
            }
        });
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(MainPage!);

        return window;
    }
}