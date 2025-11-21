using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ASP_shopgiay.Models;

[Table("PHUONGTHUCTHANHTOAN")]
[Index("TenPt", Name = "UQ__PHUONGTH__4CF9C7FB1BD10317", IsUnique = true)]
public partial class Phuongthucthanhtoan
{
    [Key]
    [Column("MaPT")]
    public int MaPt { get; set; }

    [Column("TenPT")]
    [StringLength(100)]
    public string TenPt { get; set; } = null!;

    [StringLength(255)]
    public string? MoTa { get; set; }

    public bool? TrangThai { get; set; }

    [InverseProperty("MaPtNavigation")]
    public virtual ICollection<Hoadon> Hoadons { get; set; } = new List<Hoadon>();
}
