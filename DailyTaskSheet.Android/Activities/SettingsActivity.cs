using System;
using System.ComponentModel;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using DailyTaskSheet.App.ViewModels;
using Google.Android.Material.SwitchMaterial;

namespace DailyTaskSheet.App.Activities
{
    /// <summary>
    /// Activity providing configuration options for the application.
    /// Connects to SettingsViewModel.
    /// </summary>
    [Activity(Label = "Settings", Theme = "@style/AppTheme", ParentActivity = typeof(DashboardActivity))]
    public class SettingsActivity : AppCompatActivity
    {
        private SettingsViewModel _viewModel = null!;
        private SwitchMaterial _bgSyncSwitch = null!;
        private SwitchMaterial _wifiOnlySwitch = null!;
        private Button _logoutButton = null!;
        private Button _selectFolderButton = null!;
        private TextView _selectedFolderText = null!;
        
        private const int RequestCodeOpenDocumentTree = 42;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_settings);

            var toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar?.SetDisplayHomeAsUpEnabled(true);

            var app = (App)Application;
            _viewModel = app.GetService<SettingsViewModel>()!;

            FindViews();
            BindEvents();

            _viewModel.LoadSettings();
        }

        public override bool OnSupportNavigateUp()
        {
            Finish();
            return true;
        }

        private void FindViews()
        {
            _bgSyncSwitch = FindViewById<SwitchMaterial>(Resource.Id.bgSyncSwitch)!;
            _wifiOnlySwitch = FindViewById<SwitchMaterial>(Resource.Id.wifiOnlySwitch)!;
            _logoutButton = FindViewById<Button>(Resource.Id.logoutButton)!;
            _selectFolderButton = FindViewById<Button>(Resource.Id.selectFolderButton)!;
            _selectedFolderText = FindViewById<TextView>(Resource.Id.selectedFolderText)!;
        }

        private void BindEvents()
        {
            _bgSyncSwitch.CheckedChange += (s, e) => _viewModel.BackgroundSyncEnabled = e.IsChecked;
            _wifiOnlySwitch.CheckedChange += (s, e) => _viewModel.WifiOnly = e.IsChecked;

            _selectFolderButton.Click += (s, e) =>
            {
                var intent = new Intent(Intent.ActionOpenDocumentTree);
                intent.AddFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantPersistableUriPermission);
                StartActivityForResult(intent, RequestCodeOpenDocumentTree);
            };

            _logoutButton.Click += async (s, e) =>
            {
                var dialog = new AndroidX.AppCompat.App.AlertDialog.Builder(this)
                    .SetTitle("Log Out")
                    .SetMessage("Are you sure you want to log out? Local data will be cleared.")
                    .SetPositiveButton("Log Out", async (ds, de) => await _viewModel.LogoutAsync())
                    .SetNegativeButton("Cancel", (ds, de) => { })
                    .Create();
                
                dialog.Show();
            };

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            _viewModel.LogoutRequested += ViewModel_LogoutRequested;
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == RequestCodeOpenDocumentTree && resultCode == Result.Ok && data != null)
            {
                var uri = data.Data;
                if (uri != null)
                {
                    try
                    {
                        var takeFlags = data.Flags & (ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission);
                        ContentResolver?.TakePersistableUriPermission(uri, takeFlags);
                        _viewModel.CustomRecordingPath = uri.ToString();
                    }
                    catch (Exception ex)
                    {
                        Android.Util.Log.Error("SettingsActivity", $"Failed to take persistable URI permission: {ex.Message}");
                    }
                }
            }
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                if (e.PropertyName == nameof(_viewModel.BackgroundSyncEnabled))
                    _bgSyncSwitch.Checked = _viewModel.BackgroundSyncEnabled;
                else if (e.PropertyName == nameof(_viewModel.WifiOnly))
                    _wifiOnlySwitch.Checked = _viewModel.WifiOnly;
                else if (e.PropertyName == nameof(_viewModel.CustomRecordingPath))
                {
                    if (string.IsNullOrEmpty(_viewModel.CustomRecordingPath))
                        _selectedFolderText.Text = "No custom folder selected";
                    else
                        _selectedFolderText.Text = Android.Net.Uri.Decode(_viewModel.CustomRecordingPath) ?? _viewModel.CustomRecordingPath;
                }
            });
        }

        private void ViewModel_LogoutRequested(object? sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                // Stop background sync before leaving
                Managers.SyncManager.CancelAllSync(this);

                var intent = new Intent(this, typeof(LoginActivity));
                intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                StartActivity(intent);
                Finish();
            });
        }

        protected override void OnDestroy()
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
                _viewModel.LogoutRequested -= ViewModel_LogoutRequested;
            }
            base.OnDestroy();
        }
    }
}
