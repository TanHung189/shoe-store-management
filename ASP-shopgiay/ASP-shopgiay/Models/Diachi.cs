using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ASP_shopgiay.Models;

[Table("DIACHI")]
public partial class Diachi
{
    [Key]
    [Column("MaDC")]
    public int MaDc { get; set; }

    [Column("MaTK")]
    public int MaTk { get; set; }

    [StringLength(255)]
    public string TenDiaChi { get; set; } = null!;

    [Column("DiaChi")]
    [StringLength(255)]
    public string DiaChi1 { get; set; } = null!;

    [StringLength(100)]
    public string PhuongXa { get; set; } = null!;

    [StringLength(100)]
    public string QuanHuyen { get; set; } = null!;

    [StringLength(100)]
    public string TinhThanh { get; set; } = null!;

    public bool? MacDinh { get; set; }

    [ForeignKey("MaTk")]
    [InverseProperty("Diachis")]
    public virtual Taikhoan MaTkNavigation { get; set; } = null!;
}
