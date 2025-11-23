using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ASP_shopgiay.Models;

[Table("KICHTHUOC")]
[Index("TenKichThuoc", Name = "UQ__KICHTHUO__9D0743DA5A12B187", IsUnique = true)]
public partial class Kichthuoc
{
    [Key]
    public int MaKichThuoc { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string TenKichThuoc { get; set; } = null!;

    [InverseProperty("MaKichThuocNavigation")]
    public virtual ICollection<BientheSanpham> BientheSanphams { get; set; } = new List<BientheSanpham>();
}
