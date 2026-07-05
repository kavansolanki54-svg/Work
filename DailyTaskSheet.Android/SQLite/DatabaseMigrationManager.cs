using System;
using System.Threading.Tasks;
using SQLite;
using DailyTaskSheet.App.Constants;
using DailyTaskSheet.App.Interfaces;

namespace DailyTaskSheet.App.SQLite
{
    /// <summary>
    /// Manages database schema migrations to support future upgrades.
    /// Tracks the current schema version and applies incremental migrations.
    /// </summary>
    public class DatabaseMigrationManager
    {
        private readonly SQLiteAsyncConnection _database;
        private readonly ILoggerService _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="DatabaseMigrationManager"/>.
        /// </summary>
        public DatabaseMigrationManager(SQLiteAsyncConnection database, ILoggerService logger)
        {
            _database = database;
            _logger = logger;
        }

        /// <summary>
        /// Runs all pending database migrations from the current version
        /// to the latest <see cref="AppConstants.DatabaseVersion"/>.
        /// </summary>
        public async Task RunMigrationsAsync()
        {
            try
            {
                int currentVersion = await GetCurrentVersionAsync();
                int targetVersion = AppConstants.DatabaseVersion;

                if (currentVersion >= targetVersion)
                {
                    _logger.Debug("DatabaseMigration", $"Database is up to date (v{currentVersion}).");
                    return;
                }

                _logger.Info("DatabaseMigration", $"Migrating database from v{currentVersion} to v{targetVersion}...");

                // Apply migrations sequentially
                for (int version = currentVersion + 1; version <= targetVersion; version++)
                {
                    await ApplyMigrationAsync(version);
                    await SetCurrentVersionAsync(version);
                    _logger.Info("DatabaseMigration", $"Applied migration to v{version}.");
                }

                _logger.Info("DatabaseMigration", "All migrations applied successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error("DatabaseMigration", "Migration failed", ex);
                throw;
            }
        }

        /// <summary>
        /// Applies a specific migration version.
        /// Add new migration cases here as the schema evolves.
        /// </summary>
        private async Task ApplyMigrationAsync(int version)
        {
            switch (version)
            {
                case 1:
                    // Initial schema — tables are already created by DatabaseService.InitializeAsync()
                    // This migration is a no-op for the initial version.
                    await Task.CompletedTask;
                    break;

                // Future migrations go here:
                // case 2:
                //     await _database.ExecuteAsync("ALTER TABLE CallLogs ADD COLUMN NewField TEXT DEFAULT ''");
                //     break;

                default:
                    _logger.Warning("DatabaseMigration", $"Unknown migration version: {version}");
                    break;
            }
        }

        /// <summary>
        /// Gets the current database schema version from the Settings table.
        /// Returns 0 if no version is stored (first run).
        /// </summary>
        private async Task<int> GetCurrentVersionAsync()
        {
            try
            {
                var result = await _database.ExecuteScalarAsync<string>(
                    "SELECT Value FROM Settings WHERE Key = ?", AppConstants.PrefKeyDatabaseVersion);

                return int.TryParse(result, out int version) ? version : 0;
            }
            catch
            {
                // Settings table might not exist yet on first run
                return 0;
            }
        }

        /// <summary>
        /// Stores the current database schema version in the Settings table.
        /// </summary>
        private async Task SetCurrentVersionAsync(int version)
        {
            await _database.ExecuteAsync(
                "INSERT OR REPLACE INTO Settings (Key, Value, Description, IsServerSetting, UpdatedAt) VALUES (?, ?, ?, 0, ?)",
                AppConstants.PrefKeyDatabaseVersion,
                version.ToString(),
                "Database schema version",
                DateTime.UtcNow);
        }
    }
}
