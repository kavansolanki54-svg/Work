using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DallyWorkReoprt.DAL.Models;

[Table("EmployeeMaster")]
public partial class EmployeeMaster
{
    [Key]
    [Column("EmployeeID")]
    public int EmployeeId { get; set; }

    [Column("CompanyID")]
    public int CompanyId { get; set; }

    [Column("RoleMasterID")]
    public int? RoleMasterId { get; set; }

    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [StringLength(100)]
    public string? MiddleName { get; set; }

    [StringLength(100)]
    public string? LastName { get; set; }

    [StringLength(100)]
    public string? Designation { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [StringLength(300)]
    [Unicode(false)]
    public string Passwords { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? MobileNo { get; set; }

    [StringLength(200)]
    public string? EmployeePhotoFile { get; set; }

    public byte IsAllowLogin { get; set; }

    public int? EmployeeCode { get; set; }

    public DateOnly? BirthDate { get; set; }

    [Column("DOJ")]
    public DateOnly? Doj { get; set; }

    [Column("GenderID")]
    public int? GenderId { get; set; }

    public string? Address { get; set; }

    public byte SignInAttempt { get; set; }

    [StringLength(302)]
    public string? EmployeeName { get; set; }

    public byte ActiveStatus { get; set; }

    [Column("CreatedByID")]
    [StringLength(100)]
    public string CreatedById { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    [Column("ModifiedByID")]
    [StringLength(100)]
    public string? ModifiedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    public Guid Guids { get; set; }

    public bool Tenant { get; set; }

    [InverseProperty("Employee")]
    public virtual ICollection<EmailRecipient> EmailRecipients { get; set; } = new List<EmailRecipient>();

    [InverseProperty("Employee")]
    public virtual ICollection<EmailSetting> EmailSettings { get; set; } = new List<EmailSetting>();

    [ForeignKey("GenderId")]
    [InverseProperty("EmployeeMasters")]
    public virtual Lookup? Gender { get; set; }

    [ForeignKey("RoleMasterId")]
    [InverseProperty("EmployeeMasters")]
    public virtual RoleMaster? RoleMaster { get; set; }
}
