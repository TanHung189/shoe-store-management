using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ASP_shopgiay.Models;

[Table("MAGIAMGIA")]
[Index("Code", Name = "UQ__MAGIAMGI__A25C5AA747E4D84B", IsUnique = true)]
public partial class Magiamgium
{
    [Key]
    public int MaGiamGia { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Code { get; set; } = null!;

    public int? SoTienGiam { get; set; }

    public double? PhanTramGiam { get; set; }

    public int? GiamToiDa { get; set; }

    public int? SoLuong { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayBatDau { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime NgayKetThuc { get; set; }

    [InverseProperty("MaGiamGiaNavigation")]
    public virtual ICollection<Hoadon> Hoadons { get; set; } = new List<Hoadon>();
}
