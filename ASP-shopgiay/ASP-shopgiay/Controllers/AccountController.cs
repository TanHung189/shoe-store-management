using System.Security.Claims;
using ASP_shopgiay.Data;
using ASP_shopgiay.Models;
using ASP_shopgiay.ViewModels;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP_shopgiay.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        // "Tiêm" DbContext vào Controller qua Constructor
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        //================================
        // ĐĂNG KÝ (REGISTER)
        //================================

        // Action (GET) hiển thị form đăng ký
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Action (POST) xử lý dữ liệu từ form đăng ký
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem Email đã tồn tại chưa
                var existingUser = await _context.Taikhoans.FirstOrDefaultAsync(tk => tk.Email == model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng");
                    return View(model);
                }

                // Tạo đối tượng Taikhoan mới từ ViewModel
                var newUser = new Taikhoan
                {
                    Ten = model.Ten,
                    Email = model.Email,
                    DienThoai = model.DienThoai,
                    // Băm mật khẩu trước khi lưu
                    MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhau),
                    TrangThai = true, // Mặc định là active
                    MaCv = null // <--- ĐÂY LÀ KHÁCH HÀNG (MaCV = NULL)
                };

                try
                {
                    // Thêm vào CSDL và lưu thay đổi
                    _context.Taikhoans.Add(newUser);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // Xử lý nếu có lỗi từ CSDL (ví dụ: lỗi UNIQUE...)
                    ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi đăng ký. " + ex.Message);
                    return View(model);
                }

                // Đăng ký thành công, chuyển hướng đến trang Đăng nhập
                return RedirectToAction("Login", "Account");
            }

            // Nếu ModelState không hợp lệ, hiển thị lại form với lỗi
            return View(model);
        }

        // Action (GET) hiển thị form đăng nhập
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // ĐĂNG NHẬP (LOGIN) - PHẦN XỬ LÝ (POST)
        //================================

        // === PHẦN BẠN BỊ THIẾU BẮT ĐẦU TỪ ĐÂY ===
        // Action (POST) xử lý đăng nhập
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Tìm tài khoản
                var user = await _context.Taikhoans.FirstOrDefaultAsync(tk => tk.Email == model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Tài khoản hoặc mật khẩu không đúng");
                    return View(model);
                }

                // 2. Kiểm tra mật khẩu
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.MatKhau, user.MatKhau);
                if (!isPasswordValid)
                {
                    ModelState.AddModelError(string.Empty, "Tài khoản hoặc mật khẩu không đúng");
                    return View(model);
                }

                // 3. Kiểm tra trạng thái
                if (user.TrangThai == false)
                {
                    ModelState.AddModelError(string.Empty, "Tài khoản này đã bị khóa");
                    return View(model);
                }

                // 4. TẠO PHIÊN ĐĂNG NHẬP (Tạo Cookies)
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Ten),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("MaTK", user.MaTk.ToString()),
            new Claim(ClaimTypes.Role, user.MaCv?.ToString() ?? "Customer")
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties { IsPersistent = model.RememberMe };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // 5. ĐIỀU HƯỚNG THEO VAI TRÒ
                if (user.MaCv == 1) // 1 là Admin
                {
                    return RedirectToAction("Index", "Home", new { Area = "Admin" });
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model);
        }
        // === HẾT PHẦN [HttpPost] Login ===


        //================================
        // ĐĂNG XUẤT (LOGOUT)
        //================================

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
