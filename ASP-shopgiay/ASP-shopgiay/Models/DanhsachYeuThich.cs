using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ASP_shopgiay.Models;

[Table("DANHSACH_YEU_THICH")]
[Index("MaTk", "MaBienThe", Name = "UK_YT_TK_BT", IsUnique = true)]
public partial class DanhsachYeuThich
{
    [Key]
    public int MaYeuThich { get; set; }

    [Column("MaTK")]
    public int MaTk { get; set; }

    public int MaBienThe { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayThem { get; set; }

    [ForeignKey("MaBienThe")]
    [InverseProperty("DanhsachYeuThiches")]
    public virtual BientheSanpham MaBienTheNavigation { get; set; } = null!;

    [ForeignKey("MaTk")]
    [InverseProperty("DanhsachYeuThiches")]
    public virtual Taikhoan MaTkNavigation { get; set; } = null!;
}
