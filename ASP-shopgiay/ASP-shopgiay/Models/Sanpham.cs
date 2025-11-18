using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ASP_shopgiay.Models;

[Table("SANPHAM")]
public partial class Sanpham
{
    [Key]
    [Column("MaSP")]
    [Display(Name = "Mã Sản Phẩm")]

    public int MaSp { get; set; }

    [Column("TenSP")]
    [Display(Name = "Tên Sản Phẩm")]
    [StringLength(255)]
    public string TenSp { get; set; } = null!;

    [Column("MaDM")]
    [Display(Name = "Mã doanh mục")]
    public int MaDm { get; set; }

    [Column("MaTH")]
    [Display(Name = "Mã thương hiệu")]
    public int MaTh { get; set; }

    [Display(Name = "Mô tả")]
    public string? MoTa { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    [Display(Name = "Hình ảnh")]
    public string? HinhAnh { get; set; }

    [Display(Name = "Lượt xem")]
    public int? LuotXem { get; set; }

    [Display(Name = "Lượt mua")]
    public int? LuotMua { get; set; }

    [InverseProperty("MaSpNavigation")]
    public virtual ICollection<BientheSanpham> BientheSanphams { get; set; } = new List<BientheSanpham>();

    [InverseProperty("MaSpNavigation")]
    public virtual ICollection<Danhgium> Danhgia { get; set; } = new List<Danhgium>();

    [ForeignKey("MaDm")]
    [InverseProperty("Sanphams")]
    [Display(Name = "Doanh mục")]
    public virtual Danhmuc? MaDmNavigation { get; set; } = null!;

    [ForeignKey("MaTh")]
    [InverseProperty("Sanphams")]
    [Display(Name = "Thương hiệu")]
    public virtual Thuonghieu? MaThNavigation { get; set; } = null!;
}
