using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ASP_shopgiay.Models;
namespace ASP_shopgiay.ViewModels
{
    // Lớp chứa thông tin cần thiết của một biến thể (dùng để hiển thị trong JS)
    public class VariantDetail
    {
        public int MaBienThe { get; set; }
        public int MaMau { get; set; }
        public int MaKichThuoc { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal GiaBan { get; set; }
        public int SoLuongTon { get; set; }
        public string? HinhAnh { get; set; }
    }

    public class ProductDetailViewModel
    {
        // Thông tin Sản phẩm chính
        public Sanpham Product { get; set; } = null!;

        // Tên Thương hiệu 
        public string BrandName { get; set; } = string.Empty;

        // --- Danh sách tùy chọn ---

        // Các Màu sắc có sẵn cho sản phẩm này
        public List<Mausac> AvailableColors { get; set; } = new List<Mausac>();

        // Các Kích thước có sẵn cho sản phẩm này
        public List<Kichthuoc> AvailableSizes { get; set; } = new List<Kichthuoc>();

        // Danh sách TẤT CẢ các biến thể (dùng cho JS xử lý logic chọn màu/size)
        public List<VariantDetail> AllVariants { get; set; } = new List<VariantDetail>();

        // --- Thông tin Đánh giá ---

        [DisplayFormat(DataFormatString = "{0:0.0}")]
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }

        // Danh sách các đánh giá đã được duyệt (Top 5)
        public List<Danhgium> Reviews { get; set; } = new List<Danhgium>();
    }
}

