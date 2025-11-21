using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ASP_shopgiay.Models;

[Table("DANHMUC")]
[Index("Ten", "ParentId", Name = "UQ_DANHMUC_Ten_Parent", IsUnique = true)]
public partial class Danhmuc
{
    [Key]
    [Column("MaDM")]
    public int MaDm { get; set; }

    [StringLength(100)]
    public string Ten { get; set; } = null!;

    [Column("ParentID")]
    public int? ParentId { get; set; }

    public int? ThuTu { get; set; }

    public bool? TrangThai { get; set; }

    [InverseProperty("Parent")]
    public virtual ICollection<Danhmuc> InverseParent { get; set; } = new List<Danhmuc>();

    [ForeignKey("ParentId")]
    [InverseProperty("InverseParent")]
    public virtual Danhmuc? Parent { get; set; }

    [InverseProperty("MaDmNavigation")]
    public virtual ICollection<Sanpham> Sanphams { get; set; } = new List<Sanpham>();
}
