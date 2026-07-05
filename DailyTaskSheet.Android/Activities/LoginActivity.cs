using System;
using System.ComponentModel;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using DailyTaskSheet.App.ViewModels;

namespace DailyTaskSheet.App.Activities
{
    /// <summary>
    /// Login Activity providing the user interface for authentication.
    /// Connects to LoginViewModel to process login logic.
    /// </summary>
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false, NoHistory = true)]
    public class LoginActivity : AppCompatActivity
    {
        private LoginViewModel _viewModel = null!;
        private EditText _emailEditText = null!;
        private EditText _passwordEditText = null!;
        private Button _loginButton = null!;
        private TextView _errorTextView = null!;
        private ProgressBar _loginProgressBar = null!;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set the view from the "main" layout resource
            SetContentView(Resource.Layout.activity_login);

            // Get ViewModel from DI
            var app = (App)Application;
            _viewModel = app.GetService<LoginViewModel>()!;

            // If user is already authenticated, redirect to Dashboard
            var authService = app.GetService<Interfaces.IAuthenticationService>();
            if (authService != null && authService.IsAuthenticated)
            {
                NavigateToDashboard();
                return;
            }

            FindViews();
            BindEvents();
        }

        private void FindViews()
        {
            _emailEditText = FindViewById<EditText>(Resource.Id.emailEditText)!;
            _passwordEditText = FindViewById<EditText>(Resource.Id.passwordEditText)!;
            _loginButton = FindViewById<Button>(Resource.Id.loginButton)!;
            _errorTextView = FindViewById<TextView>(Resource.Id.errorTextView)!;
            _loginProgressBar = FindViewById<ProgressBar>(Resource.Id.loginProgressBar)!;
        }

        private void BindEvents()
        {
            _loginButton.Click += async (s, e) =>
            {
                _viewModel.Email = _emailEditText.Text ?? string.Empty;
                _viewModel.Password = _passwordEditText.Text ?? string.Empty;
                await _viewModel.LoginAsync();
            };

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            _viewModel.LoginSucceeded += ViewModel_LoginSucceeded;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                if (e.PropertyName == nameof(_viewModel.IsBusy))
                {
                    _loginProgressBar.Visibility = _viewModel.IsBusy ? ViewStates.Visible : ViewStates.Gone;
                    _loginButton.Enabled = !_viewModel.IsBusy;
                    _emailEditText.Enabled = !_viewModel.IsBusy;
                    _passwordEditText.Enabled = !_viewModel.IsBusy;
                }
                else if (e.PropertyName == nameof(_viewModel.ErrorMessage))
                {
                    if (!string.IsNullOrEmpty(_viewModel.ErrorMessage))
                    {
                        _errorTextView.Text = _viewModel.ErrorMessage;
                        _errorTextView.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        _errorTextView.Visibility = ViewStates.Gone;
                    }
                }
            });
        }

        private void ViewModel_LoginSucceeded(object? sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                NavigateToDashboard();
            });
        }

        private void NavigateToDashboard()
        {
            var intent = new Intent(this, typeof(DashboardActivity));
            StartActivity(intent);
            Finish();
        }

        protected override void OnDestroy()
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
                _viewModel.LoginSucceeded -= ViewModel_LoginSucceeded;
            }
            base.OnDestroy();
        }
    }
}
