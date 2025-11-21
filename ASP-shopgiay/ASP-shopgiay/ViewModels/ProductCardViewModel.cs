using System.ComponentModel.DataAnnotations;
namespace ASP_shopgiay.ViewModels
{
    public class ProductCardViewModel
    {
        public int MaSP { get; set; }
        public string TenSP { get; set; } = string.Empty;
        public string HinhAnh { get; set; } = string.Empty;
        public string TenThuongHieu { get; set; } = string.Empty;

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal GiaThapNhat { get; set; } // Giá bán thấp nhất của các Biến Thể

        // Thêm trường Rating nếu cần thiết (Tính từ DANHGIA)
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}
