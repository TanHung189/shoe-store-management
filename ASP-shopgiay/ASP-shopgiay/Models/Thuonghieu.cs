using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ASP_shopgiay.Models;

[Table("THUONGHIEU")]
[Index("TenTh", Name = "UQ__THUONGHI__4CF9E74A90ABAB41", IsUnique = true)]
public partial class Thuonghieu
{
    [Key]
    [Column("MaTH")]
    public int MaTh { get; set; }

    [Column("TenTH")]
    [StringLength(100)]
    public string TenTh { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string? Logo { get; set; }

    [InverseProperty("MaThNavigation")]
    public virtual ICollection<Sanpham> Sanphams { get; set; } = new List<Sanpham>();
}
