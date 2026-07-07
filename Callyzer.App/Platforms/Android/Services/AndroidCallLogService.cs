using Android;
using Android.Content;
using Android.Database;
using Android.Provider;
using Callyzer.App.Converters;
using Callyzer.App.Helpers;
using Callyzer.App.Interfaces;
using Callyzer.App.Models;

namespace Callyzer.App.Platforms.Android.Services
{
    /// <summary>
    /// Android implementation of ICallLogPlatformService.
    /// Reads the device call log via ContentResolver + CallLog.Calls provider.
    /// Migrated from the existing CallLogReaderService in DailyTaskSheet.Android.
    /// </summary>
    public class AndroidCallLogService : ICallLogPlatformService
    {
        private readonly ILoggerService _logger;
        private const string Tag = "AndroidCallLogService";

        // Column projection for ContentResolver query
        private static readonly string[] Projection = new[]
        {
            CallLog.Calls.InterfaceConsts.Id,           // _id
            CallLog.Calls.Number,                        // number
            "type",                                      // call type
            "duration",                                  // call duration
            CallLog.Calls.Date,                          // date (epoch ms)
            CallLog.Calls.CachedName,                    // cached_name
            CallLog.Calls.CountryIso,                    // countryiso
            CallLog.Calls.GeocodedLocation,              // geocoded_location
        };

        public bool IsCallLogAccessSupported => true;

        public bool HasRequiredPermissions
        {
            get
            {
                var context = Platform.AppContext;
                return context.CheckSelfPermission(Manifest.Permission.ReadCallLog) == global::Android.Content.PM.Permission.Granted;
            }
        }

        public AndroidCallLogService(ILoggerService logger)
        {
            _logger = logger;
        }

        public async Task<bool> RequestPermissionsAsync()
        {
            try
            {
                var status = await Permissions.RequestAsync<Permissions.Phone>();
                if (status == PermissionStatus.Granted)
                {
                    _logger.Info(Tag, "Call log permission granted");
                    return true;
                }

                _logger.Warning(Tag, $"Call log permission denied: {status}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(Tag, "Failed to request permissions", ex);
                return false;
            }
        }

        public Task<List<CallLogModel>> ReadNewCallLogsAsync(
            long lastProcessedId, CancellationToken ct = default)
        {
            return Task.Run(() =>
            {
                ct.ThrowIfCancellationRequested();

                var results = new List<CallLogModel>();
                var context = Platform.AppContext;
                var resolver = context.ContentResolver;

                if (resolver == null)
                {
                    _logger.Error(Tag, "ContentResolver is null");
                    return results;
                }

                var selection = $"{CallLog.Calls.InterfaceConsts.Id} > ?";
                var selectionArgs = new[] { lastProcessedId.ToString() };
                var sortOrder = $"{CallLog.Calls.InterfaceConsts.Id} ASC";

                using var cursor = resolver.Query(
                    CallLog.Calls.ContentUri!,
                    Projection,
                    selection,
                    selectionArgs,
                    sortOrder);

                if (cursor == null)
                {
                    _logger.Warning(Tag, "Cursor returned null");
                    return results;
                }

                _logger.Info(Tag, $"Reading {cursor.Count} new call logs since ID {lastProcessedId}");

                while (cursor.MoveToNext())
                {
                    ct.ThrowIfCancellationRequested();
                    var callLog = MapCursorToModel(cursor);
                    if (callLog != null)
                    {
                        callLog.SyncHash = HashHelper.GenerateCallLogHash(
                            callLog.PhoneNumber, callLog.CallDate, callLog.Duration,
                            callLog.CallType, callLog.RawCallLogId);
                        results.Add(callLog);
                    }
                }

                _logger.Info(Tag, $"Read {results.Count} call logs");
                return results;
            }, ct);
        }

        public Task<List<CallLogModel>> ReadCallLogsByDateAsync(
            long fromTimestamp, long toTimestamp, CancellationToken ct = default)
        {
            return Task.Run(() =>
            {
                ct.ThrowIfCancellationRequested();
                var results = new List<CallLogModel>();
                var resolver = Platform.AppContext.ContentResolver;

                if (resolver == null) return results;

                var selection = $"{CallLog.Calls.Date} >= ? AND {CallLog.Calls.Date} < ?";
                var selectionArgs = new[] { fromTimestamp.ToString(), toTimestamp.ToString() };

                using var cursor = resolver.Query(
                    CallLog.Calls.ContentUri!,
                    Projection,
                    selection,
                    selectionArgs,
                    $"{CallLog.Calls.Date} ASC");

                if (cursor == null) return results;

                while (cursor.MoveToNext())
                {
                    ct.ThrowIfCancellationRequested();
                    var callLog = MapCursorToModel(cursor);
                    if (callLog != null) results.Add(callLog);
                }

                return results;
            }, ct);
        }

        public Task<int> GetTotalDeviceCallLogsAsync(CancellationToken ct = default)
        {
            return Task.Run(() =>
            {
                var resolver = Platform.AppContext.ContentResolver;
                if (resolver == null) return 0;

                using var cursor = resolver.Query(
                    CallLog.Calls.ContentUri!,
                    new[] { CallLog.Calls.InterfaceConsts.Id },
                    null, null, null);

                return cursor?.Count ?? 0;
            }, ct);
        }

        private CallLogModel? MapCursorToModel(ICursor cursor)
        {
            try
            {
                var rawId = cursor.GetLong(cursor.GetColumnIndexOrThrow(CallLog.Calls.InterfaceConsts.Id));
                var number = cursor.GetString(cursor.GetColumnIndexOrThrow(CallLog.Calls.Number)) ?? string.Empty;
                var typeInt = cursor.GetInt(cursor.GetColumnIndexOrThrow("type"));
                var duration = cursor.GetInt(cursor.GetColumnIndexOrThrow("duration"));
                var dateMs = cursor.GetLong(cursor.GetColumnIndexOrThrow(CallLog.Calls.Date));

                var cachedNameIdx = cursor.GetColumnIndex(CallLog.Calls.CachedName);
                var cachedName = cachedNameIdx >= 0 ? cursor.GetString(cachedNameIdx) ?? string.Empty : string.Empty;

                var countryIsoIdx = cursor.GetColumnIndex(CallLog.Calls.CountryIso);
                var countryIso = countryIsoIdx >= 0 ? cursor.GetString(countryIsoIdx) ?? string.Empty : string.Empty;

                var geoIdx = cursor.GetColumnIndex(CallLog.Calls.GeocodedLocation);
                var geo = geoIdx >= 0 ? cursor.GetString(geoIdx) ?? string.Empty : string.Empty;

                var callDate = DateTimeHelper.FromUnixMilliseconds(dateMs);
                var callType = CallTypeConverter.FromAndroidType(typeInt);

                return new CallLogModel
                {
                    RawCallLogId = rawId,
                    PhoneNumber = PhoneNumberHelper.Normalize(number),
                    ContactName = cachedName,
                    CachedName = cachedName,
                    CallType = (int)callType,
                    Duration = duration,
                    CallDate = callDate,
                    StartTime = callDate,
                    EndTime = DateTimeHelper.CalculateEndTime(callDate, duration),
                    CountryIso = countryIso,
                    GeocodedLocation = geo,
                    CreatedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.Error(Tag, $"Failed to map cursor row: {ex.Message}");
                return null;
            }
        }
    }
}
