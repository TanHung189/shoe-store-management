using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ASP_shopgiay.Models;

[Table("MAUSAC")]
[Index("TenMau", Name = "UQ__MAUSAC__332F6A914246380E", IsUnique = true)]
public partial class Mausac
{
    [Key]
    public int MaMau { get; set; }

    [StringLength(50)]
    public string TenMau { get; set; } = null!;

    [StringLength(7)]
    [Unicode(false)]
    public string? MaHex { get; set; }

    [InverseProperty("MaMauNavigation")]
    public virtual ICollection<BientheSanpham> BientheSanphams { get; set; } = new List<BientheSanpham>();
}
