using System.Diagnostics;
using ASP_shopgiay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ASP_shopgiay.Data;

namespace ASP_shopgiay.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private const int PageSize = 8;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Phương thức trợ giúp: Lấy MaSP và Giá bán MIN (truy vấn SQL)
        private IQueryable<ProductPrice> GetProductPriceIdQuery()
        {
            return _context.Sanphams
                        .Select(sp => new ProductPrice
                        {
                            MaSp = sp.MaSp,
                            MinPrice = sp.BientheSanphams.Min(bt => (decimal?)bt.GiaBan) ?? 0
                        })
                        // Sắp xếp chính: Giá giảm dần, Sắp xếp phụ: MaSp giảm dần
                        .OrderByDescending(p => p.MinPrice)
                        .ThenByDescending(p => p.MaSp)
                        .AsQueryable();
        }

        // ===============================================
        // INDEX - Load trang chủ (SẮP XẾP GIÁ CAO -> THẤP)
        // ===============================================
        public async Task<IActionResult> Index()
        {
            // 1. Lấy danh sách MaSP (ID) duy nhất cần thiết cho trang đầu tiên (Đã sắp xếp theo Giá)
            var requiredMaSps = await GetProductPriceIdQuery()
                                        .Select(p => p.MaSp)
                                        .Take(PageSize)
                                        .ToListAsync();

            // 2. Tải toàn bộ đối tượng Sanpham dựa trên danh sách ID đã lọc (Chỉ lọc, không sắp xếp SQL)
            var products = await _context.Sanphams
                                .Include(sp => sp.Danhgia)
                                .Include(sp => sp.BientheSanphams)
                                .Where(sp => requiredMaSps.Contains(sp.MaSp))
                                .ToListAsync();

            // 3. Sắp xếp lại danh sách trên bộ nhớ (Client-side sorting) theo thứ tự của requiredMaSps
            var orderedProducts = products
                .OrderBy(p => requiredMaSps.IndexOf(p.MaSp))
                .ToList();

            // 4. Kiểm tra HasMore
            var totalCount = await _context.Sanphams.Select(sp => sp.MaSp).Distinct().CountAsync();
            ViewBag.HasMore = (totalCount > orderedProducts.Count);

            // 5. Lấy Danh mục
            ViewBag.DanhMuc = await _context.Danhmucs
                                            .Where(dm => dm.ParentId == null && dm.TrangThai == true)
                                            .OrderBy(dm => dm.ThuTu)
                                            .ToListAsync();

            ViewData["IsHome"] = true;

            return View(orderedProducts); // Trả về danh sách đã sắp xếp
        }

        // ===============================================
        // LOAD MORE - Tải thêm sản phẩm (Phân trang theo GiaBan + MaSP)
        // ===============================================
        [HttpGet]
        public async Task<IActionResult> LoadMore(decimal? lastPrice = null, int? lastId = null)
        {
            var productMinPriceQuery = GetProductPriceIdQuery();

            // 1. Lọc Compound Key
            if (lastPrice.HasValue && lastId.HasValue)
            {
                decimal lastPriceValue = lastPrice.Value;
                int lastIdValue = lastId.Value;

                productMinPriceQuery = productMinPriceQuery.Where(p =>
                    p.MinPrice < lastPriceValue ||
                    (p.MinPrice == lastPriceValue && p.MaSp < lastIdValue)
                );
            }

            // 2. Lấy danh sách các MaSP cần tải
            var requiredMaSps = await productMinPriceQuery
                                        .Select(p => p.MaSp)
                                        .Take(PageSize)
                                        .ToListAsync();

            // 3. Tải toàn bộ đối tượng Sanpham (Không sắp xếp SQL)
            var newProducts = await _context.Sanphams
                                .Include(sp => sp.Danhgia)
                                .Include(sp => sp.BientheSanphams)
                                .Where(sp => requiredMaSps.Contains(sp.MaSp))
                                .ToListAsync();

            // 4. Sắp xếp lại danh sách trên bộ nhớ (Client-side sorting)
            var orderedNewProducts = newProducts
                .OrderBy(p => requiredMaSps.IndexOf(p.MaSp))
                .ToList();

            // 5. Kiểm tra HasMore
            var lastLoadedProduct = orderedNewProducts.LastOrDefault();
            var hasMore = false;

            if (lastLoadedProduct != null)
            {
                decimal lastPriceLoaded = lastLoadedProduct.GiaBanThapNhat;
                int lastLoadedId = lastLoadedProduct.MaSp;

                // Kiểm tra xem còn sản phẩm nào thỏa mãn điều kiện Compound Key hay không
                hasMore = await GetProductPriceIdQuery().Where(p =>
                       p.MinPrice < lastPriceLoaded ||
                       (p.MinPrice == lastPriceLoaded && p.MaSp < lastLoadedId)
                   ).AnyAsync();
            }

            ViewBag.HasMore = hasMore;
            ViewData["IsHome"] = true;

            return PartialView("_ProductListPartial", orderedNewProducts);
        }
        // ===============================================
        // Các Action khác (giữ nguyên)
        // ===============================================
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}