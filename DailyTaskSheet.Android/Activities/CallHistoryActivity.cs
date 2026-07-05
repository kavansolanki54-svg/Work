using System;
using System.ComponentModel;
using Android.App;
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
    /// Activity displaying the history of phone calls recorded on the device.
    /// Supports infinite scrolling and pull-to-refresh.
    /// </summary>
    [Activity(Label = "Call History", Theme = "@style/AppTheme", ParentActivity = typeof(DashboardActivity))]
    public class CallHistoryActivity : AppCompatActivity
    {
        private CallHistoryViewModel _viewModel = null!;
        private SwipeRefreshLayout _swipeRefreshLayout = null!;
        private RecyclerView _recyclerView = null!;
        private ProgressBar _progressBar = null!;
        private TextView _emptyTextView = null!;
        private CallLogAdapter _adapter = null!;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_call_history);

            var toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar?.SetDisplayHomeAsUpEnabled(true);

            var app = (App)Application;
            _viewModel = app.GetService<CallHistoryViewModel>()!;

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
            _recyclerView = FindViewById<RecyclerView>(Resource.Id.callLogRecyclerView)!;
            _progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar)!;
            _emptyTextView = FindViewById<TextView>(Resource.Id.emptyTextView)!;
        }

        private void SetupRecyclerView()
        {
            var layoutManager = new LinearLayoutManager(this);
            _recyclerView.SetLayoutManager(layoutManager);
            
            _adapter = new CallLogAdapter(_viewModel.CallLogs);
            _recyclerView.SetAdapter(_adapter);

            _recyclerView.AddOnScrollListener(new CallHistoryScrollListener(layoutManager, () =>
            {
                _ = _viewModel.LoadMoreAsync();
            }));
        }

        private void BindEvents()
        {
            _swipeRefreshLayout.Refresh += async (s, e) =>
            {
                await _viewModel.RefreshAsync();
                _swipeRefreshLayout.Refreshing = false;
            };

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                if (e.PropertyName == nameof(_viewModel.CallLogs))
                {
                    _adapter.UpdateData(_viewModel.CallLogs);
                    _emptyTextView.Visibility = _viewModel.CallLogs.Count == 0 ? ViewStates.Visible : ViewStates.Gone;
                }
                else if (e.PropertyName == nameof(_viewModel.IsBusy))
                {
                    if (_viewModel.IsBusy && _viewModel.CallLogs.Count == 0)
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

    /// <summary>
    /// Custom scroll listener for RecyclerView infinite scrolling.
    /// </summary>
    public class CallHistoryScrollListener : RecyclerView.OnScrollListener
    {
        private readonly LinearLayoutManager _layoutManager;
        private readonly Action _onLoadMore;
        private int _previousTotal = 0;
        private bool _loading = true;

        public CallHistoryScrollListener(LinearLayoutManager layoutManager, Action onLoadMore)
        {
            _layoutManager = layoutManager;
            _onLoadMore = onLoadMore;
        }

        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            base.OnScrolled(recyclerView, dx, dy);

            if (dy > 0) // Scrolling down
            {
                int visibleItemCount = recyclerView.ChildCount;
                int totalItemCount = _layoutManager.ItemCount;
                int firstVisibleItem = _layoutManager.FindFirstVisibleItemPosition();

                if (_loading && totalItemCount > _previousTotal)
                {
                    _loading = false;
                    _previousTotal = totalItemCount;
                }

                if (!_loading && (totalItemCount - visibleItemCount) <= (firstVisibleItem + 5))
                {
                    _loading = true;
                    _onLoadMore();
                }
            }
        }
    }
}
