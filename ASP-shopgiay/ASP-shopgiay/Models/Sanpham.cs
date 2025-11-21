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
    public int MaSp { get; set; }

    [Column("TenSP")]
    [StringLength(255)]
    public string TenSp { get; set; } = null!;

    [Column("MaDM")]
    public int MaDm { get; set; }

    [Column("MaTH")]
    public int MaTh { get; set; }

    public string? MoTa { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? HinhAnh { get; set; }

    public int? LuotXem { get; set; }

    public int? LuotMua { get; set; }

    [InverseProperty("MaSpNavigation")]
    public virtual ICollection<BientheSanpham> BientheSanphams { get; set; } = new List<BientheSanpham>();

    [InverseProperty("MaSpNavigation")]
    public virtual ICollection<Danhgium> Danhgia { get; set; } = new List<Danhgium>();

    [ForeignKey("MaDm")]
    [InverseProperty("Sanphams")]
    public virtual Danhmuc MaDmNavigation { get; set; } = null!;

    [ForeignKey("MaTh")]
    [InverseProperty("Sanphams")]
    public virtual Thuonghieu MaThNavigation { get; set; } = null!;
}
