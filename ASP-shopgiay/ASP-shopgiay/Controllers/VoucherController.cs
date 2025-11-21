using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASP_shopgiay.Data;
using ASP_shopgiay.Models;
using System;
using System.Threading.Tasks;
namespace ASP_shopgiay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VoucherController(ApplicationDbContext context)
        {
            _context = context;
        }

        // API: Kiểm tra mã giảm giá
        // URL gọi: /api/Voucher/Check?code=TEST10&totalAmount=1000000
        [HttpGet("Check")]
        public async Task<IActionResult> CheckVoucher(string code, decimal totalAmount)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest(new { success = false, message = "Vui lòng nhập mã giảm giá." });
            }

            // Tìm mã trong CSDL (không phân biệt hoa thường)
            var voucher = await _context.Magiamgia!
                .FirstOrDefaultAsync(v => v.Code == code);

            // 1. Kiểm tra tồn tại
            if (voucher == null)
            {
                return NotFound(new { success = false, message = "Mã giảm giá không tồn tại." });
            }

            // 2. Kiểm tra thời hạn
            var now = DateTime.Now;
            if (voucher.NgayBatDau > now)
            {
                return BadRequest(new { success = false, message = "Mã giảm giá chưa đến đợt áp dụng." });
            }
            if (voucher.NgayKetThuc < now)
            {
                return BadRequest(new { success = false, message = "Mã giảm giá đã hết hạn." });
            }

            // 3. Kiểm tra số lượng còn lại
            if (voucher.SoLuong <= 0)
            {
                return BadRequest(new { success = false, message = "Mã giảm giá đã hết lượt sử dụng." });
            }

            // 4. Tính toán số tiền được giảm
            decimal discountAmount = 0;

            if (voucher.SoTienGiam > 0)
            {
                // Giảm theo số tiền cố định (Ví dụ: 50.000đ)
                discountAmount = (decimal)voucher.SoTienGiam;
            }
            else if (voucher.PhanTramGiam > 0)
            {
                // Giảm theo phần trăm (Ví dụ: 10%)
                discountAmount = totalAmount * (decimal)(voucher.PhanTramGiam / 100.0);

                // Kiểm tra giới hạn giảm tối đa (nếu có)
                if (voucher.GiamToiDa.HasValue && voucher.GiamToiDa > 0)
                {
                    if (discountAmount > voucher.GiamToiDa.Value)
                    {
                        discountAmount = (decimal)voucher.GiamToiDa.Value;
                    }
                }
            }

            // Đảm bảo tiền giảm không vượt quá tổng đơn hàng
            if (discountAmount > totalAmount) discountAmount = totalAmount;

            // Trả về kết quả thành công
            return Ok(new
            {
                success = true,
                message = "Áp dụng mã thành công!",
                discount = discountAmount,                 // Số tiền được giảm
                finalTotal = totalAmount - discountAmount, // Tổng tiền sau khi giảm
                voucherCode = voucher.Code
            });
        }
        [HttpGet("Available")]
        public async Task<IActionResult> GetAvailableVouchers()
        {
            var now = DateTime.Now;

            var vouchers = await _context.Magiamgia!
                .Where(v => v.SoLuong > 0 &&
                            (v.NgayBatDau == null || v.NgayBatDau <= now) &&
                            v.NgayKetThuc >= now)
                .Select(v => new
                {
                    v.Code,
                    v.SoTienGiam,
                    v.PhanTramGiam,
                    v.GiamToiDa,
                    MoTa = v.SoTienGiam > 0
                        ? $"Giảm {v.SoTienGiam:N0}đ"
                        : $"Giảm {v.PhanTramGiam}% (Tối đa {v.GiamToiDa:N0}đ)"
                })
                .ToListAsync();

            return Ok(vouchers);
        }
    }
}
