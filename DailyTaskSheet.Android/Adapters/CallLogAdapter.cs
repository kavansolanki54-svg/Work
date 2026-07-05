using System;
using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using DailyTaskSheet.App.Converters;
using DailyTaskSheet.App.Extensions;
using DailyTaskSheet.App.Models;

namespace DailyTaskSheet.App.Adapters
{
    /// <summary>
    /// RecyclerView adapter for displaying call log records.
    /// Handles binding CallLogModel data to the item_call_log.xml layout.
    /// </summary>
    public class CallLogAdapter : RecyclerView.Adapter
    {
        private List<CallLogModel> _callLogs;

        /// <summary>
        /// Initializes a new instance of <see cref="CallLogAdapter"/>.
        /// </summary>
        public CallLogAdapter(List<CallLogModel> callLogs)
        {
            _callLogs = callLogs ?? new List<CallLogModel>();
        }

        /// <summary>
        /// Updates the data set and notifies the adapter to refresh.
        /// </summary>
        public void UpdateData(List<CallLogModel> newCallLogs)
        {
            _callLogs = newCallLogs;
            NotifyDataSetChanged();
        }

        /// <inheritdoc />
        public override int ItemCount => _callLogs.Count;

        /// <inheritdoc />
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context)!
                .Inflate(Resource.Layout.item_call_log, parent, false)!;
            
            return new CallLogViewHolder(itemView);
        }

        /// <inheritdoc />
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is CallLogViewHolder vh)
            {
                var callLog = _callLogs[position];
                vh.Bind(callLog);
            }
        }

        /// <summary>
        /// ViewHolder for a single call log item.
        /// </summary>
        public class CallLogViewHolder : RecyclerView.ViewHolder
        {
            private readonly ImageView _callTypeIcon;
            private readonly TextView _phoneNumberText;
            private readonly TextView _contactNameText;
            private readonly TextView _durationText;
            private readonly TextView _dateText;
            private readonly TextView _syncStatusBadge;

            public CallLogViewHolder(View itemView) : base(itemView)
            {
                _callTypeIcon = itemView.FindViewById<ImageView>(Resource.Id.callTypeIcon)!;
                _phoneNumberText = itemView.FindViewById<TextView>(Resource.Id.phoneNumberText)!;
                _contactNameText = itemView.FindViewById<TextView>(Resource.Id.contactNameText)!;
                _durationText = itemView.FindViewById<TextView>(Resource.Id.durationText)!;
                _dateText = itemView.FindViewById<TextView>(Resource.Id.dateText)!;
                _syncStatusBadge = itemView.FindViewById<TextView>(Resource.Id.syncStatusBadge)!;
            }

            public void Bind(CallLogModel callLog)
            {
                _phoneNumberText.Text = string.IsNullOrEmpty(callLog.PhoneNumber) ? "Unknown Number" : callLog.PhoneNumber;
                
                if (!string.IsNullOrEmpty(callLog.ContactName))
                {
                    _contactNameText.Text = callLog.ContactName;
                    _contactNameText.Visibility = ViewStates.Visible;
                }
                else
                {
                    _contactNameText.Visibility = ViewStates.Gone;
                }

                _durationText.Text = DateTimeExtensions.FormatDuration(callLog.Duration);
                _dateText.Text = callLog.CallDate.ToDisplayString();

                // Set call type icon based on enum
                SetCallTypeIcon(callLog.CallTypeEnum);

                // Set sync status badge
                SetSyncStatusBadge((Enums.SyncStatusEnum)callLog.SyncStatus);
            }

            private void SetCallTypeIcon(Enums.CallTypeEnum callType)
            {
                int iconResId = callType switch
                {
                    Enums.CallTypeEnum.Incoming => Android.Resource.Drawable.SymCallIncoming,
                    Enums.CallTypeEnum.Outgoing => Android.Resource.Drawable.SymCallOutgoing,
                    Enums.CallTypeEnum.Missed => Android.Resource.Drawable.SymCallMissed,
                    _ => Android.Resource.Drawable.SymCallIncoming
                };
                
                _callTypeIcon.SetImageResource(iconResId);

                // Tint red if missed
                if (callType == Enums.CallTypeEnum.Missed || callType == Enums.CallTypeEnum.Rejected)
                {
                    _callTypeIcon.SetColorFilter(Android.Graphics.Color.ParseColor("#EF4444")); // Red
                }
                else
                {
                    _callTypeIcon.SetColorFilter(Android.Graphics.Color.ParseColor("#4F46E5")); // Primary
                }
            }

            private void SetSyncStatusBadge(Enums.SyncStatusEnum status)
            {
                _syncStatusBadge.Text = status.ToString();
                
                string colorHex = status switch
                {
                    Enums.SyncStatusEnum.Synced => "#22C55E", // Green
                    Enums.SyncStatusEnum.Pending => "#F59E0B", // Amber
                    Enums.SyncStatusEnum.Syncing => "#3B82F6", // Blue
                    Enums.SyncStatusEnum.Failed => "#EF4444", // Red
                    _ => "#6B7280" // Gray
                };

                var bg = _syncStatusBadge.Background?.Mutate() as Android.Graphics.Drawables.GradientDrawable;
                bg?.SetColor(Android.Graphics.Color.ParseColor(colorHex));
            }
        }
    }
}
