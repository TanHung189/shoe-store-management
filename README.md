# 👟 Quản Lý Cửa Hàng Giày (Shoe Store Management)

![.NET Core](https://img.shields.io/badge/.NET%20Core-8.0-purple?style=for-the-badge&logo=dotnet)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5-blue?style=for-the-badge&logo=bootstrap)
![SQL Server](https://img.shields.io/badge/SQL%20Server-latest-red?style=for-the-badge&logo=microsoft-sql-server)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)

> **Báo cáo Đồ án Môn học: Quản lý Dự án Phần mềm**
>
> 👨‍🏫 **Giảng viên hướng dẫn:** TS. Nguyễn Văn Hòa
>
> 👥 **Nhóm thực hiện:** Nhóm 15

## 📝 Mô tả Dự án

Đây là hệ thống quản lý toàn diện dành cho cửa hàng bán giày, được xây dựng trên nền tảng **ASP.NET Core 8 MVC**. Mục tiêu của dự án là số hóa và tối ưu hóa các quy trình nghiệp vụ cốt lõi, từ quản lý kho hàng, đơn đặt hàng, thông tin khách hàng đến báo cáo doanh thu, giúp chủ cửa hàng dễ dàng kiểm soát hoạt động kinh doanh một cách hiệu quả và chính xác.

## 👥 Thành viên

| STT | Họ và Tên | MSSV |
|:---:|:---|:---|
| 1 | Bùi Đỗ Tấn Hưng | DTH225658 | 
| 2 | Nguyễn Thành Luận | DTH225690 | 
| 3 | Võ Xuân Phát | DTH225722 | 
| 4 | Nguyễn Phi Long | DPM225438 | 

---

## 🛠️ Công nghệ Sử dụng (Tech Stack)

Dự án được phát triển dựa trên hệ sinh thái Microsoft .NET hiện đại:

| Lĩnh vực | Công nghệ |
| :--- | :--- |
| **Backend Framework** | ![C#](https://img.shields.io/badge/C%23-239120?style=flat-square&logo=c-sharp&logoColor=white) **ASP.NET Core 8.0 (MVC)** |
| **Database** | ![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=flat-square&logo=microsoft-sql-server&logoColor=white) **SQL Server** + Entity Framework Core |
| **Frontend** | ![Bootstrap](https://img.shields.io/badge/Bootstrap-563D7C?style=flat-square&logo=bootstrap&logoColor=white) HTML5, CSS3, JS, Bootstrap 5 |
| **Version Control** | ![Git](https://img.shields.io/badge/Git-F05032?style=flat-square&logo=git&logoColor=white) Git & GitHub |

---

## ✨ Các Tính năng Chính

Hệ thống cung cấp các chức năng quản lý cốt lõi (CRUD) để vận hành cửa hàng:

### 1. Quản lý Sản phẩm (Giày)
* Tạo, chỉnh sửa, xem chi tiết sản phẩm (tên, thương hiệu, màu sắc, kích cỡ, giá bán).
* Quản lý hình ảnh và mô tả sản phẩm.

### 2. Quản lý Kho hàng
* Theo dõi số lượng tồn kho theo từng mẫu giày và kích cỡ.
* Nhập/Xuất kho và điều chỉnh tồn kho.
* Thiết lập và cảnh báo tồn kho tối thiểu.

### 3. Quản lý Đơn hàng
* Tạo, xử lý, và theo dõi trạng thái đơn hàng (Mới, Đang giao, Đã hoàn thành, Đã hủy).
* Tính toán tổng tiền, chiết khấu.

### 4. Quản lý Khách hàng & Nhà cung cấp
* Lưu trữ thông tin chi tiết khách hàng và lịch sử mua hàng.
* Quản lý thông tin các nhà cung cấp.

### 5. Quản lý Người dùng & Phân quyền
* Hệ thống đăng nhập/đăng ký an toàn (sử dụng ASP.NET Identity).
* Phân quyền người dùng theo vai trò (Admin, Nhân viên bán hàng, Thủ kho).

### 6. Báo cáo & Thống kê
* Báo cáo doanh thu chi tiết theo các mốc thời gian (ngày, tháng, quý).
* Thống kê các sản phẩm bán chạy nhất để hỗ trợ ra quyết định kinh doanh.

---

## 🚀 Cài đặt và Chạy Dự án

### Yêu cầu Tiên quyết (Prerequisites)

* **.NET SDK 8.0** hoặc mới hơn.
* **Visual Studio 2022** (hoặc VS Code với C# Extension).
* **SQL Server** (hoặc SQL Server LocalDB).

### Các bước Thiết lập

1.  **Clone Repository:**
    ```bash
    git clone [https://github.com/Ten-cua-ban/ten-repo-cua-ban.git](https://github.com/Ten-cua-ban/ten-repo-cua-ban.git)
    cd ten-repo-cua-ban
    ```

2.  **Cấu hình Cơ sở Dữ liệu:**
    * Mở tệp `appsettings.json` và cập nhật chuỗi kết nối (Connection String) nếu cần.
    * Thực hiện Migrations để tạo hoặc cập nhật cấu trúc DB:
        ```bash
        dotnet ef database update
        ```

3.  **Chạy Ứng dụng:**
    * Mở Terminal trong thư mục gốc của dự án và chạy:
        ```bash
        dotnet run
        ```
    * Hoặc chạy trực tiếp bằng Visual Studio (Nhấn `F5`).

4.  **Truy cập:**
    * Ứng dụng sẽ mở trên trình duyệt tại địa chỉ mặc định (ví dụ: `https://localhost:7001`).

---

## 👨‍💻 Đóng góp (Contributing)

Đóng góp của bạn là vô cùng quý giá. Để đóng góp vào dự án, vui lòng thực hiện theo các bước sau:

1.  **Fork** Repository này.
2.  Tạo một nhánh mới (`git checkout -b feature/tinh-nang-moi`).
3.  Commit các thay đổi của bạn (`git commit -m 'Thêm tính năng mới: ...'`).
4.  Đẩy lên nhánh vừa tạo (`git push origin feature/tinh-nang-moi`).
5.  Mở một **Pull Request** mới.

---

## ⚖️ Giấy phép (License)

Dự án thuộc bản quyền của Nhóm 15 - Lớp học phần Quản lý dự án phần mềm. Mã nguồn được chia sẻ dưới giấy phép MIT.
