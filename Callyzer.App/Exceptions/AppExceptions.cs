using System;

namespace Callyzer.App.Exceptions
{
    /// <summary>
    /// Represents errors that occur during database operations.
    /// </summary>
    public class DatabaseException : Exception
    {
        /// <summary>The database operation that failed.</summary>
        public string Operation { get; }

        /// <summary>The entity type involved in the failure.</summary>
        public string EntityType { get; }

        public DatabaseException(string message, string operation, string entityType, Exception? innerException = null)
            : base(message, innerException)
        {
            Operation = operation;
            EntityType = entityType;
        }
    }

    /// <summary>
    /// Represents errors that occur during API communication.
    /// </summary>
    public class ApiException : Exception
    {
        /// <summary>The HTTP status code returned by the API.</summary>
        public int StatusCode { get; }

        /// <summary>The API endpoint that was called.</summary>
        public string Endpoint { get; }

        public ApiException(string message, int statusCode, string endpoint, Exception? innerException = null)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            Endpoint = endpoint;
        }
    }

    /// <summary>
    /// Represents errors that occur during synchronization.
    /// </summary>
    public class SyncException : Exception
    {
        /// <summary>The sync phase that failed.</summary>
        public string Phase { get; }

        public SyncException(string message, string phase, Exception? innerException = null)
            : base(message, innerException)
        {
            Phase = phase;
        }
    }
}
