using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ASP_shopgiay.Models;

[Table("BIENTHE_SANPHAM")]
[Index("MaSp", "MaMau", "MaKichThuoc", Name = "UK_BT_SP_MAU_SIZE", IsUnique = true)]
public partial class BientheSanpham
{
    [Key]
    public int MaBienThe { get; set; }

    [Column("MaSP")]
    public int MaSp { get; set; }

    public int MaMau { get; set; }

    public int MaKichThuoc { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal GiaBan { get; set; }

    public int? SoLuong { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? HinhAnh { get; set; }

    [InverseProperty("MaBienTheNavigation")]
    public virtual ICollection<Cthoadon> Cthoadons { get; set; } = new List<Cthoadon>();

    [InverseProperty("MaBienTheNavigation")]
    public virtual ICollection<DanhsachYeuThich> DanhsachYeuThiches { get; set; } = new List<DanhsachYeuThich>();

    [InverseProperty("MaBienTheNavigation")]
    public virtual ICollection<Giohang> Giohangs { get; set; } = new List<Giohang>();

    [ForeignKey("MaKichThuoc")]
    [InverseProperty("BientheSanphams")]
    public virtual Kichthuoc MaKichThuocNavigation { get; set; } = null!;

    [ForeignKey("MaMau")]
    [InverseProperty("BientheSanphams")]
    public virtual Mausac MaMauNavigation { get; set; } = null!;

    [ForeignKey("MaSp")]
    [InverseProperty("BientheSanphams")]
    public virtual Sanpham MaSpNavigation { get; set; } = null!;
}
