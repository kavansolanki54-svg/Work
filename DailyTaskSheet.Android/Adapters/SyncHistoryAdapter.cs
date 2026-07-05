using System;
using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using DailyTaskSheet.App.Extensions;
using DailyTaskSheet.App.Models;

namespace DailyTaskSheet.App.Adapters
{
    /// <summary>
    /// RecyclerView adapter for displaying synchronization history records.
    /// Handles binding SyncHistoryModel data to the item_sync_history.xml layout.
    /// </summary>
    public class SyncHistoryAdapter : RecyclerView.Adapter
    {
        private List<SyncHistoryModel> _historyList;

        /// <summary>
        /// Initializes a new instance of <see cref="SyncHistoryAdapter"/>.
        /// </summary>
        public SyncHistoryAdapter(List<SyncHistoryModel> historyList)
        {
            _historyList = historyList ?? new List<SyncHistoryModel>();
        }

        /// <summary>
        /// Updates the data set and notifies the adapter to refresh.
        /// </summary>
        public void UpdateData(List<SyncHistoryModel> newHistoryList)
        {
            _historyList = newHistoryList;
            NotifyDataSetChanged();
        }

        /// <inheritdoc />
        public override int ItemCount => _historyList.Count;

        /// <inheritdoc />
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context)!
                .Inflate(Resource.Layout.item_sync_history, parent, false)!;
            
            return new SyncHistoryViewHolder(itemView);
        }

        /// <inheritdoc />
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is SyncHistoryViewHolder vh)
            {
                var history = _historyList[position];
                vh.Bind(history);
            }
        }

        /// <summary>
        /// ViewHolder for a single sync history item.
        /// </summary>
        public class SyncHistoryViewHolder : RecyclerView.ViewHolder
        {
            private readonly TextView _syncTimeText;
            private readonly TextView _syncStatusText;
            private readonly TextView _syncDetailsText;
            private readonly TextView _syncMessageText;

            public SyncHistoryViewHolder(View itemView) : base(itemView)
            {
                _syncTimeText = itemView.FindViewById<TextView>(Resource.Id.syncTimeText)!;
                _syncStatusText = itemView.FindViewById<TextView>(Resource.Id.syncStatusText)!;
                _syncDetailsText = itemView.FindViewById<TextView>(Resource.Id.syncDetailsText)!;
                _syncMessageText = itemView.FindViewById<TextView>(Resource.Id.syncMessageText)!;
            }

            public void Bind(SyncHistoryModel history)
            {
                _syncTimeText.Text = history.SyncTime.ToDisplayString();
                
                // Status 2 is Success (Synced), 3 is Failed
                bool isSuccess = history.Status == (int)Enums.SyncStatusEnum.Synced;
                _syncStatusText.Text = isSuccess ? "Success" : "Failed";
                _syncStatusText.SetTextColor(Android.Graphics.Color.ParseColor(isSuccess ? "#22C55E" : "#EF4444"));

                _syncDetailsText.Text = $"Uploaded: {history.Uploaded} | Duplicates: {history.Duplicates} | Failed: {history.Failed}";
                
                _syncMessageText.Text = history.Message;
                if (!string.IsNullOrEmpty(history.TriggerSource))
                {
                    _syncMessageText.Text = $"[{history.TriggerSource}] {history.Message}";
                }
            }
        }
    }
}
