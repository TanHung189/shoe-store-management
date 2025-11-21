using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ASP_shopgiay.Models;

[Table("HOADON")]
public partial class Hoadon
{
    [Key]
    [Column("MaHD")]
    public int MaHd { get; set; }

    [Column("MaTK")]
    public int MaTk { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Ngay { get; set; }

    [StringLength(255)]
    public string DiaChiGiaoHang { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string DienThoaiGiaoHang { get; set; } = null!;

    [StringLength(100)]
    public string TenNguoiNhan { get; set; } = null!;

    public int MaTrangThai { get; set; }

    public int? MaGiamGia { get; set; }

    [Column("MaPT")]
    public int MaPt { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TamTinh { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PhiVanChuyen { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? GiamGia { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? TongTien { get; set; }

    [InverseProperty("MaHdNavigation")]
    public virtual ICollection<Cthoadon> Cthoadons { get; set; } = new List<Cthoadon>();

    [ForeignKey("MaGiamGia")]
    [InverseProperty("Hoadons")]
    public virtual Magiamgium? MaGiamGiaNavigation { get; set; }

    [ForeignKey("MaPt")]
    [InverseProperty("Hoadons")]
    public virtual Phuongthucthanhtoan MaPtNavigation { get; set; } = null!;

    [ForeignKey("MaTk")]
    [InverseProperty("Hoadons")]
    public virtual Taikhoan MaTkNavigation { get; set; } = null!;

    [ForeignKey("MaTrangThai")]
    [InverseProperty("Hoadons")]
    public virtual TrangthaiDonhang MaTrangThaiNavigation { get; set; } = null!;
}
