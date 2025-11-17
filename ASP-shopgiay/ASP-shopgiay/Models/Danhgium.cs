using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ASP_shopgiay.Models;

[Table("DANHGIA")]
public partial class Danhgium
{
    [Key]
    public int MaDanhGia { get; set; }

    [Column("MaTK")]
    public int MaTk { get; set; }

    [Column("MaSP")]
    public int MaSp { get; set; }

    public byte SoSao { get; set; }

    [StringLength(1000)]
    public string? BinhLuan { get; set; }

    public bool? DaDuyet { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayDanhGia { get; set; }

    [ForeignKey("MaSp")]
    [InverseProperty("Danhgia")]
    public virtual Sanpham MaSpNavigation { get; set; } = null!;

    [ForeignKey("MaTk")]
    [InverseProperty("Danhgia")]
    public virtual Taikhoan MaTkNavigation { get; set; } = null!;
}
