using System;
using System.ComponentModel;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using DailyTaskSheet.App.Adapters;
using DailyTaskSheet.App.ViewModels;

namespace DailyTaskSheet.App.Activities
{
    /// <summary>
    /// Activity displaying the history of background sync operations.
    /// </summary>
    [Activity(Label = "Sync History", Theme = "@style/AppTheme", ParentActivity = typeof(DashboardActivity))]
    public class SyncHistoryActivity : AppCompatActivity
    {
        private SyncHistoryViewModel _viewModel = null!;
        private SwipeRefreshLayout _swipeRefreshLayout = null!;
        private RecyclerView _recyclerView = null!;
        private ProgressBar _progressBar = null!;
        private TextView _emptyTextView = null!;
        private SyncHistoryAdapter _adapter = null!;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_sync_history);

            var toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar?.SetDisplayHomeAsUpEnabled(true);

            var app = (App)Application;
            _viewModel = app.GetService<SyncHistoryViewModel>()!;

            FindViews();
            SetupRecyclerView();
            BindEvents();

            _ = _viewModel.LoadAsync();
        }

        public override bool OnSupportNavigateUp()
        {
            Finish();
            return true;
        }

        private void FindViews()
        {
            _swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout)!;
            _recyclerView = FindViewById<RecyclerView>(Resource.Id.syncHistoryRecyclerView)!;
            _progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar)!;
            _emptyTextView = FindViewById<TextView>(Resource.Id.emptyTextView)!;
        }

        private void SetupRecyclerView()
        {
            _recyclerView.SetLayoutManager(new LinearLayoutManager(this));
            _adapter = new SyncHistoryAdapter(_viewModel.SyncHistory);
            _recyclerView.SetAdapter(_adapter);
        }

        private void BindEvents()
        {
            _swipeRefreshLayout.Refresh += async (s, e) =>
            {
                await _viewModel.LoadAsync();
                _swipeRefreshLayout.Refreshing = false;
            };

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                if (e.PropertyName == nameof(_viewModel.SyncHistory))
                {
                    _adapter.UpdateData(_viewModel.SyncHistory);
                    _emptyTextView.Visibility = _viewModel.SyncHistory.Count == 0 ? ViewStates.Visible : ViewStates.Gone;
                }
                else if (e.PropertyName == nameof(_viewModel.IsBusy))
                {
                    if (_viewModel.IsBusy && _viewModel.SyncHistory.Count == 0)
                        _progressBar.Visibility = ViewStates.Visible;
                    else
                        _progressBar.Visibility = ViewStates.Gone;
                }
                else if (e.PropertyName == nameof(_viewModel.ErrorMessage))
                {
                    if (!string.IsNullOrEmpty(_viewModel.ErrorMessage))
                        Toast.MakeText(this, _viewModel.ErrorMessage, ToastLength.Long)?.Show();
                }
            });
        }

        protected override void OnDestroy()
        {
            if (_viewModel != null) _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            base.OnDestroy();
        }
    }
}
