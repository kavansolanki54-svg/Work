using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Provider;
using DailyTaskSheet.App.Interfaces;

namespace DailyTaskSheet.App.Services
{
    public class NativeRecordingScannerService : INativeRecordingScannerService
    {
        private readonly Context _context;
        private readonly IPreferenceService _preferenceService;

        public NativeRecordingScannerService(Context context, IPreferenceService preferenceService)
        {
            _context = context;
            _preferenceService = preferenceService;
        }

        public async Task<string?> FindRecordingPathAsync(string phoneNumber, DateTime callStartTime)
        {
            return await Task.Run(() =>
            {
                try
                {
                    // Clean phone number (remove spaces, dashes)
                    string cleanNumber = new string(phoneNumber.Where(char.IsDigit).ToArray());
                    
                    // 1. Try Custom Folder First
                    string? customFound = TryFindInCustomDirectory(cleanNumber, callStartTime);
                    if (!string.IsNullOrEmpty(customFound)) return customFound;

                    // 2. Try MediaStore
                    var uri = MediaStore.Audio.Media.ExternalContentUri;
                    
                    string[] projection = {
                        MediaStore.Audio.Media.InterfaceConsts.Data,
                        MediaStore.Audio.Media.InterfaceConsts.DateAdded,
                        MediaStore.Audio.Media.InterfaceConsts.DisplayName
                    };

                    // We look for audio files. We can narrow down by DateAdded.
                    // DateAdded in MediaStore is in seconds since epoch.
                    long startTimeSeconds = ((DateTimeOffset)callStartTime).ToUnixTimeSeconds();
                    
                    // Look for files added around the call time (e.g., up to 2 hours after call started, and slightly before)
                    long minTime = startTimeSeconds - 300; // 5 mins before
                    long maxTime = startTimeSeconds + 7200; // 2 hours after

                    string selection = $"{MediaStore.Audio.Media.InterfaceConsts.DateAdded} >= ? AND {MediaStore.Audio.Media.InterfaceConsts.DateAdded} <= ?";
                    string[] selectionArgs = { minTime.ToString(), maxTime.ToString() };
                    string sortOrder = $"{MediaStore.Audio.Media.InterfaceConsts.DateAdded} DESC";

                    if (uri != null)
                    {
                        using var cursor = _context.ContentResolver?.Query(uri, projection, selection, selectionArgs, sortOrder);
                        
                        if (cursor != null && cursor.MoveToFirst())
                        {
                            int dataColumn = cursor.GetColumnIndexOrThrow(MediaStore.Audio.Media.InterfaceConsts.Data);
                            int nameColumn = cursor.GetColumnIndexOrThrow(MediaStore.Audio.Media.InterfaceConsts.DisplayName);

                            do
                            {
                                string path = cursor.GetString(dataColumn) ?? string.Empty;
                                string name = cursor.GetString(nameColumn) ?? string.Empty;

                                bool containsNumber = !string.IsNullOrEmpty(cleanNumber) && name.Replace(" ", "").Replace("-", "").Contains(cleanNumber);
                                bool isCallFolder = path.Contains("Call", StringComparison.OrdinalIgnoreCase) || 
                                                    path.Contains("Record", StringComparison.OrdinalIgnoreCase);

                                if (containsNumber || isCallFolder)
                                {
                                    if (File.Exists(path))
                                    {
                                        return path;
                                    }
                                }

                            } while (cursor.MoveToNext());
                        }
                    }

                    // 3. Fallback to Common Directories
                    return TryFindInCommonDirectories(cleanNumber, callStartTime);
                }
                catch (Exception ex)
                {
                    Android.Util.Log.Error("NativeScanner", $"Error scanning for recording: {ex.Message}");
                    return null;
                }
            });
        }

        private string? TryFindInCustomDirectory(string cleanNumber, DateTime callStartTime)
        {
            try
            {
                string customUriString = _preferenceService.GetString(DailyTaskSheet.App.Constants.AppConstants.PrefKeyCustomRecordingPath, string.Empty);
                if (string.IsNullOrEmpty(customUriString)) return null;

                var treeUri = Android.Net.Uri.Parse(customUriString);
                if (treeUri == null) return null;

                var documentTree = AndroidX.DocumentFile.Provider.DocumentFile.FromTreeUri(_context, treeUri);
                if (documentTree == null || !documentTree.CanRead()) return null;

                // Search files in the selected directory
                foreach (var document in documentTree.ListFiles())
                {
                    if (document.IsDirectory) continue;

                    string name = document.Name ?? string.Empty;
                    // Check if it's an audio file and contains the number
                    if (name.Replace(" ", "").Replace("-", "").Contains(cleanNumber))
                    {
                        // Check last modified time
                        long lastModifiedMillis = document.LastModified();
                        var lastModifiedDate = DateTimeOffset.FromUnixTimeMilliseconds(lastModifiedMillis).UtcDateTime;

                        if (Math.Abs((lastModifiedDate - callStartTime.ToUniversalTime()).TotalHours) < 12)
                        {
                            // Return the content URI string to upload
                            return document.Uri?.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Android.Util.Log.Error("NativeScanner", $"Error checking custom dir: {ex.Message}");
            }
            return null;
        }

        private string? TryFindInCommonDirectories(string cleanNumber, DateTime callStartTime)
        {
            try
            {
                var externalStorage = Android.OS.Environment.ExternalStorageDirectory?.AbsolutePath;
                if (externalStorage == null) return null;

                string[] commonPaths = {
                    Path.Combine(externalStorage, "Call", "Recordings"),
                    Path.Combine(externalStorage, "Recordings", "Call"),
                    Path.Combine(externalStorage, "MIUI", "sound_recorder", "call_rec"),
                    Path.Combine(externalStorage, "Music", "Call Recordings")
                };

                foreach (var path in commonPaths)
                {
                    if (Directory.Exists(path))
                    {
                        var files = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly);
                        foreach (var file in files)
                        {
                            var fileName = Path.GetFileName(file).Replace(" ", "").Replace("-", "");
                            var fileInfo = new FileInfo(file);
                            
                            if (fileName.Contains(cleanNumber) && Math.Abs((fileInfo.CreationTimeUtc - callStartTime.ToUniversalTime()).TotalHours) < 12)
                            {
                                return file;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Android.Util.Log.Error("NativeScanner", $"Error checking common dirs: {ex.Message}");
            }

            return null;
        }
    }
}
