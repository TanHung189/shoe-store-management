
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASP_shopgiay.Data;
using ASP_shopgiay.Models;
using Microsoft.AspNetCore.Authorization;

namespace ASP_shopgiay.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class HoaDonController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HoaDonController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/HoaDon
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Hoadons
                .Include(h => h.MaGiamGiaNavigation)
                .Include(h => h.MaPtNavigation)
                .Include(h => h.MaTkNavigation)
                .Include(h => h.MaTrangThaiNavigation)
                .OrderByDescending(h => h.Ngay);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/HoaDon/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoadon = await _context.Hoadons
                .Include(h => h.MaGiamGiaNavigation)
                .Include(h => h.MaPtNavigation)
                .Include(h => h.MaTkNavigation)
                .Include(h => h.MaTrangThaiNavigation)
                .Include(h => h.Cthoadons)
                    .ThenInclude(c => c.MaBienTheNavigation)
                        .ThenInclude(b => b.MaSpNavigation)
                .FirstOrDefaultAsync(m => m.MaHd == id);

            if (hoadon == null)
            {
                return NotFound();
            }

            return View(hoadon);
        }

        // GET: Admin/HoaDon/Create
        public IActionResult Create()
        {
            ViewData["MaGiamGia"] = new SelectList(_context.Magiamgia, "MaGiamGia", "Code");
            ViewData["MaPt"] = new SelectList(_context.Phuongthucthanhtoans, "MaPt", "TenPt");
            ViewData["MaTk"] = new SelectList(_context.Taikhoans.Where(t => t.MaCv == null),
                "MaTk", "Ten");
            ViewData["MaTrangThai"] = new SelectList(_context.TrangthaiDonhangs, "MaTrangThai", "TenTrangThai");
            return View();
        }

        // POST: Admin/HoaDon/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaTk,DiaChiGiaoHang,DienThoaiGiaoHang,TenNguoiNhan,MaTrangThai,MaPt,TamTinh,PhiVanChuyen,MaGiamGia,GiamGia,TongTien")] Hoadon hoadon)
        {
            hoadon.Ngay = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Add(hoadon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Edit), new { id = hoadon.MaHd });
            }

            ViewData["MaGiamGia"] = new SelectList(_context.Magiamgia, "MaGiamGia", "Code", hoadon.MaGiamGia);
            ViewData["MaPt"] = new SelectList(_context.Phuongthucthanhtoans, "MaPt", "TenPt", hoadon.MaPt);
            ViewData["MaTk"] = new SelectList(_context.Taikhoans.Where(t => t.MaCv == null),
                "MaTk", "Ten", hoadon.MaTk);
            ViewData["MaTrangThai"] = new SelectList(_context.TrangthaiDonhangs, "MaTrangThai", "TenTrangThai", hoadon.MaTrangThai);
            return View(hoadon);
        }

        // GET: Admin/HoaDon/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoadon = await _context.Hoadons
                .Include(h => h.Cthoadons)
                    .ThenInclude(c => c.MaBienTheNavigation)
                        .ThenInclude(b => b.MaSpNavigation)
                .Include(h => h.Cthoadons)
                    .ThenInclude(c => c.MaBienTheNavigation)
                        .ThenInclude(b => b.MaMauNavigation)
                .Include(h => h.Cthoadons)
                    .ThenInclude(c => c.MaBienTheNavigation)
                        .ThenInclude(b => b.MaKichThuocNavigation)
                .FirstOrDefaultAsync(hd => hd.MaHd == id);

            if (hoadon == null)
            {
                return NotFound();
            }

            ViewData["MaGiamGia"] = new SelectList(_context.Magiamgia, "MaGiamGia", "Code", hoadon.MaGiamGia);
            ViewData["MaPt"] = new SelectList(_context.Phuongthucthanhtoans, "MaPt", "TenPt", hoadon.MaPt);
            ViewData["MaTk"] = new SelectList(_context.Taikhoans.Where(t => t.MaCv == null),
                "MaTk", "Ten", hoadon.MaTk);
            ViewData["MaTrangThai"] = new SelectList(_context.TrangthaiDonhangs, "MaTrangThai", "TenTrangThai", hoadon.MaTrangThai);
            return View(hoadon);
        }

        // POST: Admin/HoaDon/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaHd,MaTk,Ngay,DiaChiGiaoHang,DienThoaiGiaoHang,TenNguoiNhan,MaTrangThai,MaGiamGia,MaPt,TamTinh,PhiVanChuyen,GiamGia,TongTien")] Hoadon hoadon)
        {
            if (id != hoadon.MaHd)
            {
                return NotFound();
            }

            if (hoadon.MaTk == 0)
            {
                ModelState.AddModelError("MaTk", "User ID is missing!");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hoadon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HoadonExists(hoadon.MaHd))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { id = hoadon.MaHd });
            }
            ViewData["MaGiamGia"] = new SelectList(_context.Magiamgia, "MaGiamGia", "Code", hoadon.MaGiamGia);
            ViewData["MaPt"] = new SelectList(_context.Phuongthucthanhtoans, "MaPt", "TenPt", hoadon.MaPt);
            ViewData["MaTk"] = new SelectList(_context.Taikhoans.Where(t => t.MaCv == null),
                "MaTk", "Ten", hoadon.MaTk);
            ViewData["MaTrangThai"] = new SelectList(_context.TrangthaiDonhangs, "MaTrangThai", "TenTrangThai", hoadon.MaTrangThai);
            return View(hoadon);
        }

        // GET: Admin/HoaDon/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoadon = await _context.Hoadons
                .Include(h => h.MaGiamGiaNavigation)
                .Include(h => h.MaPtNavigation)
                .Include(h => h.MaTkNavigation)
                .Include(h => h.MaTrangThaiNavigation)
                .FirstOrDefaultAsync(m => m.MaHd == id);
            if (hoadon == null)
            {
                return NotFound();
            }

            return View(hoadon);
        }

        // POST: Admin/HoaDon/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hoadon = await _context.Hoadons.FindAsync(id);
            if (hoadon != null)
            {
                _context.Hoadons.Remove(hoadon);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Lấy danh sách các biến thể sản phẩm (BIENTHE_SANPHAM) để hiển thị trong dropdown
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetProductVariants(int? searchQuery = null)
        {
            var variants = await _context.BientheSanphams
                .Include(b => b.MaSpNavigation)
                .Include(b => b.MaMauNavigation)
                .Include(b => b.MaKichThuocNavigation)
                .Where(b => searchQuery == null ||
                    b.MaSpNavigation.TenSp.Contains(searchQuery.ToString()) ||
                    b.MaBienThe == searchQuery)
                .Select(b => new
                {
                    maBienThe = b.MaBienThe,
                    tenSanPham = b.MaSpNavigation.TenSp,
                    tenMau = b.MaMauNavigation.TenMau,
                    tenKichThuoc = b.MaKichThuocNavigation.TenKichThuoc,
                    giaBan = b.GiaBan,
                    soLuong = b.SoLuong,
                    label = $"{b.MaSpNavigation.TenSp} - {b.MaMauNavigation.TenMau} - Size {b.MaKichThuocNavigation.TenKichThuoc}"
                })
                .ToListAsync();

            return Json(variants);
        }

        /// <summary>
        /// Lấy chi tiết một biến thể sản phẩm theo MaBienThe
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetVariantDetail(int id)
        {
            var variant = await _context.BientheSanphams
                .Include(b => b.MaSpNavigation)
                .Include(b => b.MaMauNavigation)
                .Include(b => b.MaKichThuocNavigation)
                .FirstOrDefaultAsync(b => b.MaBienThe == id);

            if (variant == null)
            {
                return NotFound();
            }

            return Json(new
            {
                maBienThe = variant.MaBienThe,
                tenSanPham = variant.MaSpNavigation.TenSp,
                tenMau = variant.MaMauNavigation.TenMau,
                tenKichThuoc = variant.MaKichThuocNavigation.TenKichThuoc,
                giaBan = variant.GiaBan,
                soLuongTon = variant.SoLuong
            });
        }

        /// <summary>
        /// Thêm chi tiết hóa đơn (CTHOADON) - API AJAX
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddOrderDetail(int maHd, int maBienThe, int soLuong, decimal donGia, decimal giamGiaCt = 0)
        {
            try
            {
                var hoadon = await _context.Hoadons.FindAsync(maHd);
                if (hoadon == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
                }

                var bienThe = await _context.BientheSanphams.FindAsync(maBienThe);
                if (bienThe == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy biến thể sản phẩm" });
                }

                // Kiểm tra nếu sản phẩm này đã có trong đơn hàng
                var existingDetail = await _context.Cthoadons
                    .FirstOrDefaultAsync(c => c.MaHd == maHd && c.MaBienThe == maBienThe);

                if (existingDetail != null)
                {
                    // Cập nhật số lượng
                    existingDetail.SoLuong += soLuong;
                    _context.Update(existingDetail);
                }
                else
                {
                    // Thêm mới chi tiết hóa đơn
                    var cthoadon = new Cthoadon
                    {
                        MaHd = maHd,
                        MaBienThe = maBienThe,
                        SoLuong = soLuong,
                        DonGia = donGia,
                        GiamGiaCt = giamGiaCt
                    };
                    _context.Cthoadons.Add(cthoadon);
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Thêm chi tiết hóa đơn thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        /// <summary>
        /// Cập nhật chi tiết hóa đơn - API AJAX
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UpdateOrderDetail(int maCthd, int soLuong, decimal donGia, decimal giamGiaCt = 0)
        {
            try
            {
                var cthoadon = await _context.Cthoadons.FindAsync(maCthd);
                if (cthoadon == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy chi tiết hóa đơn" });
                }

                cthoadon.SoLuong = soLuong;
                cthoadon.DonGia = donGia;
                cthoadon.GiamGiaCt = giamGiaCt;

                _context.Update(cthoadon);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Cập nhật chi tiết hóa đơn thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        /// <summary>
        /// Xóa chi tiết hóa đơn - API AJAX
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> DeleteOrderDetail(int maCthd)
        {
            try
            {
                var cthoadon = await _context.Cthoadons.FindAsync(maCthd);
                if (cthoadon == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy chi tiết hóa đơn" });
                }

                _context.Cthoadons.Remove(cthoadon);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Xóa chi tiết hóa đơn thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        /// <summary>
        /// Lấy danh sách chi tiết hóa đơn hiện tại
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetOrderDetails(int maHd)
        {
            var details = await _context.Cthoadons
                .Where(c => c.MaHd == maHd)
                .Include(c => c.MaBienTheNavigation)
                    .ThenInclude(b => b.MaSpNavigation)
                .Include(c => c.MaBienTheNavigation)
                    .ThenInclude(b => b.MaMauNavigation)
                .Include(c => c.MaBienTheNavigation)
                    .ThenInclude(b => b.MaKichThuocNavigation)
                .Select(c => new
                {
                    maCthd = c.MaCthd,
                    maBienThe = c.MaBienThe,
                    tenSanPham = c.MaBienTheNavigation.MaSpNavigation.TenSp,
                    tenMau = c.MaBienTheNavigation.MaMauNavigation.TenMau,
                    tenKichThuoc = c.MaBienTheNavigation.MaKichThuocNavigation.TenKichThuoc,
                    soLuong = c.SoLuong,
                    donGia = c.DonGia,
                    giamGiaCt = c.GiamGiaCt ?? 0,
                    thanhTien = c.SoLuong * c.DonGia
                })
                .ToListAsync();

            return Json(details);
        }

        /// <summary>
        /// Lấy tất cả biến thể sản phẩm để hiển thị trong combobox
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllProductVariants()
        {
            var variants = await _context.BientheSanphams
                .Include(b => b.MaSpNavigation)
                .Include(b => b.MaMauNavigation)
                .Include(b => b.MaKichThuocNavigation)
                .OrderBy(b => b.MaSpNavigation.TenSp)
                .Select(b => new
                {
                    maBienThe = b.MaBienThe,
                    tenSanPham = b.MaSpNavigation.TenSp,
                    tenMau = b.MaMauNavigation.TenMau,
                    tenKichThuoc = b.MaKichThuocNavigation.TenKichThuoc,
                    giaBan = b.GiaBan,
                    soLuongTon = b.SoLuong ?? 0,
                    displayText = $"{b.MaSpNavigation.TenSp} | {b.MaMauNavigation.TenMau} | Size {b.MaKichThuocNavigation.TenKichThuoc}"
                })
                .ToListAsync();

            return Json(variants);
        }

        /// <summary>
        /// Action này được gọi bằng AJAX để lấy thông tin khách hàng
        /// khi admin chọn một khách hàng từ dropdown.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCustomerInfo(int id)
        {
            var customer = await _context.Taikhoans.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }
            return Json(new
            {
                ten = customer.Ten,
                dienThoai = customer.DienThoai,
            });
        }

        /// <summary>
        /// Action này được gọi bằng AJAX để lấy giá trị (số tiền giảm)
        /// của một mã giảm giá.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetDiscountAmount(int id)
        {
            var coupon = await _context.Magiamgia.FindAsync(id);

            if (coupon == null)
            {
                return NotFound();
            }

            return Json(new
            {
                discountValue = coupon.GiamToiDa
            });
        }

        private bool HoadonExists(int id)
        {
            return _context.Hoadons.Any(e => e.MaHd == id);
        }
    }
}