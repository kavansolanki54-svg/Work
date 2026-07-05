using System;

namespace DailyTaskSheet.App.Exceptions
{
    /// <summary>
    /// Represents an exception thrown when a SQLite database operation fails.
    /// Wraps the underlying SQLite error with contextual information about the operation.
    /// </summary>
    public class DatabaseException : Exception
    {
        /// <summary>Gets the database operation that failed (e.g., Insert, Update, Query).</summary>
        public string? Operation { get; }

        /// <summary>Gets the name of the table involved in the failed operation.</summary>
        public string? TableName { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="DatabaseException"/>.
        /// </summary>
        /// <param name="message">Human-readable error message.</param>
        /// <param name="operation">The database operation that failed.</param>
        /// <param name="tableName">The table involved in the operation.</param>
        /// <param name="innerException">The underlying SQLite exception.</param>
        public DatabaseException(
            string message,
            string? operation = null,
            string? tableName = null,
            Exception? innerException = null)
            : base(message, innerException)
        {
            Operation = operation;
            TableName = tableName;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"DatabaseException: {Message} | Operation: {Operation ?? "N/A"} | Table: {TableName ?? "N/A"}";
        }
    }
}
