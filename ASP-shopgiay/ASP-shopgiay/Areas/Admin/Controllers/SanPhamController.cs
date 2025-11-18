using ASP_shopgiay.Data;
using ASP_shopgiay.Models;
using ASP_shopgiay.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASP_shopgiay.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class SanPhamController : Controller
    {
        private readonly ApplicationDbContext _context;

        private IPhotoService _photoService;

        public SanPhamController(ApplicationDbContext context, IPhotoService photoService)
        {
            _context = context;
            _photoService = photoService;
        }

        // GET: Sanphams
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Sanphams.Include(s => s.MaDmNavigation).Include(s => s.MaThNavigation);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Sanphams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanpham = await _context.Sanphams
                .Include(s => s.MaDmNavigation)
                .Include(s => s.MaThNavigation)
                .FirstOrDefaultAsync(m => m.MaSp == id);
            if (sanpham == null)
            {
                return NotFound();
            }

            return View(sanpham);
        }

        // GET: Sanphams/Create
        public IActionResult Create()
        {
            ViewData["MaDm"] = new SelectList(_context.Danhmucs, "MaDm", "Ten");
            ViewData["MaTh"] = new SelectList(_context.Thuonghieus, "MaTh", "TenTh");
            return View();
        }

        // POST: Sanphams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("TenSp,MaDm,MaTh,MoTa")] Sanpham sanpham,
            IFormFile hinhAnhFile) // <-- Tham số file ảnh
        {
            if (ModelState.IsValid)
            {
                // 4. Xử lý Upload ảnh
                if (hinhAnhFile != null && hinhAnhFile.Length > 0)
                {
                    // Gọi service để upload
                    var uploadResult = await _photoService.AddPhotoAsync(hinhAnhFile);

                    // Nếu upload lỗi, trả về thông báo
                    if (uploadResult.Error != null)
                    {
                        ModelState.AddModelError("", "Lỗi khi upload ảnh: " + uploadResult.Error.Message);
                        // (Phải tải lại SelectList nếu có lỗi)
                        ViewData["MaDm"] = new SelectList(_context.Danhmucs, "MaDm", "Ten", sanpham.MaDm);
                        ViewData["MaTh"] = new SelectList(_context.Thuonghieus, "MaTh", "TenTh", sanpham.MaTh);
                        return View(sanpham);
                    }

                    // 5. Nếu thành công, gán URL vào model
                    sanpham.HinhAnh = uploadResult.SecureUrl.ToString();
                }
                else
                {
                    // (Tùy chọn) Gán ảnh mặc định nếu không upload
                    sanpham.HinhAnh = "/images/default-product.png";
                }

                _context.Add(sanpham);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Nếu ModelState không hợp lệ, tải lại SelectList
            ViewData["MaDm"] = new SelectList(_context.Danhmucs, "MaDm", "Ten", sanpham.MaDm);
            ViewData["MaTh"] = new SelectList(_context.Thuonghieus, "MaTh", "TenTh", sanpham.MaTh);
            return View(sanpham);
        }

        // GET: Sanphams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanpham = await _context.Sanphams.FindAsync(id);
            if (sanpham == null)
            {
                return NotFound();
            }
            ViewData["MaDm"] = new SelectList(_context.Danhmucs, "MaDm", "Ten", sanpham.MaDm);
            ViewData["MaTh"] = new SelectList(_context.Thuonghieus, "MaTh", "TenTh", sanpham.MaTh);
            return View(sanpham);
        }

        // POST: Sanphams/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("MaSp,TenSp,MaDm,MaTh,MoTa,HinhAnh,LuotXem,LuotMua")] Sanpham sanpham,
            IFormFile? hinhAnhFile) // <-- Thêm IFormFile (có thể null)
        {
            if (id != sanpham.MaSp)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // XỬ LÝ UPLOAD ẢNH MỚI (NẾU CÓ)
                    if (hinhAnhFile != null && hinhAnhFile.Length > 0)
                    {
                        // (Nâng cao: Bạn nên xóa ảnh cũ trên Cloudinary tại đây)
                        // var deleteResult = await _photoService.DeletePhotoAsync(sanpham.PublicId);

                        // Upload ảnh mới
                        var uploadResult = await _photoService.AddPhotoAsync(hinhAnhFile);
                        if (uploadResult.Error != null)
                        {
                            ModelState.AddModelError("", "Lỗi upload ảnh: " + uploadResult.Error.Message);
                            // Tải lại SelectList nếu lỗi
                            ViewData["MaDm"] = new SelectList(_context.Danhmucs, "MaDm", "Ten", sanpham.MaDm);
                            ViewData["MaTh"] = new SelectList(_context.Thuonghieus, "MaTh", "TenTh", sanpham.MaTh);
                            return View(sanpham);
                        }

                        // Gán URL ảnh mới vào model
                        sanpham.HinhAnh = uploadResult.SecureUrl.ToString();
                    }
                    // Nếu hinhAnhFile là null, sanpham.HinhAnh (từ input ẩn) 
                    // sẽ giữ nguyên giá trị URL cũ, và đó là điều chúng ta muốn.

                    _context.Update(sanpham);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // ... (code xử lý lỗi)
                }
                return RedirectToAction(nameof(Index));
            }

            // Nếu ModelState không hợp lệ, tải lại SelectList
            ViewData["MaDm"] = new SelectList(_context.Danhmucs, "MaDm", "Ten", sanpham.MaDm);
            ViewData["MaTh"] = new SelectList(_context.Thuonghieus, "MaTh", "TenTh", sanpham.MaTh);
            return View(sanpham);
        }

        // GET: Sanphams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanpham = await _context.Sanphams
                .Include(s => s.MaDmNavigation)
                .Include(s => s.MaThNavigation)
                .FirstOrDefaultAsync(m => m.MaSp == id);
            if (sanpham == null)
            {
                return NotFound();
            }

            return View(sanpham);
        }

        // POST: Sanphams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sanpham = await _context.Sanphams.FindAsync(id);
            if (sanpham == null)
            {
                return NotFound();
            }

            // 1. XÓA ẢNH TRÊN CLOUDINARY
            if (!string.IsNullOrEmpty(sanpham.HinhAnh))
            {
                try
                {
                    // Bóc tách PublicId từ URL
                    string publicId = _photoService.GetPublicIdFromUrl(sanpham.HinhAnh);

                    if (!string.IsNullOrEmpty(publicId))
                    {
                        await _photoService.DeletePhotoAsync(publicId);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi xóa ảnh Cloudinary: {ex.Message}");
                }
            }

            // 2. XÓA SẢN PHẨM TRONG CSDL
            _context.Sanphams.Remove(sanpham);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool SanphamExists(int id)
        {
            return _context.Sanphams.Any(e => e.MaSp == id);
        }
    }
}
