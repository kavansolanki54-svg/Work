using System;
using System.IO;
using System.Threading.Tasks;
using SQLite;
using DailyTaskSheet.App.Constants;
using DailyTaskSheet.App.Interfaces;
using DailyTaskSheet.App.Models;

namespace DailyTaskSheet.App.SQLite
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

        /// <summary>
        /// Initializes a new instance of <see cref="DatabaseService"/>.
        /// </summary>
        /// <param name="logger">Logger service for diagnostics.</param>
        public DatabaseService(ILoggerService logger)
        {
            _logger = logger;
            _databasePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                AppConstants.DatabaseName);
        }

        /// <summary>
        /// Gets the SQLite async connection, creating and initializing it if necessary.
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
                            _database = new SQLiteAsyncConnection(_databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
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

                // Create all tables
                await Database.CreateTableAsync<UserModel>();
                await Database.CreateTableAsync<CallLogModel>();
                await Database.CreateTableAsync<PendingSyncModel>();
                await Database.CreateTableAsync<SyncHistoryModel>();
                await Database.CreateTableAsync<ApiLogModel>();
                await Database.CreateTableAsync<ErrorLogModel>();
                await Database.CreateTableAsync<FailedRequestModel>();
                await Database.CreateTableAsync<SettingsModel>();
                await Database.CreateTableAsync<DeviceInformation>();

                // Create additional indexes for performance
                await CreateIndexesAsync();

                // Run migrations if needed
                var migrationManager = new DatabaseMigrationManager(Database, _logger);
                await migrationManager.RunMigrationsAsync();

                _logger.Info("DatabaseService", "Database initialization complete.");
            }
            catch (Exception ex)
            {
                _logger.Error("DatabaseService", "Failed to initialize database", ex);
                throw new Exceptions.DatabaseException(
                    "Failed to initialize the local database.",
                    "Initialize",
                    "All",
                    ex);
            }
        }

        /// <summary>
        /// Creates additional indexes that are not defined via attributes.
        /// </summary>
        private async Task CreateIndexesAsync()
        {
            try
            {
                // Composite index on CallLogs for sync queries
                await Database.ExecuteAsync(
                    "CREATE INDEX IF NOT EXISTS idx_calllogs_syncstatus_calldate ON CallLogs (SyncStatus, CallDate)");

                // Index on CallLogs for duplicate detection
                await Database.ExecuteAsync(
                    "CREATE INDEX IF NOT EXISTS idx_calllogs_rawid_hash ON CallLogs (RawCallLogId, SyncHash)");

                // Index on PendingSync for retry queries
                await Database.ExecuteAsync(
                    "CREATE INDEX IF NOT EXISTS idx_pendingsync_status_nextretry ON PendingSync (Status, NextAttempt)");

                // Index on SyncHistory for recent queries
                await Database.ExecuteAsync(
                    "CREATE INDEX IF NOT EXISTS idx_synchistory_synctime ON SyncHistory (SyncTime DESC)");

                // Index on ApiLogs for recent queries
                await Database.ExecuteAsync(
                    "CREATE INDEX IF NOT EXISTS idx_apilogs_createdat ON ApiLogs (CreatedAt DESC)");

                // Index on ErrorLogs for recent queries
                await Database.ExecuteAsync(
                    "CREATE INDEX IF NOT EXISTS idx_errorlogs_createdat ON ErrorLogs (CreatedAt DESC)");

                _logger.Debug("DatabaseService", "Additional indexes created successfully.");
            }
            catch (Exception ex)
            {
                _logger.Warning("DatabaseService", $"Index creation warning (non-fatal): {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the database file path.
        /// </summary>
        public string DatabasePath => _databasePath;

        /// <summary>
        /// Gets the database file size in bytes.
        /// </summary>
        public long GetDatabaseSize()
        {
            var fileInfo = new FileInfo(_databasePath);
            return fileInfo.Exists ? fileInfo.Length : 0;
        }

        /// <summary>
        /// Closes the database connection.
        /// </summary>
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
