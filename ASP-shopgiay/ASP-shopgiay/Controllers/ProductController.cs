using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASP_shopgiay.Data;
using ASP_shopgiay.Models;
using ASP_shopgiay.ViewModels;
using System.Linq;

namespace ASP_shopgiay.Controllers
{
    public class ProductController : Controller
    {
        // Khai báo DbContext(Giữ tên DbContext của bạn: ApplicationDbContext)
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Action xử lý Trang Danh sách (List)
        // Cho phép lọc theo Danh mục (categoryId), Thương hiệu (brandId) và Phân trang (page)
        public async Task<IActionResult> List(int? categoryId, int? brandId, int page = 1)
        {
            // Truy vấn cơ sở dữ liệu (Sử dụng DbSet Sanpham!)
            var allProducts = _context.Sanphams.AsQueryable();

            // Áp dụng Lọc theo Danh mục (MaDm)
            if (categoryId.HasValue)
            {
                allProducts = allProducts.Where(sp => sp.MaDm == categoryId.Value);
            }

            // Áp dụng Lọc theo Thương hiệu (MaTh)
            if (brandId.HasValue)
            {
                allProducts = allProducts.Where(sp => sp.MaTh == brandId.Value);
            }

            // --- 1. Phân trang ---
            int pageSize = 12;
            int totalItems = await allProducts.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var productsToSkip = (page - 1) * pageSize;

            var productQuery = allProducts
                // Sắp xếp theo MaSp (Mã sản phẩm)
                .OrderBy(sp => sp.MaSp)
                .Skip(productsToSkip)
                .Take(pageSize);

            // Chuyển đổi Sanpham sang ProductCardViewModel (dùng hàm helper)
            var productCardViewModels = await GetProductCards(productQuery);


            // --- 2. Lấy dữ liệu cho Sidebar/Bộ lọc (Sử dụng DbSet Danhmuc! và Thuonghieu!) ---
            var allCategories = await _context.Danhmucs.ToListAsync();
            var allBrands = await _context.Thuonghieus.ToListAsync();


            // --- 3. Tạo và trả về ViewModel ---
            var viewModel = new ProductListViewModel
            {
                Products = productCardViewModels,
                AllCategories = allCategories,
                AllBrands = allBrands,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                CurrentCategoryId = categoryId,
                CurrentBrandId = brandId
            };

            return View(viewModel);
        }

        /// <summary>
        /// Hàm helper để chuyển đổi IQueryable<Sanpham> thành List<ProductCardViewModel>,
        /// đồng thời tính toán Giá thấp nhất. (Giống như trong HomeController)
        /// </summary>
        private async Task<List<ProductCardViewModel>> GetProductCards(IQueryable<Sanpham> query)
        {
            // Lấy danh sách MaSp từ truy vấn
            var productIds = await query.Select(sp => sp.MaSp).ToListAsync();

            // Lấy giá thấp nhất cho mỗi sản phẩm từ Biến Thể
            // Sử dụng DbSet BientheSanpham! và thuộc tính MaSp (PascalCase)
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
                MaSP = sp.MaSp,
                TenSP = sp.TenSp!,
                HinhAnh = sp.HinhAnh!,
                // Sử dụng Navigation Property MaThNavigation.TenTh (Chính xác theo Scaffolding)
                TenThuongHieu = sp.MaThNavigation!.TenTh,
                GiaThapNhat = lowestPrices.GetValueOrDefault(sp.MaSp, 0),
            }).ToList();
        }

        // Action Chi tiết Sản phẩm (Sẽ được xây dựng trong GIAI ĐOẠN 3)

        // public async Task<IActionResult> Detail(int productId) { ... }
        // GIAI ĐOẠN 3: Action xử lý Trang Chi tiết Sản phẩm (DETAIL)
        // ===============================================
        public async Task<IActionResult> Detail(int productId)
        {
            // Truy vấn sản phẩm chính và các mối quan hệ cần thiết
            var product = await _context.Sanphams!
                .Include(sp => sp.MaThNavigation)    // Thương hiệu
                .Include(sp => sp.MaDmNavigation)    // Danh mục
                .Include(sp => sp.Danhgia!)          // Đánh giá
                .FirstOrDefaultAsync(sp => sp.MaSp == productId);

            if (product == null)
            {
                return NotFound();
            }

            // Lấy TẤT CẢ biến thể có sẵn (Số lượng > 0)
            var variants = await _context.BientheSanphams!
                .Where(bt => bt.MaSp == productId && (bt.SoLuong ?? 0) > 0)
                .Include(bt => bt.MaMauNavigation)
                .Include(bt => bt.MaKichThuocNavigation)
                .ToListAsync();

            // --- Xử lý Dữ liệu cho ViewModel ---

            // 1. Lấy danh sách Màu sắc và Kích thước duy nhất
            // (Sử dụng DistinctBy để tránh lặp lại màu sắc/kích thước)
            var availableColors = variants
                .Select(v => v.MaMauNavigation!)
                .DistinctBy(c => c.MaMau)
                .ToList();

            var availableSizes = variants
                .Select(v => v.MaKichThuocNavigation!)
                .DistinctBy(s => s.MaKichThuoc)
                .ToList();

            // 2. Chuẩn bị danh sách tất cả Variants cho JavaScript
            var allVariantsForJs = variants.Select(v => new VariantDetail
            {
                MaBienThe = v.MaBienThe,
                MaMau = v.MaMau,
                MaKichThuoc = v.MaKichThuoc,
                GiaBan = v.GiaBan,
                SoLuongTon = v.SoLuong ?? 0,
                HinhAnh = v.HinhAnh
            }).ToList();

            // 3. Tính toán Rating
            var reviews = product.Danhgia.Where(dg => dg.DaDuyet == true).ToList();
            // Hàm tính trung bình sao (làm tròn tới 0.5 gần nhất)
            double avgRating = reviews.Any() ? Math.Round(reviews.Average(dg => (double)dg.SoSao) * 2) / 2 : 0;

            // --- Xây dựng ViewModel ---
            var viewModel = new ProductDetailViewModel
            {
                Product = product,
                BrandName = product.MaThNavigation!.TenTh,
                AvailableColors = availableColors,
                AvailableSizes = availableSizes,
                AllVariants = allVariantsForJs,
                AverageRating = avgRating,
                ReviewCount = reviews.Count,
                Reviews = reviews.OrderByDescending(dg => dg.NgayDanhGia).Take(5).ToList() // Lấy 5 đánh giá gần nhất
            };

            return View(viewModel);
        }
    }

}

