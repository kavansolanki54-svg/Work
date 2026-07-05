using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Provider;
using DailyTaskSheet.App.Converters;
using DailyTaskSheet.App.Helpers;
using DailyTaskSheet.App.Interfaces;
using DailyTaskSheet.App.Models;

namespace DailyTaskSheet.App.Services
{
    /// <summary>
    /// Reads call log records from the Android system via ContentResolver.
    /// Queries CallLog.Calls.ContentUri and maps all available columns to CallLogModel.
    /// </summary>
    public class CallLogReaderService : ICallLogReaderService
    {
        private readonly Context _context;
        private readonly IDeviceService _deviceService;
        private readonly ILoggerService _logger;

        /// <summary>All columns to project from the CallLog.Calls content provider.</summary>
        private static readonly string[] Projection = new string[]
        {
            Android.Provider.BaseColumns.Id,           // _ID
            CallLog.Calls.Number,                         // NUMBER
            CallLog.Calls.Type,           // TYPE
            CallLog.Calls.Duration,       // DURATION
            CallLog.Calls.Date,           // DATE (epoch ms)
            CallLog.Calls.CachedName,                     // CACHED_NAME
            CallLog.Calls.CachedNumberLabel,              // CACHED_NUMBER_LABEL
            CallLog.Calls.CachedNumberType,               // CACHED_NUMBER_TYPE
            CallLog.Calls.CountryIso,                     // COUNTRY_ISO
            CallLog.Calls.GeocodedLocation,               // GEOCODED_LOCATION
            CallLog.Calls.NumberPresentation, // NUMBER_PRESENTATION
            CallLog.Calls.PhoneAccountComponentName,      // PHONE_ACCOUNT_COMPONENT_NAME
            CallLog.Calls.PhoneAccountId,                 // PHONE_ACCOUNT_ID
            CallLog.Calls.IsRead,         // IS_READ
            CallLog.Calls.New,            // NEW
        };

        /// <summary>
        /// Initializes a new instance of <see cref="CallLogReaderService"/>.
        /// </summary>
        public CallLogReaderService(Context context, IDeviceService deviceService, ILoggerService logger)
        {
            _context = context;
            _deviceService = deviceService;
            _logger = logger;
        }

        /// <inheritdoc />
        public Task<List<CallLogModel>> ReadNewCallLogsAsync(long lastProcessedId, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                var callLogs = new List<CallLogModel>();
                ICursor? cursor = null;

                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    long startOfTodayMs = new DateTimeOffset(DateTime.Today).ToUnixTimeMilliseconds();

                    string selection = lastProcessedId > 0
                        ? $"{Android.Provider.BaseColumns.Id} > ? AND {CallLog.Calls.Date} >= ?"
                        : $"{CallLog.Calls.Date} >= ?";
                    
                    string[] selectionArgs = lastProcessedId > 0
                        ? new[] { lastProcessedId.ToString(), startOfTodayMs.ToString() }
                        : new[] { startOfTodayMs.ToString() };
                    string sortOrder = $"{Android.Provider.BaseColumns.Id} ASC";

                    cursor = _context.ContentResolver?.Query(
                        CallLog.Calls.ContentUri!,
                        Projection,
                        selection,
                        selectionArgs,
                        sortOrder);

                    if (cursor == null)
                    {
                        _logger.Warning("CallLogReader", "ContentResolver returned null cursor.");
                        return callLogs;
                    }

                    string deviceId = _deviceService.GetDeviceId();
                    string deviceName = _deviceService.GetDeviceName();
                    int batteryPct = _deviceService.GetBatteryPercentage();
                    string timeZone = _deviceService.GetTimeZone();

                    while (cursor.MoveToNext())
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var callLog = MapCursorToCallLog(cursor, deviceId, deviceName, batteryPct, timeZone);
                        if (callLog != null)
                        {
                            callLogs.Add(callLog);
                        }
                    }

                    _logger.Info("CallLogReader", $"Read {callLogs.Count} new call logs (after ID {lastProcessedId}).");
                }
                catch (global::System.OperationCanceledException) { throw; }
                catch (Exception ex)
                {
                    _logger.Error("CallLogReader", "Failed to read call logs", ex);
                }
                finally
                {
                    cursor?.Close();
                }

                return callLogs;
            }, cancellationToken);
        }

        /// <inheritdoc />
        public Task<List<CallLogModel>> ReadCallLogsByDateAsync(long fromTimestamp, long toTimestamp, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                var callLogs = new List<CallLogModel>();
                ICursor? cursor = null;

                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    string selection = $"{CallLog.Calls.Date} >= ? AND {CallLog.Calls.Date} <= ?";
                    string[] selectionArgs = new[] { fromTimestamp.ToString(), toTimestamp.ToString() };
                    string sortOrder = $"{CallLog.Calls.Date} DESC";

                    cursor = _context.ContentResolver?.Query(
                        CallLog.Calls.ContentUri!,
                        Projection,
                        selection,
                        selectionArgs,
                        sortOrder);

                    if (cursor == null) return callLogs;

                    string deviceId = _deviceService.GetDeviceId();
                    string deviceName = _deviceService.GetDeviceName();
                    int batteryPct = _deviceService.GetBatteryPercentage();
                    string timeZone = _deviceService.GetTimeZone();

                    while (cursor.MoveToNext())
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var callLog = MapCursorToCallLog(cursor, deviceId, deviceName, batteryPct, timeZone);
                        if (callLog != null) callLogs.Add(callLog);
                    }
                }
                catch (global::System.OperationCanceledException) { throw; }
                catch (Exception ex)
                {
                    _logger.Error("CallLogReader", "Failed to read call logs by date", ex);
                }
                finally
                {
                    cursor?.Close();
                }

                return callLogs;
            }, cancellationToken);
        }

        /// <inheritdoc />
        public Task<int> GetTotalDeviceCallLogsAsync(CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                ICursor? cursor = null;
                try
                {
                    cursor = _context.ContentResolver?.Query(
                        CallLog.Calls.ContentUri!,
                        new[] { Android.Provider.BaseColumns.Id },
                        null, null, null);

                    return cursor?.Count ?? 0;
                }
                catch (Exception ex)
                {
                    _logger.Warning("CallLogReader", $"Failed to count device call logs: {ex.Message}");
                    return 0;
                }
                finally
                {
                    cursor?.Close();
                }
            }, cancellationToken);
        }

        /// <summary>
        /// Maps a single cursor row to a CallLogModel, reading all available columns safely.
        /// </summary>
        private CallLogModel? MapCursorToCallLog(ICursor cursor, string deviceId, string deviceName, int batteryPct, string timeZone)
        {
            try
            {
                long rawId = GetLong(cursor, Android.Provider.BaseColumns.Id);
                if (rawId <= 0) return null;

                string phoneNumber = GetString(cursor, CallLog.Calls.Number);
                long dateMs = GetLong(cursor, CallLog.Calls.Date);
                int duration = GetInt(cursor, CallLog.Calls.Duration);
                int callTypeInt = GetInt(cursor, CallLog.Calls.Type);

                DateTime callDate = DateTimeHelper.FromUnixMilliseconds(dateMs);
                DateTime endTime = DateTimeHelper.CalculateEndTime(callDate, duration);

                var callLog = new CallLogModel
                {
                    RawCallLogId = rawId,
                    PhoneNumber = phoneNumber,
                    ContactName = GetString(cursor, CallLog.Calls.CachedName),
                    CallType = callTypeInt,
                    Duration = duration,
                    CallDate = callDate,
                    StartTime = callDate,
                    EndTime = endTime,
                    CachedName = GetString(cursor, CallLog.Calls.CachedName),
                    CachedNumberLabel = GetString(cursor, CallLog.Calls.CachedNumberLabel),
                    CachedNumberType = GetInt(cursor, CallLog.Calls.CachedNumberType),
                    CountryIso = GetString(cursor, CallLog.Calls.CountryIso),
                    GeocodedLocation = GetString(cursor, CallLog.Calls.GeocodedLocation),
                    Presentation = GetInt(cursor, CallLog.Calls.NumberPresentation),
                    PhoneAccountComponent = GetString(cursor, CallLog.Calls.PhoneAccountComponentName),
                    PhoneAccountId = GetString(cursor, CallLog.Calls.PhoneAccountId),
                    IsRead = GetInt(cursor, CallLog.Calls.IsRead) == 1,
                    IsNew = GetInt(cursor, CallLog.Calls.New) == 1,
                    DeviceId = deviceId,
                    Manufacturer = Build.Manufacturer ?? "Unknown",
                    DeviceModel = Build.Model ?? "Unknown",
                    AndroidVersion = Build.VERSION.Release ?? "Unknown",
                    BatteryPercentage = batteryPct,
                    TimeZone = timeZone,
                    SyncStatus = (int)Enums.SyncStatusEnum.Pending,
                    SyncHash = HashHelper.GenerateCallLogHash(phoneNumber, callDate, duration, callTypeInt, rawId),
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                };

                // Try to determine SIM slot from PhoneAccountId
                callLog.SimSlot = ParseSimSlot(callLog.PhoneAccountId);

                return callLog;
            }
            catch (Exception ex)
            {
                _logger.Warning("CallLogReader", $"Error mapping call log row: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Safely gets a string value from a cursor column.
        /// </summary>
        private static string GetString(ICursor cursor, string columnName)
        {
            try
            {
                int index = cursor.GetColumnIndex(columnName);
                return index >= 0 && !cursor.IsNull(index) ? cursor.GetString(index) ?? string.Empty : string.Empty;
            }
            catch { return string.Empty; }
        }

        /// <summary>
        /// Safely gets an integer value from a cursor column.
        /// </summary>
        private static int GetInt(ICursor cursor, string columnName)
        {
            try
            {
                int index = cursor.GetColumnIndex(columnName);
                return index >= 0 && !cursor.IsNull(index) ? cursor.GetInt(index) : 0;
            }
            catch { return 0; }
        }

        /// <summary>
        /// Safely gets a long value from a cursor column.
        /// </summary>
        private static long GetLong(ICursor cursor, string columnName)
        {
            try
            {
                int index = cursor.GetColumnIndex(columnName);
                return index >= 0 && !cursor.IsNull(index) ? cursor.GetLong(index) : 0L;
            }
            catch { return 0L; }
        }

        /// <summary>
        /// Attempts to parse SIM slot index from PhoneAccountId.
        /// </summary>
        private static int ParseSimSlot(string phoneAccountId)
        {
            if (string.IsNullOrEmpty(phoneAccountId)) return -1;

            // Common patterns: "0", "1", or ICCID-based IDs
            if (int.TryParse(phoneAccountId, out int slot) && slot >= 0 && slot <= 3)
            {
                return slot;
            }
            return -1;
        }
    }
}
