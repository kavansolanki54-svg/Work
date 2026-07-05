using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DallyWorkReoprt.DAL.Models;

[Table("RefreshToken")]
public partial class RefreshToken
{
    [Key]
    public int Id { get; set; }

    [StringLength(1000)]
    [Unicode(false)]
    public string Token { get; set; } = null!;

    [Unicode(false)]
    public string JwtId { get; set; } = null!;

    public int UserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ExpiryDate { get; set; }

    public bool IsUsed { get; set; }

    public bool IsRevoked { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }
}
