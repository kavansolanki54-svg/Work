using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DallyWorkReoprt.DAL.Models
{
    [Table("DeviceInformation")]
    public class DeviceInformation
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string DeviceId { get; set; } = null!;
        
        public int EmployeeId { get; set; }
        
        [MaxLength(100)]
        public string? Manufacturer { get; set; }
        
        [MaxLength(100)]
        public string? Model { get; set; }
        
        [MaxLength(50)]
        public string? OsVersion { get; set; }
        
        [MaxLength(50)]
        public string? AppVersion { get; set; }
        
        [MaxLength(50)]
        public string? Platform { get; set; }
        
        public int BatteryPercentage { get; set; }
        
        [MaxLength(100)]
        public string? TimeZone { get; set; }
        
        public DateTime LastSyncTime { get; set; }
    }
}
