namespace ASP_shopgiay.Models
{
    public class DanhMucMenuVM
    {
        // MaDM tương ứng với MaDM trong Danhmuc.cs
        public int MaDM { get; set; }

        // Ten tương ứng với Ten trong Danhmuc.cs
        public string Ten { get; set; }

        // Danh sách các Danh mục con thuộc danh mục này
        public List<DanhMucMenuVM> DanhMucCon { get; set; } = new List<DanhMucMenuVM>();
    }
}
