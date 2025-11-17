using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ASP_shopgiay.Models;

[Table("TRANGTHAI_DONHANG")]
public partial class TrangthaiDonhang
{
    [Key]
    public int MaTrangThai { get; set; }

    [StringLength(100)]
    public string TenTrangThai { get; set; } = null!;

    [StringLength(255)]
    public string? MoTa { get; set; }

    [InverseProperty("MaTrangThaiNavigation")]
    public virtual ICollection<Hoadon> Hoadons { get; set; } = new List<Hoadon>();
}
