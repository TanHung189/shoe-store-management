using ASP_shopgiay.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Đăng ký dịch vụ DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseSqlServer(connectionString));
// Add services to the container.

// Đọc cấu hình Cloudinary từ appsettings.json
builder.Services.Configure<ASP_shopgiay.Helpers.CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

// Đăng ký dịch vụ PhotoService
builder.Services.AddScoped<ASP_shopgiay.Services.IPhotoService, ASP_shopgiay.Services.PhotoService>();

// 3. ĐĂNG KÝ DỊCH VỤ XÁC THỰC (AUTHENTICATION)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Đường dẫn đến trang đăng nhập
        options.AccessDeniedPath = "/Account/AccessDenied"; // Trang cấm truy cập
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Thời gian cookie hết hạn
    });

builder.Services.AddHttpContextAccessor();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Kích hoạt xác thực
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
