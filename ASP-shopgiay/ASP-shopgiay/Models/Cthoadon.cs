using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ASP_shopgiay.Models;

[Table("CTHOADON")]
public partial class Cthoadon
{
    [Key]
    [Column("MaCTHD")]
    public int MaCthd { get; set; }

    [Column("MaHD")]
    public int MaHd { get; set; }

    public int MaBienThe { get; set; }

    public int SoLuong { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal DonGia { get; set; }

    [Column("GiamGiaCT", TypeName = "decimal(18, 2)")]
    public decimal? GiamGiaCt { get; set; }

    [Column(TypeName = "decimal(29, 2)")]
    public decimal? ThanhTienGoc { get; set; }

    [ForeignKey("MaBienThe")]
    [InverseProperty("Cthoadons")]
    public virtual BientheSanpham MaBienTheNavigation { get; set; } = null!;

    [ForeignKey("MaHd")]
    [InverseProperty("Cthoadons")]
    public virtual Hoadon MaHdNavigation { get; set; } = null!;
}
