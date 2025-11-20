using ASP_shopgiay.Data;
using ASP_shopgiay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASP_shopgiay.Helpers;
using Microsoft.Extensions.Configuration.UserSecrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASP_shopgiay.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TaiKhoanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaiKhoanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/TaiKhoan
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Taikhoans.Include(t => t.MaCvNavigation);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/TaiKhoan/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taikhoan = await _context.Taikhoans
                .Include(t => t.MaCvNavigation)
                .FirstOrDefaultAsync(m => m.MaTk == id);
            if (taikhoan == null)
            {
                return NotFound();
            }

            return View(taikhoan);
        }

        // GET: Admin/TaiKhoan/Create
        public IActionResult Create()
        {
            ViewData["MaCv"] = new SelectList(_context.Chucvus, "MaCv", "MaCv");
            return View();
        }

        // POST: Admin/TaiKhoan/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaTk,Ten,DienThoai,Email,MatKhau,TrangThai,MaCv")] Taikhoan taikhoan)
        {
            if (ModelState.IsValid)
            {
                // Mã hóa trước khi lưu
                taikhoan.MatKhau = HashPass.ToBCrypt(taikhoan.MatKhau);

                _context.Add(taikhoan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaCv"] = new SelectList(_context.Chucvus, "MaCv", "MaCv", taikhoan.MaCv);
            return View(taikhoan);
        }

        // GET: Admin/TaiKhoan/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taikhoan = await _context.Taikhoans.FindAsync(id);
            if (taikhoan == null)
            {
                return NotFound();
            }
            ViewData["MaCv"] = new SelectList(_context.Chucvus, "MaCv", "MaCv", taikhoan.MaCv);
            return View(taikhoan);
        }

        // POST: Admin/TaiKhoan/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaTk,Ten,DienThoai,Email,MatKhau,TrangThai,MaCv")] Taikhoan taikhoan)
        {
            if (id != taikhoan.MaTk)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taikhoan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaikhoanExists(taikhoan.MaTk))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaCv"] = new SelectList(_context.Chucvus, "MaCv", "MaCv", taikhoan.MaCv);
            return View(taikhoan);
        }

        // GET: Admin/TaiKhoan/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taikhoan = await _context.Taikhoans
                .Include(t => t.MaCvNavigation)
                .FirstOrDefaultAsync(m => m.MaTk == id);
            if (taikhoan == null)
            {
                return NotFound();
            }

            return View(taikhoan);
        }

        // POST: Admin/TaiKhoan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taikhoan = await _context.Taikhoans.FindAsync(id);
            if (taikhoan != null)
            {
                _context.Taikhoans.Remove(taikhoan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaikhoanExists(int id)
        {
            return _context.Taikhoans.Any(e => e.MaTk == id);
        }
    }
}
