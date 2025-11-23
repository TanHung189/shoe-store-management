using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ASP_shopgiay.Models;

[Table("TAIKHOAN")]
[Index("Email", Name = "UQ__TAIKHOAN__A9D10534E2234D3B", IsUnique = true)]
public partial class Taikhoan
{
    [Key]
    [Column("MaTK")]
    public int MaTk { get; set; }

    [StringLength(100)]
    public string Ten { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string? DienThoai { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [StringLength(256)]
    public string MatKhau { get; set; } = null!;

    public bool? TrangThai { get; set; }

    [Column("MaCV")]
    public int? MaCv { get; set; }

    [InverseProperty("MaTkNavigation")]
    public virtual ICollection<Danhgium> Danhgia { get; set; } = new List<Danhgium>();

    [InverseProperty("MaTkNavigation")]
    public virtual ICollection<DanhsachYeuThich> DanhsachYeuThiches { get; set; } = new List<DanhsachYeuThich>();

    [InverseProperty("MaTkNavigation")]
    public virtual ICollection<Diachi> Diachis { get; set; } = new List<Diachi>();

    [InverseProperty("MaTkNavigation")]
    public virtual ICollection<Giohang> Giohangs { get; set; } = new List<Giohang>();

    [InverseProperty("MaTkNavigation")]
    public virtual ICollection<Hoadon> Hoadons { get; set; } = new List<Hoadon>();

    [ForeignKey("MaCv")]
    [InverseProperty("Taikhoans")]
    public virtual Chucvu? MaCvNavigation { get; set; }
}
