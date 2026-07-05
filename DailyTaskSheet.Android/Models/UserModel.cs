using System;
using SQLite;

namespace DailyTaskSheet.App.Models
{
    /// <summary>
    /// Represents the locally stored authenticated user information.
    /// Persisted in SQLite for offline access.
    /// </summary>
    [Table("Users")]
    public class UserModel
    {
        /// <summary>Local SQLite auto-increment primary key.</summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>Backend employee ID.</summary>
        [Indexed(Unique = true)]
        public int EmployeeId { get; set; }

        /// <summary>User's email address.</summary>
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        /// <summary>User/employee display name.</summary>
        [MaxLength(200)]
        public string UserName { get; set; } = string.Empty;

        /// <summary>Company ID the user belongs to.</summary>
        [Column("CompanyId")]
        public int CompanyId { get; set; }

        [Column("CompanyName")]
        public string CompanyName { get; set; } = string.Empty;

        /// <summary>Role ID assigned to the user.</summary>
        public int RoleId { get; set; }

        /// <summary>Role name assigned to the user.</summary>
        [MaxLength(100)]
        public string RoleName { get; set; } = string.Empty;

        /// <summary>Role type (e.g., "Admin", "Employee").</summary>
        [MaxLength(100)]
        public string RoleType { get; set; } = string.Empty;

        /// <summary>Whether this user is a tenant administrator.</summary>
        public bool IsTenant { get; set; }

        /// <summary>Timestamp when the user record was first stored.</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>Timestamp when the user record was last updated.</summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
