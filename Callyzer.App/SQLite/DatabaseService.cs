using System;
using System.IO;
using System.Threading.Tasks;
using SQLite;
using Callyzer.App.Constants;
using Callyzer.App.Interfaces;
using Callyzer.App.Models;

namespace Callyzer.App.SQLite
{
    /// <summary>
    /// Manages the SQLite database connection, table creation, and schema initialization.
    /// Implements a lazy singleton connection pattern for thread safety.
    /// </summary>
    public class DatabaseService
    {
        private static SQLiteAsyncConnection? _database;
        private static readonly object _lock = new object();
        private readonly ILoggerService _logger;
        private readonly string _databasePath;

        public DatabaseService(ILoggerService logger, string? dbPath = null)
        {
            _logger = logger;
            _databasePath = dbPath ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                AppConstants.DatabaseName);
        }

        /// <summary>
        /// Gets the SQLite async connection, creating it if necessary.
        /// </summary>
        public SQLiteAsyncConnection Database
        {
            get
            {
                if (_database == null)
                {
                    lock (_lock)
                    {
                        if (_database == null)
                        {
                            _database = new SQLiteAsyncConnection(_databasePath,
                                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
                            _logger.Info("DatabaseService", $"Database created at: {_databasePath}");
                        }
                    }
                }
                return _database;
            }
        }

        /// <summary>
        /// Initializes all database tables and indexes.
        /// Must be called once during application startup.
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                _logger.Info("DatabaseService", "Initializing database tables...");

                await Database.CreateTableAsync<CallLogModel>();
                await Database.CreateTableAsync<AnalyticsCacheModel>();

                // Create performance indexes
                await CreateIndexesAsync();

                _logger.Info("DatabaseService", "Database initialization complete.");
            }
            catch (Exception ex)
            {
                _logger.Error("DatabaseService", "Failed to initialize database", ex);
                throw new Exceptions.DatabaseException(
                    "Failed to initialize the local database.",
                    "Initialize", "All", ex);
            }
        }

        private async Task CreateIndexesAsync()
        {
            try
            {
                // Composite indexes for analytics queries (50K+ performance)
                await Database.ExecuteAsync(
                    "CREATE INDEX IF NOT EXISTS idx_cl_date_type ON CallLogs (CallDate, CallType)");
                await Database.ExecuteAsync(
                    "CREATE INDEX IF NOT EXISTS idx_cl_phone_date ON CallLogs (PhoneNumber, CallDate)");
                await Database.ExecuteAsync(
                    "CREATE INDEX IF NOT EXISTS idx_cl_duration_desc ON CallLogs (Duration DESC)");
                await Database.ExecuteAsync(
                    "CREATE INDEX IF NOT EXISTS idx_cl_sim_date ON CallLogs (SimSlot, CallDate)");
                await Database.ExecuteAsync(
                    "CREATE INDEX IF NOT EXISTS idx_cl_syncstatus_calldate ON CallLogs (SyncStatus, CallDate)");
                await Database.ExecuteAsync(
                    "CREATE INDEX IF NOT EXISTS idx_cl_rawid_hash ON CallLogs (RawCallLogId, SyncHash)");

                // Analytics cache index
                await Database.ExecuteAsync(
                    "CREATE UNIQUE INDEX IF NOT EXISTS idx_ac_period ON AnalyticsCache (PeriodType, PeriodStart)");

                _logger.Debug("DatabaseService", "Database indexes created successfully.");
            }
            catch (Exception ex)
            {
                _logger.Warning("DatabaseService", $"Index creation warning (non-fatal): {ex.Message}");
            }
        }

        /// <summary>Gets the database file path.</summary>
        public string DatabasePath => _databasePath;

        /// <summary>Gets the database file size in bytes.</summary>
        public long GetDatabaseSize()
        {
            var fileInfo = new FileInfo(_databasePath);
            return fileInfo.Exists ? fileInfo.Length : 0;
        }

        /// <summary>Closes the database connection.</summary>
        public async Task CloseAsync()
        {
            if (_database != null)
            {
                await _database.CloseAsync();
                _database = null;
                _logger.Info("DatabaseService", "Database connection closed.");
            }
        }
    }
}
