using ASP_shopgiay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASP_shopgiay.Models;
using System.Linq;
using ASP_shopgiay.Data;

// Đảm bảo namespace chính xác
namespace ShopGiay.ViewComponents
{
    public class MegaMenuViewComponent : ViewComponent
    {
        // Thay thế bằng DbContext thực tế của bạn
        private readonly ApplicationDbContext _context;

        // Khởi tạo Dependency Injection
        public MegaMenuViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // 1. Lấy tất cả danh mục đang hoạt động (TrangThai = 1)
            var allDms = await _context.Danhmucs
                                        .Where(dm => dm.TrangThai == true)
                                        .OrderBy(dm => dm.ThuTu)
                                        .ToListAsync();

            // 2. Lọc ra Danh mục Cha (ParentID == NULL) và ánh xạ sang ViewModel
            var parentDms = allDms
                .Where(dm => dm.ParentId == null)
                .Select(dm => new DanhMucMenuVM
                {
                    MaDM = dm.MaDm,
                    Ten = dm.Ten
                }).ToList();

            // 3. Ghép Danh mục Con vào Danh mục Cha tương ứng
            foreach (var parent in parentDms)
            {
                parent.DanhMucCon = allDms
                    .Where(dm => dm.ParentId == parent.MaDM)
                    .Select(dm => new DanhMucMenuVM
                    {
                        MaDM = dm.MaDm,
                        Ten = dm.Ten
                    })
                    .ToList();
            }

            // Trả về ViewModel đã tổ chức
            return View(parentDms);
        }
    }
}

// Lưu ý: Bạn cần thay thế 'ShopGiayContext' bằng tên DbContext thực tế
// và đảm bảo các tên thuộc tính (MaDM, Ten, ParentID, TrangThai, ThuTu)
// trong Danhmuc.cs khớp với code trên.