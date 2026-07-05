using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.AppCompat.App;
using DailyTaskSheet.App.Interfaces;

namespace DailyTaskSheet.App.Activities
{
    /// <summary>
    /// Splash Activity serving as the application entry point.
    /// Checks authentication state and routes to the appropriate screen.
    /// </summary>
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, NoHistory = true, Exported = true)]
    public class SplashActivity : AppCompatActivity
    {
        protected override async void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_splash);

            // Initialize database asynchronously without blocking the UI thread
            var app = (App)Application;
            var dbService = app.GetService<DailyTaskSheet.App.SQLite.DatabaseService>();
            if (dbService != null)
            {
                await dbService.InitializeAsync();
            }

            // Minimum delay to show splash screen smoothly
            await Task.Delay(1500);

            RouteToNextActivity();
        }

        private void RouteToNextActivity()
        {
            var app = (App)Application;
            var authService = app.GetService<IAuthenticationService>();

            Intent intent;
            if (authService != null && authService.IsAuthenticated)
            {
                intent = new Intent(this, typeof(DashboardActivity));
            }
            else
            {
                intent = new Intent(this, typeof(LoginActivity));
            }

            StartActivity(intent);
            Finish(); // Ensure user can't navigate back to splash
        }
    }
}
