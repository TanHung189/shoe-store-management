using System.Diagnostics;
using System.Linq;
using ASP_shopgiay.Data;
using ASP_shopgiay.Models;
using ASP_shopgiay.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP_shopgiay.Controllers
{
    // ViewModel cho Trang Chủ
    public class HomeViewModel
    {
        public List<Danhmuc> DanhMucCha { get; set; } = new List<Danhmuc>();
        public List<ProductCardViewModel> SanPhamNoiBat { get; set; } = new List<ProductCardViewModel>();
        public List<ProductCardViewModel> SanPhamMoi { get; set; } = new List<ProductCardViewModel>();
    }
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Action xử lý Trang Chủ (Index)
        public async Task<IActionResult> Index()
        {
            // 1. Lấy danh mục cha (ParentID = NULL) cho Menu
            // Sử dụng DbSet Danhmuc và thuộc tính ParentId (PascalCase)
            var danhMucCha = await _context.Danhmucs
                .Where(dm => dm.ParentId == null && dm.TrangThai == true)
                .OrderBy(dm => dm.ThuTu)
                .ToListAsync();

            // 2. Lấy danh sách sản phẩm nổi bật (Top 8 theo LuotMua)
            // Sử dụng DbSet Sanpham
            var sanPhamNoiBat = await GetProductCards(
                _context.Sanphams
                    .OrderByDescending(sp => sp.LuotMua)
                    .Take(8)
            );

            // 3. Lấy danh sách sản phẩm mới (Top 8 theo MaSP)
            var sanPhamMoi = await GetProductCards(
                _context.Sanphams
                    // Sử dụng thuộc tính MaSp (PascalCase)
                    .OrderByDescending(sp => sp.MaSp)
                    .Take(8)
            );

            var viewModel = new HomeViewModel
            {
                DanhMucCha = danhMucCha,
                SanPhamNoiBat = sanPhamNoiBat,
                SanPhamMoi = sanPhamMoi
            };

            return View(viewModel);
        }

        /// <summary>
        /// Hàm helper để chuyển đổi IQueryable<Sanpham> thành List<ProductCardViewModel>,
        /// đồng thời tính toán Giá thấp nhất.
        /// </summary>
        private async Task<List<ProductCardViewModel>> GetProductCards(IQueryable<Sanpham> query)
        {
            // Lấy danh sách MaSp từ truy vấn
            var productIds = await query.Select(sp => sp.MaSp).ToListAsync();

            // Lấy giá thấp nhất cho mỗi sản phẩm từ Biến Thể
            // Sử dụng DbSet BientheSanpham và thuộc tính MaSp (PascalCase)
            var lowestPrices = await _context.BientheSanphams
                .Where(bt => productIds.Contains(bt.MaSp))
                .GroupBy(bt => bt.MaSp)
                .Select(g => new
                {
                    MaSP = g.Key,
                    GiaThapNhat = g.Min(bt => bt.GiaBan)
                })
                .ToDictionaryAsync(x => x.MaSP, x => x.GiaThapNhat);

            // Lấy thông tin SP và THUONGHIEU (dùng Include)
            var products = await query
                // Sử dụng Navigation Property MaThNavigation (Chính xác theo Scaffolding)
                .Include(sp => sp.MaThNavigation)
                .ToListAsync();

            return products.Select(sp => new ProductCardViewModel
            {
                MaSP = sp.MaSp, // Sử dụng MaSp
                TenSP = sp.TenSp!, // Sử dụng TenSp
                HinhAnh = sp.HinhAnh!,
                // Sử dụng Navigation Property MaThNavigation.TenTh (Chính xác theo Scaffolding)
                TenThuongHieu = sp.MaThNavigation!.TenTh,
                GiaThapNhat = lowestPrices.GetValueOrDefault(sp.MaSp, 0),
            }).ToList();
        }
    }
}