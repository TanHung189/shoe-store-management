using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASP_shopgiay.Data;

namespace ASP_shopgiay.Areas.Admin.Controllers
{
    public class HomeController : BaseAdminController
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Action này sẽ xử lý đường dẫn /Admin/Home/Index
        public async Task<IActionResult> Index()
        {
            // Lấy dữ liệu thống kê từ các Model
            var stats = new
            {
                // Tổng số sản phẩm
                TotalProducts = await _context.Sanphams.CountAsync(),

                // Tổng số danh mục
                TotalCategories = await _context.Danhmucs.CountAsync(),

                // Tổng số đơn hàng
                TotalOrders = await _context.Hoadons.CountAsync(),

                // Tổng số tài khoản (khách hàng + nhân viên)
                TotalAccounts = await _context.Taikhoans.CountAsync(),

                // Tổng doanh thu (tổng tiền từ các hóa đơn)
                TotalRevenue = await _context.Hoadons.SumAsync(h => h.TongTien ?? 0),

                // Số đơn hàng chưa xác nhận
                PendingOrders = await _context.Hoadons
                    .Where(h => h.MaTrangThai == 1) // 1 = Chờ xác nhận
                    .CountAsync(),

                // Sản phẩm bán chạy nhất (Top 5)
                TopSellingProducts = await _context.Sanphams
                    .OrderByDescending(p => p.LuotMua)
                    .Take(5)
                    .ToListAsync(),

                // Danh mục sản phẩm
                Categories = await _context.Danhmucs
                    .Where(c => c.ParentId == null) // Chỉ danh mục cha
                    .ToListAsync()
            };

            ViewData["TotalProducts"] = stats.TotalProducts;
            ViewData["TotalCategories"] = stats.TotalCategories;
            ViewData["TotalOrders"] = stats.TotalOrders;
            ViewData["TotalAccounts"] = stats.TotalAccounts;
            ViewData["TotalRevenue"] = stats.TotalRevenue;
            ViewData["PendingOrders"] = stats.PendingOrders;
            ViewData["TopSellingProducts"] = stats.TopSellingProducts;
            ViewData["Categories"] = stats.Categories;

            return View();
        }
    }
}