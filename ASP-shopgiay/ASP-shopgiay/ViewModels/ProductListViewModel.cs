using System.Collections.Generic;
using ASP_shopgiay.Models; // Dùng các Models Entity của bạn
using ASP_shopgiay.ViewModels; // ProductCardViewModel nằm trong đây

namespace ASP_shopgiay.ViewModels
{
    public class ProductListViewModel
    {
        // Danh sách Sản phẩm hiển thị trên lưới
        public List<ProductCardViewModel> Products { get; set; } = new List<ProductCardViewModel>();

        // Danh sách tất cả Danh mục (để dùng cho bộ lọc sidebar)
        public List<Danhmuc> AllCategories { get; set; } = new List<Danhmuc>();

        // Danh sách tất cả Thương hiệu (để dùng cho bộ lọc sidebar)
        public List<Thuonghieu> AllBrands { get; set; } = new List<Thuonghieu>();

        // --- Thông tin Phân trang & Lọc hiện tại ---

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 12; // Mặc định mỗi trang 12 sản phẩm

        // Các tham số lọc đang áp dụng
        public int? CurrentCategoryId { get; set; }
        public int? CurrentBrandId { get; set; }
    }
}
