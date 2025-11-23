using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Linq; // Cần thiết cho .Min(), .Average(), .Any()

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
    [Display(Name = "Mã danh mục")]
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

    // =======================================================
    // <<< THUỘC TÍNH TÍNH TOÁN (RATING) >>>
    // =======================================================
    [NotMapped]
    public double TrungBinhSao
    {
        get
        {
            if (Danhgia == null || !Danhgia.Any()) return 0;

            var validReviews = Danhgia.Where(dg => dg.DaDuyet == true);
            if (!validReviews.Any()) return 0;

            double average = validReviews.Average(dg => (double)dg.SoSao);
            return Math.Round(average * 2) / 2; // Làm tròn tới 0.5 gần nhất
        }
    }

    [NotMapped]
    public int SoLuotDanhGia
    {
        get
        {
            if (Danhgia == null) return 0;
            return Danhgia.Count(dg => dg.DaDuyet == true);
        }
    }

    // =======================================================
    // <<< THUỘC TÍNH TÍNH TOÁN (GIÁ THẤP NHẤT) >>>
    // =======================================================
    [NotMapped]
    public decimal GiaBanThapNhat
    {
        get
        {
            // Kiểm tra và trả về giá bán thấp nhất từ tập hợp BientheSanphams
            if (BientheSanphams == null || !BientheSanphams.Any())
            {
                return 0;
            }
            return BientheSanphams.Min(bt => (decimal?)bt.GiaBan) ?? 0;
        }
    }

    // =======================================================
    // <<< NAVIGATION PROPERTIES >>>
    // =======================================================
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
