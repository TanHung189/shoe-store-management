using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ASP_shopgiay.Models;

[Table("GIOHANG")]
[Index("MaTk", "MaBienThe", Name = "UK_GH_TK_BT", IsUnique = true)]
public partial class Giohang
{
    [Key]
    public int MaGioHang { get; set; }

    [Column("MaTK")]
    public int MaTk { get; set; }

    public int MaBienThe { get; set; }

    public int? SoLuong { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayThem { get; set; }

    [ForeignKey("MaBienThe")]
    [InverseProperty("Giohangs")]
    public virtual BientheSanpham MaBienTheNavigation { get; set; } = null!;

    [ForeignKey("MaTk")]
    [InverseProperty("Giohangs")]
    public virtual Taikhoan MaTkNavigation { get; set; } = null!;
}
