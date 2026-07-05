using System;
using System.ComponentModel;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.CardView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using DailyTaskSheet.App.Interfaces;
using DailyTaskSheet.App.ViewModels;

namespace DailyTaskSheet.App.Activities
{
    /// <summary>
    /// Dashboard Activity providing the main overview, sync status, and navigation to other screens.
    /// Triggers permission requests upon first launch.
    /// </summary>
    [Activity(Label = "Dashboard", Theme = "@style/AppTheme")]
    public class DashboardActivity : AppCompatActivity
    {
        private DashboardViewModel _viewModel = null!;
        private IPermissionService _permissionService = null!;
        
        private SwipeRefreshLayout _swipeRefreshLayout = null!;
        private TextView _employeeNameTextView = null!;
        private TextView _companyNameTextView = null!;
        private TextView _lastSyncTextView = null!;
        private TextView _nextSyncTextView = null!;
        private TextView _pendingCountTextView = null!;
        private Button _syncNowButton = null!;
        private CardView _callHistoryCard = null!;
        private CardView _syncHistoryCard = null!;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            var toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            var app = (App)Application;
            _viewModel = app.GetService<DashboardViewModel>()!;
            _permissionService = app.GetService<IPermissionService>()!;

            FindViews();
            BindEvents();
            
            // Initial load
            _ = _viewModel.LoadDataAsync();
            
            // Check permissions
            CheckAndRequestPermissions();
        }

        protected override void OnResume()
        {
            base.OnResume();
            _ = _viewModel.LoadDataAsync();
        }

        public override bool OnCreateOptionsMenu(IMenu? menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_dashboard, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.action_settings)
            {
                StartActivity(new Intent(this, typeof(SettingsActivity)));
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void FindViews()
        {
            _swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout)!;
            _employeeNameTextView = FindViewById<TextView>(Resource.Id.employeeNameTextView)!;
            _companyNameTextView = FindViewById<TextView>(Resource.Id.companyNameTextView)!;
            _lastSyncTextView = FindViewById<TextView>(Resource.Id.lastSyncTextView)!;
            _nextSyncTextView = FindViewById<TextView>(Resource.Id.nextSyncTextView)!;
            _pendingCountTextView = FindViewById<TextView>(Resource.Id.pendingCountTextView)!;
            _syncNowButton = FindViewById<Button>(Resource.Id.syncNowButton)!;
            _callHistoryCard = FindViewById<CardView>(Resource.Id.callHistoryCard)!;
            _syncHistoryCard = FindViewById<CardView>(Resource.Id.syncHistoryCard)!;
        }

        private void BindEvents()
        {
            _swipeRefreshLayout.Refresh += async (s, e) =>
            {
                await _viewModel.LoadDataAsync();
                _swipeRefreshLayout.Refreshing = false;
            };

            _syncNowButton.Click += async (s, e) =>
            {
                if (!_permissionService.AreCallLogPermissionsGranted())
                {
                    Toast.MakeText(this, "Call Log permissions required to sync.", ToastLength.Long)?.Show();
                    CheckAndRequestPermissions();
                    return;
                }
                
                await _viewModel.SyncNowAsync();
            };

            _callHistoryCard.Click += (s, e) => StartActivity(new Intent(this, typeof(CallHistoryActivity)));
            _syncHistoryCard.Click += (s, e) => StartActivity(new Intent(this, typeof(SyncHistoryActivity)));

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                if (e.PropertyName == nameof(_viewModel.EmployeeName))
                    _employeeNameTextView.Text = _viewModel.EmployeeName;
                else if (e.PropertyName == nameof(_viewModel.CompanyName))
                    _companyNameTextView.Text = _viewModel.CompanyName;
                else if (e.PropertyName == nameof(_viewModel.LastSyncTime))
                    _lastSyncTextView.Text = _viewModel.LastSyncTime;
                else if (e.PropertyName == nameof(_viewModel.NextSyncTime))
                    _nextSyncTextView.Text = _viewModel.NextSyncTime;
                else if (e.PropertyName == nameof(_viewModel.PendingCount))
                    _pendingCountTextView.Text = _viewModel.PendingCount.ToString();
                else if (e.PropertyName == nameof(_viewModel.IsSyncingNow))
                {
                    _syncNowButton.Enabled = !_viewModel.IsSyncingNow;
                    _syncNowButton.Text = _viewModel.IsSyncingNow ? "Syncing..." : "Sync Now";
                }
                else if (e.PropertyName == nameof(_viewModel.ErrorMessage))
                {
                    if (!string.IsNullOrEmpty(_viewModel.ErrorMessage))
                        Toast.MakeText(this, _viewModel.ErrorMessage, ToastLength.Long)?.Show();
                }
            });
        }

        private void CheckAndRequestPermissions()
        {
            var pending = _permissionService.GetPendingPermissions();
            if (pending.Count > 0)
            {
                RequestPermissions(pending.ToArray(), 100);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            
            if (requestCode == 100)
            {
                bool allGranted = true;
                foreach (var result in grantResults)
                {
                    if (result != Permission.Granted) allGranted = false;
                }

                if (!allGranted)
                {
                    Toast.MakeText(this, "Permissions are required for the app to function properly.", ToastLength.Long)?.Show();
                }
                else
                {
                    // If permissions just granted, kick off an initial sync
                    _ = _viewModel.SyncNowAsync();
                }
            }
        }

        protected override void OnDestroy()
        {
            if (_viewModel != null) _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            base.OnDestroy();
        }
    }
}
