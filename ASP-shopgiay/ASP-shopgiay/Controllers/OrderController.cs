using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ASP_shopgiay.Data;
using ASP_shopgiay.Models;
using ASP_shopgiay.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
namespace ASP_shopgiay.Controllers
{
    [Authorize] // Bắt buộc đăng nhập mới được thực hiện đặt hàng
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst("MaTK");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return 0;
        }

        // ==========================================
        // 1. HIỂN THỊ TRANG THANH TOÁN (GET)
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Checkout(string? voucherCode) // <--- Nhận mã từ URL
        {
            var userId = GetUserId();
            if (userId == 0) return RedirectToAction("Login", "Account");

            // 1. Lấy giỏ hàng
            var cartItems = await _context.Giohangs!
                .Where(gh => gh.MaTk == userId)
                .Include(gh => gh.MaBienTheNavigation)
                    .ThenInclude(bt => bt.MaSpNavigation)
                .Include(gh => gh.MaBienTheNavigation)
                    .ThenInclude(bt => bt.MaMauNavigation)
                .Include(gh => gh.MaBienTheNavigation)
                    .ThenInclude(bt => bt.MaKichThuocNavigation)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Warning"] = "Giỏ hàng trống.";
                return RedirectToAction("Index", "Cart");
            }

            // 2. Tính toán cơ bản
            decimal tamTinh = cartItems.Sum(i => (i.SoLuong ?? 0) * i.MaBienTheNavigation.GiaBan);
            decimal giamGia = 0;

            // 3. XỬ LÝ MÃ GIẢM GIÁ (Server-side Check)
            if (!string.IsNullOrEmpty(voucherCode))
            {
                var voucher = await _context.Magiamgia!
                    .FirstOrDefaultAsync(v => v.Code == voucherCode);

                // Kiểm tra điều kiện Voucher: Tồn tại, Còn lượt, Còn hạn
                if (voucher != null && voucher.SoLuong > 0 &&
                    (voucher.NgayBatDau == null || voucher.NgayBatDau <= DateTime.Now) &&
                    voucher.NgayKetThuc >= DateTime.Now)
                {
                    // Tính tiền giảm
                    if (voucher.SoTienGiam > 0)
                    {
                        giamGia = (decimal)voucher.SoTienGiam;
                    }
                    else if (voucher.PhanTramGiam > 0)
                    {
                        giamGia = tamTinh * (decimal)(voucher.PhanTramGiam / 100.0);
                        if (voucher.GiamToiDa.HasValue && giamGia > voucher.GiamToiDa)
                        {
                            giamGia = (decimal)voucher.GiamToiDa;
                        }
                    }

                    // Không giảm quá giá trị đơn hàng
                    if (giamGia > tamTinh) giamGia = tamTinh;

                    // Lưu thông tin để hiển thị bên View
                    ViewData["VoucherCode"] = voucher.Code;
                    ViewData["DiscountAmount"] = giamGia;
                    ViewData["SuccessMessage"] = "Đã áp dụng mã giảm giá!";
                }
                else
                {
                    // Mã không hợp lệ (hoặc hết hạn ngay lúc chuyển trang)
                    ViewData["ErrorMessage"] = "Mã giảm giá không hợp lệ hoặc đã hết hạn.";
                }
            }

            // 4. Chuẩn bị ViewModel
            var user = await _context.Taikhoans!.FindAsync(userId);
            var viewModel = new CheckoutViewModel
            {
                TenNguoiNhan = user?.Ten ?? "",
                DienThoaiGiaoHang = user?.DienThoai ?? "",
                CartItems = cartItems.Select(item => new CartItemViewModel
                {
                    TenSanpham = item.MaBienTheNavigation.MaSpNavigation.TenSp,
                    TenMau = item.MaBienTheNavigation.MaMauNavigation.TenMau,
                    TenKichThuoc = item.MaBienTheNavigation.MaKichThuocNavigation.TenKichThuoc,
                    DonGia = item.MaBienTheNavigation.GiaBan,
                    SoLuong = item.SoLuong ?? 1,
                    HinhAnh = item.MaBienTheNavigation.HinhAnh ?? item.MaBienTheNavigation.MaSpNavigation.HinhAnh
                }).ToList(),

                // Tổng tiền cuối cùng = Tạm tính - Giảm giá
                TongTien = tamTinh - giamGia
            };

            return View(viewModel);
        }

        // ==========================================
        // 2. XỬ LÝ ĐẶT HÀNG (POST)
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Thêm tham số voucherCode để nhận lại từ Form khi submit
        public async Task<IActionResult> Checkout(CheckoutViewModel model, string? voucherCode)
        {
            var userId = GetUserId();
            var cartItems = await _context.Giohangs!
                .Where(gh => gh.MaTk == userId)
                .Include(gh => gh.MaBienTheNavigation)
                .ToListAsync();

            if (!cartItems.Any()) return RedirectToAction("Index", "Cart");

            if (ModelState.IsValid)
            {
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    // --- TÍNH TOÁN LẠI TIỀN (Bảo mật) ---
                    decimal tamTinh = cartItems.Sum(i => (i.SoLuong ?? 0) * i.MaBienTheNavigation.GiaBan);
                    decimal giamGia = 0;
                    int? maGiamGiaId = null;

                    // Kiểm tra voucher lần cuối trước khi trừ kho/trừ tiền
                    if (!string.IsNullOrEmpty(voucherCode))
                    {
                        var voucher = await _context.Magiamgia!
                            .FirstOrDefaultAsync(v => v.Code == voucherCode);

                        if (voucher != null && voucher.SoLuong > 0 &&
                            (voucher.NgayBatDau == null || voucher.NgayBatDau <= DateTime.Now) &&
                            voucher.NgayKetThuc >= DateTime.Now)
                        {
                            if (voucher.SoTienGiam > 0) giamGia = (decimal)voucher.SoTienGiam;
                            else if (voucher.PhanTramGiam > 0)
                            {
                                giamGia = tamTinh * (decimal)(voucher.PhanTramGiam / 100.0);
                                if (voucher.GiamToiDa.HasValue && giamGia > voucher.GiamToiDa)
                                    giamGia = (decimal)voucher.GiamToiDa;
                            }
                            if (giamGia > tamTinh) giamGia = tamTinh;

                            maGiamGiaId = voucher.MaGiamGia;

                            // Trừ số lượng Voucher
                            voucher.SoLuong -= 1;
                            _context.Magiamgia!.Update(voucher);
                        }
                    }

                    // A. Tạo Hóa Đơn
                    var hoadon = new Hoadon
                    {
                        MaTk = userId,
                        Ngay = DateTime.Now,
                        TenNguoiNhan = model.TenNguoiNhan,
                        DiaChiGiaoHang = model.DiaChiGiaoHang,
                        DienThoaiGiaoHang = model.DienThoaiGiaoHang,
                        MaTrangThai = 1, // 1 = Chờ xác nhận
                        MaPt = model.MaPt,
                        MaGiamGia = maGiamGiaId, // Lưu ID voucher đã dùng

                        TamTinh = tamTinh,
                        PhiVanChuyen = 30000, // Phí ship mặc định
                        GiamGia = giamGia
                    };
                    hoadon.TongTien = hoadon.TamTinh + hoadon.PhiVanChuyen - hoadon.GiamGia;

                    _context.Hoadons!.Add(hoadon);
                    await _context.SaveChangesAsync();

                    // B. Chi tiết & Trừ kho
                    foreach (var item in cartItems)
                    {
                        var cthd = new Cthoadon
                        {
                            MaHd = hoadon.MaHd,
                            MaBienThe = item.MaBienThe,
                            SoLuong = item.SoLuong ?? 1,
                            DonGia = item.MaBienTheNavigation.GiaBan,
                            GiamGiaCt = 0
                        };
                        _context.Cthoadons!.Add(cthd);

                        var variant = item.MaBienTheNavigation;
                        if ((variant.SoLuong ?? 0) >= item.SoLuong)
                        {
                            variant.SoLuong -= item.SoLuong;
                            _context.BientheSanphams!.Update(variant);
                        }
                        else
                        {
                            throw new Exception($"Sản phẩm {variant.MaBienThe} không đủ hàng.");
                        }
                    }

                    // C. Xóa giỏ hàng
                    _context.Giohangs!.RemoveRange(cartItems);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return RedirectToAction("OrderSuccess", new { id = hoadon.MaHd });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "Lỗi xử lý: " + ex.Message);
                }
            }

            // Load lại dữ liệu hiển thị nếu lỗi
            // (Lưu ý: Cần load lại cartItems vào ViewModel như phần GET)
            return View(model);
        }

        public IActionResult OrderSuccess(int id)
        {
            return View(id);
        }

        public async Task<IActionResult> History()
        {
            var userId = GetUserId();
            var orders = await _context.Hoadons!
                .Where(h => h.MaTk == userId)
                .OrderByDescending(h => h.Ngay)
                .Include(h => h.Cthoadons)
                .ToListAsync();
            return View(orders);
        }
    }
}

