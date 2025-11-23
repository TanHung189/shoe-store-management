using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ASP_shopgiay.ViewModels; // Để dùng CartItemViewModel
namespace ASP_shopgiay.ViewModels
{
    public class CheckoutViewModel
    {
        // --- Thông tin người nhận (Form nhập liệu) ---
        [Required(ErrorMessage = "Vui lòng nhập họ tên người nhận")]
        [Display(Name = "Họ tên người nhận")]
        public string TenNguoiNhan { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
        [Display(Name = "Địa chỉ nhận hàng")]
        public string DiaChiGiaoHang { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Display(Name = "Số điện thoại")]
        [RegularExpression(@"^0\d{9,10}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string DienThoaiGiaoHang { get; set; } = null!;

        [Display(Name = "Ghi chú đơn hàng")]
        public string? GhiChu { get; set; }

        // Phương thức thanh toán (Mặc định 1 = COD)
        [Required]
        public int MaPt { get; set; } = 1;

        // --- Dữ liệu hiển thị (Chỉ đọc - lấy từ Giỏ hàng sang) ---
        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal TongTien { get; set; }
    }
}
