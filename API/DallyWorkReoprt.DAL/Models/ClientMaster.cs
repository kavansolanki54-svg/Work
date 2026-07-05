using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DallyWorkReoprt.DAL.Models;

[Table("ClientMaster")]
[Index("CompanyId", Name = "IX_ClientMaster_CompanyID")]
public partial class ClientMaster
{
    [Key]
    [Column("ClientID")]
    public int ClientId { get; set; }

    [StringLength(200)]
    public string ClientName { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? ClientShortCode { get; set; }

    [Column("CompanyID")]
    public int CompanyId { get; set; }

    public byte ActiveStatus { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    [Column("CreatedByID")]
    [StringLength(100)]
    [Unicode(false)]
    public string CreatedById { get; set; } = null!;

    [Column("ModifiedByID")]
    [StringLength(100)]
    [Unicode(false)]
    public string? ModifiedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    public Guid Guids { get; set; }

    [ForeignKey("CompanyId")]
    [InverseProperty("ClientMasters")]
    public virtual CompanyMaster Company { get; set; } = null!;
}
