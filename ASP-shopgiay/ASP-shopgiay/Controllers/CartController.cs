using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ASP_shopgiay.Data;
using ASP_shopgiay.Models;
using ASP_shopgiay.ViewModels;
using System.Linq;
using System;
using System.Threading.Tasks;
namespace ASP_shopgiay.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- Hàm phụ: Lấy ID người dùng đang đăng nhập ---
        private int GetUserId()
        {
            var userIdClaim = User.FindFirst("MaTK"); // Lấy claim "MaTK" đã lưu lúc đăng nhập
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            // Nếu không tìm thấy (hiếm khi xảy ra do có [Authorize]), ném lỗi hoặc trả về 0
            throw new Exception("Không xác định được người dùng.");
        }

        // ===============================================
        // 1. XEM GIỎ HÀNG (Index)
        // ===============================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = GetUserId();

                // Truy vấn dữ liệu từ bảng GIOHANG, kết nối (Join) với các bảng khác để lấy thông tin chi tiết
                var cartItems = await _context.Giohangs!
                    .Where(gh => gh.MaTk == userId)
                    .Include(gh => gh.MaBienTheNavigation) // Lấy thông tin Biến thể
                        .ThenInclude(bt => bt.MaSpNavigation) // Từ Biến thể lấy thông tin Sản phẩm
                            .ThenInclude(sp => sp.MaThNavigation) // Từ Sản phẩm lấy Thương hiệu
                    .Include(gh => gh.MaBienTheNavigation)
                        .ThenInclude(bt => bt.MaMauNavigation) // Lấy Màu
                    .Include(gh => gh.MaBienTheNavigation)
                        .ThenInclude(bt => bt.MaKichThuocNavigation) // Lấy Size
                    .ToListAsync();

                // Chuyển đổi sang ViewModel để hiển thị
                var viewModel = new CartViewModel();
                foreach (var item in cartItems)
                {
                    var variant = item.MaBienTheNavigation;
                    var product = variant!.MaSpNavigation;

                    viewModel.Items.Add(new CartItemViewModel
                    {
                        MaGioHang = item.MaGioHang,
                        MaBienThe = item.MaBienThe,
                        TenSanpham = product!.TenSp,
                        TenThuongHieu = product.MaThNavigation?.TenTh ?? "N/A",
                        TenMau = variant.MaMauNavigation?.TenMau ?? "N/A",
                        TenKichThuoc = variant.MaKichThuocNavigation?.TenKichThuoc ?? "N/A",
                        // Ưu tiên ảnh biến thể, nếu không có thì lấy ảnh sản phẩm
                        HinhAnh = !string.IsNullOrEmpty(variant.HinhAnh) ? variant.HinhAnh : product.HinhAnh!,
                        SoLuong = item.SoLuong ?? 1,
                        DonGia = variant.GiaBan,
                        SoLuongTon = variant.SoLuong ?? 0
                    });
                }

                return View(viewModel);
            }
            catch
            {
                // Nếu lỗi lấy ID (ví dụ cookie hết hạn), logout ra trang chủ
                return RedirectToAction("Logout", "Account");
            }
        }

        // ===============================================
        // 2. THÊM VÀO GIỎ HÀNG (Add To Cart)
        // ===============================================
        [HttpPost]
        public async Task<IActionResult> AddToCart(int variantId, int quantity)
        {
            // Lấy User ID
            int userId;
            try { userId = GetUserId(); }
            catch { return RedirectToAction("Login", "Account"); }

            if (quantity <= 0) quantity = 1;

            // 1. Kiểm tra tồn kho của biến thể
            var variant = await _context.BientheSanphams!.FindAsync(variantId);
            if (variant == null) return NotFound();

            if ((variant.SoLuong ?? 0) < quantity)
            {
                TempData["Error"] = "Số lượng yêu cầu vượt quá tồn kho!";
                return RedirectToAction("Detail", "Product", new { productId = variant.MaSp });
            }

            // 2. Kiểm tra sản phẩm đã có trong giỏ chưa
            var cartItem = await _context.Giohangs!
                .FirstOrDefaultAsync(gh => gh.MaTk == userId && gh.MaBienThe == variantId);

            if (cartItem != null)
            {
                // Đã có -> Cộng dồn số lượng
                cartItem.SoLuong += quantity;

                // Kiểm tra lại xem cộng dồn có lố tồn kho không
                if (cartItem.SoLuong > variant.SoLuong)
                {
                    cartItem.SoLuong = variant.SoLuong;
                    TempData["Warning"] = "Đã thêm tối đa số lượng có thể mua.";
                }
                else
                {
                    TempData["Success"] = "Đã cập nhật số lượng trong giỏ hàng!";
                }
                _context.Update(cartItem);
            }
            else
            {
                // Chưa có -> Tạo mới
                var newItem = new Giohang
                {
                    MaTk = userId,
                    MaBienThe = variantId,
                    SoLuong = quantity,
                    NgayThem = DateTime.Now
                };
                _context.Add(newItem);
                TempData["Success"] = "Đã thêm sản phẩm vào giỏ hàng!";
            }

            await _context.SaveChangesAsync();

            // Chuyển hướng đến trang Giỏ hàng để khách xem
            return RedirectToAction("Index");
        }

        // ===============================================
        // 3. CẬP NHẬT SỐ LƯỢNG (Update Cart)
        // ===============================================
        [HttpPost]
        public async Task<IActionResult> UpdateCart(int maGioHang, int soLuongMoi)
        {
            int userId;
            try { userId = GetUserId(); } catch { return RedirectToAction("Login", "Account"); }

            if (soLuongMoi <= 0)
            {
                // Nếu nhập <= 0 thì coi như muốn xóa
                return RedirectToAction("RemoveFromCart", new { maGioHang });
            }

            var cartItem = await _context.Giohangs!
                .Include(gh => gh.MaBienTheNavigation)
                .FirstOrDefaultAsync(gh => gh.MaGioHang == maGioHang && gh.MaTk == userId);

            if (cartItem != null)
            {
                // Kiểm tra tồn kho
                int tonKho = cartItem.MaBienTheNavigation?.SoLuong ?? 0;
                if (soLuongMoi > tonKho)
                {
                    cartItem.SoLuong = tonKho;
                    TempData["Warning"] = $"Số lượng tối đa là {tonKho}. Đã điều chỉnh.";
                }
                else
                {
                    cartItem.SoLuong = soLuongMoi;
                    TempData["Success"] = "Cập nhật giỏ hàng thành công.";
                }
                _context.Update(cartItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // ===============================================
        // 4. XÓA KHỎI GIỎ HÀNG (Remove)
        // ===============================================
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int maGioHang)
        {
            int userId;
            try { userId = GetUserId(); } catch { return RedirectToAction("Login", "Account"); }

            var cartItem = await _context.Giohangs!
                .FirstOrDefaultAsync(gh => gh.MaGioHang == maGioHang && gh.MaTk == userId);

            if (cartItem != null)
            {
                _context.Giohangs!.Remove(cartItem);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã xóa sản phẩm khỏi giỏ hàng.";
            }

            return RedirectToAction("Index");
        }
        
    }
}
