using Microsoft.AspNetCore.Mvc;
namespace ASP_shopgiay.Areas.Admin.Controllers
{
    public class HomeController : BaseAdminController
    {
        // Action này sẽ xử lý đường dẫn /Admin/Home/Index
        public IActionResult Index()
        {
            // Trả về View có tên là Index.cshtml
            // Hệ thống sẽ tự tìm nó trong /Areas/Admin/Views/Home/Index.cshtml
            return View();
        }
    }
}
