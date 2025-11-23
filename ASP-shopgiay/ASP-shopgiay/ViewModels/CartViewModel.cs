using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
namespace ASP_shopgiay.ViewModels
{
    // 1. Class đại diện cho MỘT dòng sản phẩm trong giỏ
    public class CartItemViewModel
    {
        public int MaGioHang { get; set; }
        public int MaBienThe { get; set; }
        public string TenSanpham { get; set; } = string.Empty;
        public string TenThuongHieu { get; set; } = string.Empty;
        public string TenMau { get; set; } = string.Empty;
        public string TenKichThuoc { get; set; } = string.Empty;
        public string HinhAnh { get; set; } = string.Empty;

        public int SoLuong { get; set; } // Số lượng khách mua

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal DonGia { get; set; } // Giá bán tại thời điểm hiện tại

        public int SoLuongTon { get; set; } // Để giới hạn số lượng mua tối đa

        // Tính thành tiền cho dòng này (Số lượng * Đơn giá)
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal ThanhTien => SoLuong * DonGia;
    }
    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();

        // Tính tổng tiền tạm tính của cả giỏ
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal TamTinh => Items.Sum(item => item.ThanhTien);
    }
}
