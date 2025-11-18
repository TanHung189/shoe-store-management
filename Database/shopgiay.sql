
CREATE DATABASE ShopGiay;
GO
USE ShopGiay;
GO

-- =====================================
-- 1. BẢNG TRA CỨU (KHÔNG THAY ĐỔI)
-- =====================================

CREATE TABLE DANHMUC (
    MaDM INT IDENTITY(1,1) PRIMARY KEY,
    Ten NVARCHAR(100) NOT NULL,
    ParentID INT NULL,        -- NULL = danh mục cha, có giá trị = con
    ThuTu INT DEFAULT 0,      -- Thứ tự hiển thị trên menu
    TrangThai BIT DEFAULT 1,  -- 1 = hiển thị, 0 = ẩn
    CONSTRAINT FK_DANHMUC_PARENT FOREIGN KEY (ParentID) REFERENCES DANHMUC(MaDM)
);
GO

CREATE TABLE CHUCVU (
    MaCV INT IDENTITY(1,1) PRIMARY KEY,
    Ten NVARCHAR(100) NOT NULL,
    HeSo FLOAT NULL
);

CREATE TABLE TRANGTHAI_DONHANG (
    MaTrangThai INT IDENTITY(1,1) PRIMARY KEY,
    TenTrangThai NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(255)
);

CREATE TABLE MAGIAMGIA (
    MaGiamGia INT IDENTITY(1,1) PRIMARY KEY,
    Code VARCHAR(50) UNIQUE NOT NULL,
    SoTienGiam INT DEFAULT 0,
    PhanTramGiam FLOAT DEFAULT 0,
    GiamToiDa INT NULL,
    SoLuong INT DEFAULT 1,
    NgayBatDau DATETIME DEFAULT GETDATE(),
    NgayKetThuc DATETIME NOT NULL
);

CREATE TABLE PHUONGTHUCTHANHTOAN (
    MaPT INT IDENTITY(1,1) PRIMARY KEY,
    TenPT NVARCHAR(100) UNIQUE NOT NULL,
    MoTa NVARCHAR(255),
    TrangThai BIT DEFAULT 1
);

CREATE TABLE THUONGHIEU (
    MaTH INT IDENTITY(1,1) PRIMARY KEY,
    TenTH NVARCHAR(100) UNIQUE NOT NULL,
    Logo VARCHAR(255)
);

CREATE TABLE MAUSAC (
    MaMau INT IDENTITY(1,1) PRIMARY KEY,
    TenMau NVARCHAR(50) UNIQUE NOT NULL,
    MaHex VARCHAR(7)
);

CREATE TABLE KICHTHUOC (
    MaKichThuoc INT IDENTITY(1,1) PRIMARY KEY,
    TenKichThuoc VARCHAR(20) UNIQUE NOT NULL
);

-- =====================================
-- 2. BẢNG TÀI KHOẢN HỢP NHẤT (ĐÃ THAY ĐỔI)
-- (Gộp TAIKHOAN, KHACHHANG, NHANVIEN làm một)
-- =====================================

CREATE TABLE TAIKHOAN (
    MaTK INT IDENTITY(1,1) PRIMARY KEY,
    
    -- Thông tin hồ sơ (gộp từ KHACHHANG và NHANVIEN)
    Ten NVARCHAR(100) NOT NULL, 
    DienThoai VARCHAR(20) NULL,
    
    -- Thông tin đăng nhập
    Email VARCHAR(100) NOT NULL UNIQUE,
    MatKhau NVARCHAR(256) NOT NULL, -- Nhớ băm mật khẩu
    TrangThai BIT DEFAULT 1,
    
    -- Thông tin phân quyền (thay cho LoaiTK)
    MaCV INT NULL, -- Sẽ là NULL nếu đây là Khách hàng
    
    CONSTRAINT FK_TAIKHOAN_CHUCVU FOREIGN KEY (MaCV) REFERENCES CHUCVU(MaCV)
);
GO

-- =====================================
-- 3. SẢN PHẨM VÀ BIẾN THỂ (KHÔNG THAY ĐỔI)
-- =====================================

CREATE TABLE SANPHAM (
    MaSP INT IDENTITY(1,1) PRIMARY KEY,
    TenSP NVARCHAR(255) NOT NULL,
    MaDM INT NOT NULL,
    MaTH INT NOT NULL,
    MoTa NVARCHAR(MAX),
    HinhAnh VARCHAR(255),
    LuotXem INT DEFAULT 0,
    LuotMua INT DEFAULT 0,
    CONSTRAINT FK_SP_DM FOREIGN KEY (MaDM) REFERENCES DANHMUC(MaDM),
    CONSTRAINT FK_SP_TH FOREIGN KEY (MaTH) REFERENCES THUONGHIEU(MaTH)
);

CREATE TABLE BIENTHE_SANPHAM (
    MaBienThe INT IDENTITY(1,1) PRIMARY KEY,
    MaSP INT NOT NULL,
    MaMau INT NOT NULL,
    MaKichThuoc INT NOT NULL,
    GiaBan DECIMAL(18,2) NOT NULL,
    SoLuong INT DEFAULT 0,
    HinhAnh VARCHAR(255),
    CONSTRAINT FK_BT_SP FOREIGN KEY (MaSP) REFERENCES SANPHAM(MaSP) ON DELETE CASCADE,
    CONSTRAINT FK_BT_MAU FOREIGN KEY (MaMau) REFERENCES MAUSAC(MaMau),
    CONSTRAINT FK_BT_SIZE FOREIGN KEY (MaKichThuoc) REFERENCES KICHTHUOC(MaKichThuoc),
    CONSTRAINT UK_BT_SP_MAU_SIZE UNIQUE(MaSP, MaMau, MaKichThuoc)
);

-- =====================================
-- 4. BẢNG NGHIỆP VỤ (ĐÃ THAY ĐỔI)
-- (Tất cả các bảng nghiệp vụ giờ sẽ liên kết với MaTK)
-- =====================================

-- THAY ĐỔI: Dùng MaTK thay cho MaKH
CREATE TABLE DIACHI (
    MaDC INT IDENTITY(1,1) PRIMARY KEY,
    MaTK INT NOT NULL, 
    TenDiaChi NVARCHAR(255) NOT NULL,
    DiaChi NVARCHAR(255) NOT NULL,
    PhuongXa NVARCHAR(100) NOT NULL,
    QuanHuyen NVARCHAR(100) NOT NULL,
    TinhThanh NVARCHAR(100) NOT NULL,
    MacDinh BIT DEFAULT 0,
    CONSTRAINT FK_DIACHI_TAIKHOAN FOREIGN KEY (MaTK) REFERENCES TAIKHOAN(MaTK) ON DELETE CASCADE
);

-- THAY ĐỔI: Dùng MaTK thay cho MaKH
CREATE TABLE GIOHANG (
    MaGioHang INT IDENTITY(1,1) PRIMARY KEY,
    MaTK INT NOT NULL,
    MaBienThe INT NOT NULL,
    SoLuong INT DEFAULT 1,
    NgayThem DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_GH_TAIKHOAN FOREIGN KEY (MaTK) REFERENCES TAIKHOAN(MaTK) ON DELETE CASCADE,
    CONSTRAINT FK_GH_BT FOREIGN KEY (MaBienThe) REFERENCES BIENTHE_SANPHAM(MaBienThe),
    CONSTRAINT UK_GH_TK_BT UNIQUE(MaTK, MaBienThe)
);

-- THAY ĐỔI: Dùng MaTK thay cho MaKH
CREATE TABLE DANHSACH_YEU_THICH (
    MaYeuThich INT IDENTITY(1,1) PRIMARY KEY,
    MaTK INT NOT NULL,
    MaBienThe INT NOT NULL,
    NgayThem DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_YT_TAIKHOAN FOREIGN KEY (MaTK) REFERENCES TAIKHOAN(MaTK) ON DELETE CASCADE,
    CONSTRAINT FK_YT_BT FOREIGN KEY (MaBienThe) REFERENCES BIENTHE_SANPHAM(MaBienThe),
    CONSTRAINT UK_YT_TK_BT UNIQUE(MaTK, MaBienThe)
);

-- THAY ĐỔI: Dùng MaTK thay cho MaKH
CREATE TABLE DANHGIA (
    MaDanhGia INT IDENTITY(1,1) PRIMARY KEY,
    MaTK INT NOT NULL,
    MaSP INT NOT NULL,
    SoSao TINYINT NOT NULL,
    BinhLuan NVARCHAR(1000),
    DaDuyet BIT DEFAULT 0,
    NgayDanhGia DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_DG_TAIKHOAN FOREIGN KEY (MaTK) REFERENCES TAIKHOAN(MaTK),
    CONSTRAINT FK_DG_SP FOREIGN KEY (MaSP) REFERENCES SANPHAM(MaSP)
);

-- THAY ĐỔI: Dùng MaTK thay cho MaKH
CREATE TABLE HOADON (
    MaHD INT IDENTITY(1,1) PRIMARY KEY,
    MaTK INT NOT NULL,
    Ngay DATETIME DEFAULT GETDATE(),
    DiaChiGiaoHang NVARCHAR(255) NOT NULL,
    DienThoaiGiaoHang VARCHAR(20) NOT NULL,
    TenNguoiNhan NVARCHAR(100) NOT NULL,
    MaTrangThai INT NOT NULL,
    MaGiamGia INT NULL,
    MaPT INT NOT NULL,
    TamTinh DECIMAL(18,2) DEFAULT 0,
    PhiVanChuyen DECIMAL(18,2) DEFAULT 0,
    GiamGia DECIMAL(18,2) DEFAULT 0,
    TongTien DECIMAL(18,2) DEFAULT 0,
    CONSTRAINT FK_HD_TAIKHOAN FOREIGN KEY (MaTK) REFERENCES TAIKHOAN(MaTK),
    CONSTRAINT FK_HD_TT FOREIGN KEY (MaTrangThai) REFERENCES TRANGTHAI_DONHANG(MaTrangThai),
    CONSTRAINT FK_HD_GG FOREIGN KEY (MaGiamGia) REFERENCES MAGIAMGIA(MaGiamGia),
    CONSTRAINT FK_HD_PT FOREIGN KEY (MaPT) REFERENCES PHUONGTHUCTHANHTOAN(MaPT)
);

-- Bảng CTHOADON (Không thay đổi, vẫn liên kết với MaHD)
CREATE TABLE CTHOADON (
    MaCTHD INT IDENTITY(1,1) PRIMARY KEY,
    MaHD INT NOT NULL,
    MaBienThe INT NOT NULL,
    SoLuong INT NOT NULL,
    DonGia DECIMAL(18,2) NOT NULL,
    GiamGiaCT DECIMAL(18,2) DEFAULT 0,
    ThanhTienGoc AS (SoLuong * DonGia) PERSISTED,
    CONSTRAINT FK_CTHD_HD FOREIGN KEY (MaHD) REFERENCES HOADON(MaHD) ON DELETE CASCADE,
    CONSTRAINT FK_CTHD_BT FOREIGN KEY (MaBienThe) REFERENCES BIENTHE_SANPHAM(MaBienThe)
);
GO

-- =====================================
-- 5. DỮ LIỆU MẪU CƠ BẢN (ĐÃ THAY ĐỔI)
-- =====================================

-- Chèn dữ liệu Chức Vụ
INSERT INTO CHUCVU (Ten, HeSo)
VALUES (N'Quản trị viên', 2.0);
GO

-- Chèn dữ liệu cho TAIKHOAN (gộp)
-- (Lưu ý: Bạn phải tự băm mật khẩu '123456' trong code, ở đây chỉ là ví dụ)
INSERT INTO TAIKHOAN (Ten, DienThoai, Email, MatKhau, MaCV)
VALUES 
-- Tài khoản Admin (MaTK=1, MaCV=1)
(N'Võ Xuân Phát', '0373204635', 'admin@gmail.com', '$2y$10$8SmpVo6ZqKIjuRWWI56ngOyE0tDkrjsxY9i2UtFJgmhLQ9kAoR4.e', 1), -- 123456

-- Tài khoản Khách hàng (MaTK=2, MaCV=NULL)
(N'Bùi Đỗ Tấn Hưng', '0909000003', 'hung@gmail.com', '$2y$10$Imb2mwnmODQtPw2jFTQwGOEc8gh2U4A2uH2RNvJc00bBIThj4vekm', NULL); --hung123
GO

-- Danh mục cha
INSERT INTO DANHMUC (Ten, ParentID, ThuTu, TrangThai) VALUES
(N'Giày thể thao', NULL, 1, 1),
(N'Giày thời trang', NULL, 2, 1),
(N'Phụ kiện giày', NULL, 3, 1);

-- Danh mục con Giày thể thao
INSERT INTO DANHMUC (Ten, ParentID, ThuTu, TrangThai) VALUES
(N'Nam', 1, 1, 1),
(N'Nữ', 1, 2, 1);

-- Danh mục con Giày thời trang
INSERT INTO DANHMUC (Ten, ParentID, ThuTu, TrangThai) VALUES
(N'Sneakers', 2, 1, 1),
(N'Giày da', 2, 2, 1);

-- Danh mục con Phụ kiện giày
INSERT INTO DANHMUC (Ten, ParentID, ThuTu, TrangThai) VALUES
(N'Tất', 3, 1, 1),
(N'Dây giày', 3, 2, 1);
GO

-- Chèn dữ liệu mẫu cho TRANGTHAI_DONHANG [cite: 24]
INSERT INTO TRANGTHAI_DONHANG (TenTrangThai, MoTa) VALUES
(N'Chờ xác nhận', N'Đơn hàng mới, đang chờ shop xem và xác nhận'),
(N'Đã xác nhận', N'Shop đã xác nhận, đang chờ đóng gói'),
(N'Đang giao hàng', N'Đơn hàng đã được bàn giao cho đơn vị vận chuyển'),
(N'Hoàn thành', N'Khách hàng đã nhận được hàng'),
(N'Đã hủy', N'Đơn hàng đã bị hủy');
GO

-- Chèn dữ liệu mẫu cho PHUONGTHUCTHANHTOAN (Đã tối ưu)
INSERT INTO PHUONGTHUCTHANHTOAN (TenPT, MoTa) VALUES
(N'Thanh toán khi nhận hàng (COD)', N'Khách hàng trả tiền mặt khi nhận hàng'),
(N'Chuyển khoản ngân hàng', N'Khách hàng chuyển khoản trước'),
(N'Thanh toán qua ví điện tử', N'Thanh toán qua Momo, ZaloPay...');
GO

-- Chèn dữ liệu mẫu cho THUONGHIEU (Đã tối ưu)
INSERT INTO THUONGHIEU (TenTH) VALUES
(N'Nike'),
(N'Adidas'),
(N'Bitis'),
(N'Puma');
GO

-- Chèn dữ liệu mẫu cho MAUSAC (Đã tối ưu)
INSERT INTO MAUSAC (TenMau, MaHex) VALUES
(N'Trắng', '#FFFFFF'),
(N'Đen', '#000000'),
(N'Đỏ', '#FF0000'),
(N'Xanh Navy', '#000080'),
(N'Xám', '#808080'),
(N'Be', '#F5F5DC'),
(N'Nâu', '#8B4513'),
(N'Hồng', '#FFC0CB'), -- MaMau = 8
(N'Cam', '#FFA500');  -- MaMau = 9
GO


-- Chèn dữ liệu mẫu cho KICHTHUOC
INSERT INTO KICHTHUOC (TenKichThuoc) VALUES
('38'), ('38.5'), ('39'), ('39.5'), ('40'), ('40.5'), ('41'), ('41.5'), ('42'), ('42.5'), ('43'), (N'Không áp dụng');
GO

-- SANPHAM thương hiệu Nike
INSERT INTO SANPHAM (TenSP, MaDM, MaTH, MoTa, HinhAnh, LuotMua)
VALUES
(N'Nike Air Force 1 \''07', 4, 1, N'Huyền thoại trong làng sneaker, Nike Air Force 1 ''07 mang đến sự thoải mái và phong cách cổ điển.', '/images/products/nike-af1-white.jpg', 15);

UPDATE SANPHAM
SET HinhAnh = 'nike-af1-white.jpg' -- Thay đổi giá trị cột HinhAnh
WHERE TenSP = N'Nike Air Force 1 \''07'; -- Điều kiện tìm kiếm sản phẩm
select * from SANPHAM
-- BIENTHE_SANPHAM
INSERT INTO BIENTHE_SANPHAM (MaSP, MaMau, MaKichThuoc, GiaBan, SoLuong, HinhAnh)
VALUES
(1, 1, 3, 2800000, 20, '/images/products/nike-af1-white.jpg'),  --(Màu trắng, size 39)
(1, 1, 5, 2800000, 35, '/images/products/nike-af1-white.jpg'),
(1, 4, 2, 2900000, 30, '/images/products/nike-af1-Navy.jpg'),
(1, 4, 3, 2900000, 30, '/images/products/nike-af1-Navy.jpg');

INSERT INTO SANPHAM (TenSP, MaDM, MaTH, MoTa, HinhAnh, LuotMua)
VALUES
(N'Nike Air Max 270', 4, 1, N'Sneaker Air Max 270 với thiết kế êm ái, hiện đại.', '/images/products/nike-airmax270.jpg', 12);

UPDATE SANPHAM
SET HinhAnh = 'nike-airmax270.jpg' -- Thay đổi giá trị cột HinhAnh
WHERE TenSP = N'Nike Air Max 270'; -- Điều kiện tìm kiếm sản phẩm
select * from SANPHAM
INSERT INTO BIENTHE_SANPHAM (MaSP, MaMau, MaKichThuoc, GiaBan, SoLuong, HinhAnh)
VALUES
(2, 1, 3, 4490000, 40, '/images/products/nike-airmax270.jpg'),
(2, 2, 4, 4190000, 35, '/images/products/nike-airmax270-black.jpg'),
(2, 2, 5, 4190000, 35, '/images/products/nike-airmax270-black.jpg'),
(2, 4, 5, 4490000, 25, '/images/products/nike-airmax270-navy.jpg'),
(2, 4, 6, 4490000, 25, '/images/products/nike-airmax270-navy.jpg');

-- Nike Dunk Low
INSERT INTO SANPHAM (TenSP, MaDM, MaTH, MoTa, HinhAnh, LuotMua)
VALUES
(N'Nike Dunk Low', 5, 1, N'Sneaker cổ điển, phối màu đa dạng.', '/images/products/nike-dunklow.jpg', 12);
UPDATE SANPHAM
SET HinhAnh = 'nike-dunklow.jpg' -- Thay đổi giá trị cột HinhAnh
WHERE TenSP = N'Nike Dunk Low'; -- Điều kiện tìm kiếm sản phẩm
select * from SANPHAM

-- Biến thể Nike Dunk Low
INSERT INTO BIENTHE_SANPHAM (MaSP, MaMau, MaKichThuoc, GiaBan, SoLuong, HinhAnh)
VALUES
(3, 1, 2, 2900000, 20, '/images/products/nike-dunklow.jpg'),
(3, 1, 3, 2900000, 20, '/images/products/nike-dunklow.jpg'),
(3, 1, 4, 2900000, 20, '/images/products/nike-dunklow.jpg'),
(3, 3, 4, 2950000, 30, '/images/products/nike-dunklow-red.jpg'),
(3, 3, 5, 2950000, 30, '/images/products/nike-dunklow-red.jpg'),
(3, 3, 6, 2950000, 30, '/images/products/nike-dunklow-red.jpg');


-- Nike React Infinity Run
INSERT INTO SANPHAM (TenSP, MaDM, MaTH, MoTa, HinhAnh, LuotMua)
VALUES
(N'Nike React Infinity Run', 5, 1, N'Giày chạy bộ, giảm chấn tốt.', '/images/products/nike-react-infinity.jpg', 18);

UPDATE SANPHAM
SET HinhAnh = 'nike-react-infinity.jpg' -- Thay đổi giá trị cột HinhAnh
WHERE TenSP = N'Nike React Infinity Run'; -- Điều kiện tìm kiếm sản phẩm
select * from SANPHAM
-- Biến thể Nike React Infinity Run
INSERT INTO BIENTHE_SANPHAM (MaSP, MaMau, MaKichThuoc, GiaBan, SoLuong, HinhAnh)
VALUES
(4, 1, 1, 2479500, 25, '/images/products/nike-react-infinity.jpg'),
(4, 1, 2, 2479500, 25, '/images/products/nike-react-infinity.jpg'),
(4, 2, 4, 2479500, 20, '/images/products/nike-react-black.jpg'),
(4, 2, 5, 2479500, 20, '/images/products/nike-react-black.jpg'),
(4, 2, 6, 2479500, 20, '/images/products/nike-react-black.jpg');


-- 2. THƯƠNG HIỆU: Adidas
-- =====================================

-- Adidas Ultraboost 22
INSERT INTO SANPHAM (TenSP, MaDM, MaTH, MoTa, HinhAnh, LuotMua)
VALUES
(N'Adidas Ultraboost 22', 4, 2, N'Hiệu năng tối đa, đế Boost siêu êm.', '/images/products/adidas-ultraboost22.jpg', 22);

UPDATE SANPHAM
SET HinhAnh = 'adidas-ultraboost22.jpg' -- Thay đổi giá trị cột HinhAnh
WHERE TenSP = N'Adidas Ultraboost 22'; -- Điều kiện tìm kiếm sản phẩm
select * from SANPHAM
INSERT INTO BIENTHE_SANPHAM (MaSP, MaMau, MaKichThuoc, GiaBan, SoLuong, HinhAnh)
VALUES
(5, 8, 3, 5100000, 10, '/images/products/adidas-ultraboost22.jpg'),
(5, 8, 4, 5100000, 10, '/images/products/adidas-ultraboost22.jpg'),
(5, 6, 2, 3550000, 30, '/images/products/adidas-ultraboost-xam.jpg'),
(5, 6, 3, 3550000, 30, '/images/products/adidas-ultraboost-xam.jpg'),
(5, 6, 4, 3550000, 30, '/images/products/adidas-ultraboost-xam.jpg'),
(5, 4, 6, 3550000, 25, '/images/products/adidas-ultraboost-navy.jpg');

-- Adidas Stan Smith
INSERT INTO SANPHAM (TenSP, MaDM, MaTH, MoTa, HinhAnh, LuotMua)
VALUES
(N'Adidas Stan Smith', 6, 2, N'Giày da cổ điển, biểu tượng của thời trang.', '/images/products/adidas-stansmith.jpg', 18);

UPDATE SANPHAM
SET HinhAnh = 'adidas-stansmith.jpg' -- Thay đổi giá trị cột HinhAnh
WHERE TenSP = N'Adidas Stan Smith'; -- Điều kiện tìm kiếm sản phẩm
select * from SANPHAM
INSERT INTO BIENTHE_SANPHAM (MaSP, MaMau, MaKichThuoc, GiaBan, SoLuong, HinhAnh)
VALUES
(6, 1, 1, 2400000, 15, '/images/products/adidas-stansmith.jpg'),
(6, 1, 2, 2400000, 15, '/images/products/adidas-stansmith.jpg'),
(6, 8, 2, 2400000, 30, '/images/products/adidas-stansmith-prink.jpg'),
(6, 8, 3, 2400000, 30, '/images/products/adidas-stansmith-prink.jpg');


-- Adidas Superstar
INSERT INTO SANPHAM (TenSP, MaDM, MaTH, MoTa, HinhAnh, LuotMua)
VALUES
(N'Adidas Superstar', 6, 2, N'Sneaker biểu tượng với mũi vỏ sò.', '/images/products/Adidas Superstar.jpg', 25);

UPDATE SANPHAM
SET HinhAnh = 'Adidas Superstar.jpg' -- Thay đổi giá trị cột HinhAnh
WHERE TenSP = N'Adidas Superstar'; -- Điều kiện tìm kiếm sản phẩm
INSERT INTO BIENTHE_SANPHAM (MaSP, MaMau, MaKichThuoc, GiaBan, SoLuong, HinhAnh)
VALUES
(7, 1, 4, 2900000, 40, '/images/products/Adidas Superstar.jpg'),
(7, 2, 4, 2900000, 40, '/images/products/Adidas Superstar.jpg'),
(7, 3, 4, 2900000, 40, '/images/products/Adidas Superstar.jpg');

-- Adidas NMD_R1
INSERT INTO SANPHAM (TenSP, MaDM, MaTH, MoTa, HinhAnh, LuotMua)
VALUES
(N'Adidas NMD_R1', 5, 2, N'Sự kết hợp giữa công nghệ Boost và thiết kế hiện đại.', '/images/products/adidas-nmdr1.jpg', 19);

UPDATE SANPHAM
SET HinhAnh = 'adidas-nmdr1.jpg' -- Thay đổi giá trị cột HinhAnh
WHERE TenSP = N'Adidas NMD_R1'; -- Điều kiện tìm kiếm sản phẩm
INSERT INTO BIENTHE_SANPHAM (MaSP, MaMau, MaKichThuoc, GiaBan, SoLuong, HinhAnh)
VALUES
(8, 1, 1, 3100000, 30, '/images/products/adidas-nmdr1.jpg'), -- Size 38
(8, 1, 2, 3100000, 30, '/images/products/adidas-nmdr1.jpg'), -- Size 38.5
(8, 1, 3, 3100000, 30, '/images/products/adidas-nmdr1.jpg'); -- Size 39
-- Dòng bị trùng đã được loại bỏ
GO

-- =====================================
-- 3. THƯƠNG HIỆU: Bitis
-- =====================================

-- Bitis Hunter Street
INSERT INTO SANPHAM (TenSP, MaDM, MaTH, MoTa, HinhAnh, LuotMua)
VALUES
(N'Bitis Hunter Street', 4, 3, N'Sự lựa chọn hàng đầu cho giới trẻ năng động.', '/images/products/bitis-hunter-street.jpg', 30);
UPDATE SANPHAM
SET HinhAnh = 'bitis-hunter-street.jpg' -- Thay đổi giá trị cột HinhAnh
WHERE TenSP = N'Bitis Hunter Street'; -- Điều kiện tìm kiếm sản phẩm
INSERT INTO BIENTHE_SANPHAM (MaSP, MaMau, MaKichThuoc, GiaBan, SoLuong, HinhAnh)
VALUES
(9, 1, 3, 950000, 40, '/images/products/bitis-hunter-street.jpg'),
(9, 1, 4, 950000, 40, '/images/products/bitis-hunter-street.jpg'),
(9, 1, 5, 950000, 40, '/images/products/bitis-hunter-street.jpg'),
(9, 5, 1, 950000, 35, '/images/products/bitis-hunter-street-xam.jpg'),
(9, 5, 2, 950000, 35, '/images/products/bitis-hunter-street-xam.jpg'),
(9, 5, 3, 950000, 35, '/images/products/bitis-hunter-street-xam.jpg');

-- Bitis Hunter X
INSERT INTO SANPHAM (TenSP, MaDM, MaTH, MoTa, HinhAnh, LuotMua)
VALUES
(N'Bitis Hunter X', 5, 3, N'Đế phylon êm ái, thiết kế năng động.', '/images/products/bitis-hunterx.jpg', 28);
UPDATE SANPHAM
SET HinhAnh = 'bitis-hunterx.jpg' -- Thay đổi giá trị cột HinhAnh
WHERE TenSP = N'Bitis Hunter X'; -- Điều kiện tìm kiếm sản phẩm
INSERT INTO BIENTHE_SANPHAM (MaSP, MaMau, MaKichThuoc, GiaBan, SoLuong, HinhAnh)
VALUES
(10, 1, 1, 980000, 30, '/images/products/bitis-hunterx.jpg'),
(10, 1, 2, 980000, 30, '/images/products/bitis-hunterx.jpg'),
(10, 1, 3, 980000, 30, '/images/products/bitis-hunterx.jpg'),
(10, 1, 4, 980000, 30, '/images/products/bitis-hunterx.jpg'),
(10, 4, 1, 980000, 25, '/images/products/bitis-hunterx-navy.jpg'),
(10, 4, 2, 980000, 25, '/images/products/bitis-hunterx-navy.jpg'),
(10, 4, 3, 980000, 25, '/images/products/bitis-hunterx-navy.jpg'),
(10, 4, 4, 980000, 25, '/images/products/bitis-hunterx-navy.jpg');

-- Bitis Hunter Core
INSERT INTO SANPHAM (TenSP, MaDM, MaTH, MoTa, HinhAnh, LuotMua)
VALUES
(N'Bitis Hunter Core', 4, 3, N'Sự cân bằng giữa bền bỉ và nhẹ nhàng.', '/images/products/bitis-hunter-core.jpg', 26);
UPDATE SANPHAM
SET HinhAnh = 'bitis-hunter-core.jpg' -- Thay đổi giá trị cột HinhAnh
WHERE TenSP = N'Bitis Hunter Core'; -- Điều kiện tìm kiếm sản phẩm
INSERT INTO BIENTHE_SANPHAM (MaSP, MaMau, MaKichThuoc, GiaBan, SoLuong, HinhAnh)
VALUES
(11, 1, 1, 620000, 35, '/images/products/bitis-hunter-core.jpg'),
(11, 1, 2, 620000, 35, '/images/products/bitis-hunter-core.jpg'),
(11, 1, 3, 620000, 35, '/images/products/bitis-hunter-core.jpg');

-- Bitis Hunter Marvel Edition
INSERT INTO SANPHAM (TenSP, MaDM, MaTH, MoTa, HinhAnh, LuotMua)
VALUES
(N'Bitis Hunter Marvel Edition', 4, 3, N'Phiên bản giới hạn hợp tác cùng Marvel.', '/images/products/bitis-marvel.jpg', 40);
UPDATE SANPHAM
SET HinhAnh = 'bitis-marvel.jpg' -- Thay đổi giá trị cột HinhAnh
WHERE TenSP = N'Bitis Hunter Marvel Edition'; -- Điều kiện tìm kiếm sản phẩm
INSERT INTO BIENTHE_SANPHAM (MaSP, MaMau, MaKichThuoc, GiaBan, SoLuong, HinhAnh)
VALUES
(12, 1, 4, 1050000, 25, '/images/products/bitis-marvel-white.jpg'),
(12, 3, 5, 1050000, 30, '/images/products/bitis-marvel.jpg');

GO

-- =====================================
-- 4. THƯƠNG HIỆU: Puma
-- =====================================

-- Puma Suede Classic
INSERT INTO SANPHAM (TenSP, MaDM, MaTH, MoTa, HinhAnh, LuotMua)
VALUES
(N'Puma Suede Classic', 6, 4, N'Sneaker biểu tượng từ năm 1968, phong cách cổ điển.', '/images/products/puma-suede-classic.jpg', 8);
UPDATE SANPHAM
SET HinhAnh = 'puma-suede-classic.jpg' -- Thay đổi giá trị cột HinhAnh
WHERE TenSP = N'Puma Suede Classic'; -- Điều kiện tìm kiếm sản phẩm
INSERT INTO BIENTHE_SANPHAM (MaSP, MaMau, MaKichThuoc, GiaBan, SoLuong, HinhAnh)
VALUES
(13, 4, 1, 2400000, 25, '/images/products/puma-suede-classic.jpg'),
(13, 4, 2, 2400000, 25, '/images/products/puma-suede-classic.jpg'),
(13, 4, 3, 2400000, 25, '/images/products/puma-suede-classic.jpg'),
(13, 4, 4, 2400000, 25, '/images/products/puma-suede-classic.jpg'),
(13, 9, 1, 2450000, 5, '/images/products/puma-suede-orange.jpg'),
(13, 9, 2, 2450000, 5, '/images/products/puma-suede-orange.jpg'),
(13, 9, 3, 2450000, 5, '/images/products/puma-suede-orange.jpg'),
(13, 9, 4, 2450000, 5, '/images/products/puma-suede-orange.jpg');

-- Puma RS-X
INSERT INTO SANPHAM (TenSP, MaDM, MaTH, MoTa, HinhAnh, LuotMua)
VALUES
(N'Puma RS-X', 5, 4, N'Thiết kế chunky đậm chất hiện đại, cực êm.', '/images/products/puma-rsx.jpg', 2);
UPDATE SANPHAM
SET HinhAnh = 'puma-rsx.jpg' -- Thay đổi giá trị cột HinhAnh
WHERE TenSP = N'Puma RS-X'; -- Điều kiện tìm kiếm sản phẩm
INSERT INTO BIENTHE_SANPHAM (MaSP, MaMau, MaKichThuoc, GiaBan, SoLuong, HinhAnh)
VALUES
(14, 1, 5, 2600000, 30, '/images/products/puma-rsx-white-40.jpg'),
(14, 2, 6, 2650000, 20, '/images/products/puma-rsx-black-40.5.jpg');

-- Puma Future Rider
INSERT INTO SANPHAM (TenSP, MaDM, MaTH, MoTa, HinhAnh, LuotMua)
VALUES
(N'Puma Future Rider', 7, 4, N'Giày retro pha chút hiện đại, cực nhẹ.', '/images/products/puma-futurerider-red.jpg', 16);
UPDATE SANPHAM
SET HinhAnh = 'puma-futurerider-red.jpg' -- Thay đổi giá trị cột HinhAnh
WHERE TenSP = N'Puma Future Rider'; -- Điều kiện tìm kiếm sản phẩm
INSERT INTO BIENTHE_SANPHAM (MaSP, MaMau, MaKichThuoc, GiaBan, SoLuong, HinhAnh)
VALUES
(15, 3, 4, 2550000, 25, '/images/products/puma-futurerider-red.jpg'),
(15, 3, 5, 2550000, 25, '/images/products/puma-futurerider-red.jpg'),
(15, 3, 6, 2550000, 25, '/images/products/puma-futurerider-red.jpg'),
(15, 8, 5, 2550000, 25, '/images/products/puma-futurerider-prink.jpg'),
(15, 8, 6, 2550000, 25, '/images/products/puma-futurerider-prink.jpg'),
(15, 8, 7, 2550000, 25, '/images/products/puma-futurerider-prink.jpg');

-- Puma Cali Dream
INSERT INTO SANPHAM (TenSP, MaDM, MaTH, MoTa, HinhAnh, LuotMua)
VALUES
(N'Puma Cali Dream', 6, 4, N'Phong cách năng động, nữ tính, phù hợp mọi outfit.', '/images/products/puma-calidream-pink.jpg', 19);
UPDATE SANPHAM
SET HinhAnh = 'puma-calidream-pink.jpg' -- Thay đổi giá trị cột HinhAnh
WHERE TenSP = N'Puma Cali Dream'; -- Điều kiện tìm kiếm sản phẩm
INSERT INTO BIENTHE_SANPHAM (MaSP, MaMau, MaKichThuoc, GiaBan, SoLuong, HinhAnh)
VALUES
(16, 8, 1, 2450000, 15, '/images/products/puma-calidream-pink.jpg'),
(16, 8, 2, 2450000, 15, '/images/products/puma-calidream-pink.jpg'),
(16, 8, 3, 2450000, 15, '/images/products/puma-calidream-pink.jpg'),
(16, 8, 4, 2450000, 15, '/images/products/puma-calidream-pink.jpg');


GO


-- ==============================
-- 🧦 DANH MỤC: TẤT (MaDM = 8)
-- ==============================
INSERT INTO SANPHAM (TenSP, MaDM, MaTH, MoTa, HinhAnh, LuotMua)
VALUES
(N'Tất thể thao Nike Everyday Cushioned', 8, 1, N'Tất cổ trung Nike co giãn, thoáng khí, thấm hút tốt khi vận động.', 
'/images/products/vo1.jpg', 50),



(N'Tất Adidas Performance Cushioned', 8, 2, N'Tất Adidas mềm mại, ôm chân, hỗ trợ thoải mái cho mọi hoạt động thể thao.', 
'/images/products/tat adidas.jpg', 42);
UPDATE SANPHAM
SET HinhAnh = 'tat adidas.jpg' -- Thay đổi giá trị cột HinhAnh
WHERE TenSP = N'Tất Adidas Performance Cushioned'; -- Điều kiện tìm kiếm sản phẩm
UPDATE SANPHAM
SET HinhAnh = 'vo1.jpg' -- Thay đổi giá trị cột HinhAnh
WHERE TenSP = N'Tất thể thao Nike Everyday Cushioned'; -- Điều kiện tìm kiếm sản phẩm

select * from SANPHAM
-- ==============================
-- 🪢 DANH MỤC: DÂY GIÀY (MaDM = 9)
-- ==============================
INSERT INTO SANPHAM (TenSP, MaDM, MaTH, MoTa, HinhAnh, LuotMua)
VALUES
(N'Dây giày Nike Flat Laces 120cm', 9, 1, N'Dây giày phẳng Nike dài 120cm, thích hợp cho giày thể thao và sneakers.', 
'/images/products/daygiay nike.jpg', 35),


(N'Dây giày Bitis Hunter StreetStyle', 9, 3, N'Dây giày Bitis StreetStyle bền chắc, dễ phối màu, phù hợp phong cách năng động.', 
'/images/products/daygiay bettit.jpg', 20);
GO
UPDATE SANPHAM
SET HinhAnh = 'daygiay nike.jpg' -- Thay đổi giá trị cột HinhAnh
WHERE TenSP = N'Dây giày Nike Flat Laces 120cm'; -- Điều kiện tìm kiếm sản phẩm
UPDATE SANPHAM
SET HinhAnh = 'daygiay bettit.jpg' -- Thay đổi giá trị cột HinhAnh
WHERE TenSP = N'Dây giày Bitis Hunter StreetStyle'; -- Điều kiện tìm kiếm sản phẩm


-- ==============================
-- Biến thể cho Tất Nike Everyday
-- ==============================
INSERT INTO BIENTHE_SANPHAM (MaSP, MaMau, MaKichThuoc, GiaBan, SoLuong, HinhAnh)
VALUES
(17, 1, 12, 150000, 50, '/images/products/vo1.jpg');

-- ==============================
-- Biến thể cho Tất Adidas Performance
-- ==============================
INSERT INTO BIENTHE_SANPHAM (MaSP, MaMau, MaKichThuoc, GiaBan, SoLuong, HinhAnh)
VALUES
(18, 1, 12, 140000, 60, '/images/products/tat adidas.jpg');
-- ==============================
-- Biến thể cho Dây giày Nike Flat Laces
-- ==============================
INSERT INTO BIENTHE_SANPHAM (MaSP, MaMau, MaKichThuoc, GiaBan, SoLuong, HinhAnh)
VALUES
(19, 1, 12, 120000, 80, '/images/products/daygiay nike.jpg');
-- ==============================
-- Biến thể cho Dây giày Bitis Hunter
-- ==============================
INSERT INTO BIENTHE_SANPHAM (MaSP, MaMau, MaKichThuoc, GiaBan, SoLuong, HinhAnh)
VALUES
(20, 2, 12, 100000, 60, '/images/products/daygiay bettit.jpg');
GO

SELECT 
    SP.MaSP, 
    SP.TenSP, 
    DM.Ten AS TenDanhMuc, 
    TH.TenTH AS ThuongHieu
FROM SANPHAM SP
JOIN DANHMUC DM ON SP.MaDM = DM.MaDM
JOIN THUONGHIEU TH ON SP.MaTH = TH.MaTH
ORDER BY DM.ParentID, DM.MaDM, SP.MaSP;

-- =====================================
-- CHÈN DỮ LIỆU MẪU CHO BẢNG DANHGIA
-- =====================================

-- Đánh giá cho Sản phẩm 1: Nike Air Force 1 '07 (MaSP=1)
-- Khách hàng Hưng (MaTK=2) đánh giá 5 sao
INSERT INTO DANHGIA (MaTK, MaSP, SoSao, BinhLuan, DaDuyet, NgayDanhGia) VALUES
(2, 1, 5, N'Giày huyền thoại, đi rất êm và phong cách. Chuẩn size!', 1, GETDATE());

select * from DANHGIA
-- Thêm 2 đánh giá khác cho MaSP=1 từ các tài khoản giả định (MaTK=3, MaTK=4)
-- (Giả định bạn có thêm tài khoản MaTK=3 và MaTK=4)
INSERT INTO DANHGIA (MaTK, MaSP, SoSao, BinhLuan, DaDuyet, NgayDanhGia) VALUES
(2, 2, 4, N'Giày đẹp, nhưng hộp hơi móp.', 1, DATEADD(day, -5, GETDATE())),
(2, 2, 5, N'Quá hài lòng! Shipper giao nhanh.', 1, DATEADD(day, -2, GETDATE()));
-- >>> Kết quả: MaSP=1 có rating trung bình là (5+4+5)/3 = 4.67 (Làm tròn thành 4.5 hoặc 5.0)

-- Đánh giá cho Sản phẩm 9: Bitis Hunter Street (MaSP=9) - Rất nhiều đánh giá tốt
INSERT INTO DANHGIA (MaTK, MaSP, SoSao, BinhLuan, DaDuyet, NgayDanhGia) VALUES
(2, 9, 5, N'Giày quốc dân, giá tốt, chất lượng bền bỉ.', 1, GETDATE()),
(5, 9, 5, N'Màu trắng rất dễ phối đồ. Đáng tiền!', 1, DATEADD(day, -10, GETDATE())),
(6, 9, 4, N'Hơi nặng hơn mình nghĩ một chút, nhưng vẫn ổn.', 1, DATEADD(day, -8, GETDATE()));
-- >>> Kết quả: MaSP=9 có rating trung bình là (5+5+4)/3 = 4.67 (Làm tròn thành 4.5 hoặc 5.0)

-- Đánh giá cho Sản phẩm 14: Puma RS-X (MaSP=14) - Ít đánh giá, điểm thấp hơn
INSERT INTO DANHGIA (MaTK, MaSP, SoSao, BinhLuan, DaDuyet, NgayDanhGia) VALUES
(2, 14, 3, N'Form giày hơi ôm, đi lâu bị đau chân.', 1, GETDATE()),
(7, 14, 4, N'Thiết kế đẹp, đế êm, nhưng giá hơi cao so với chất lượng.', 1, DATEADD(month, -1, GETDATE()));
-- >>> Kết quả: MaSP=14 có rating trung bình là (3+4)/2 = 3.5

-- Đánh giá đang chờ duyệt (DaDuyet = 0) cho MaSP=3: Nike Dunk Low
INSERT INTO DANHGIA (MaTK, MaSP, SoSao, BinhLuan, DaDuyet, NgayDanhGia) VALUES
(2, 3, 5, N'Đang chờ shop duyệt bình luận này.', 0, GETDATE());
-- >>> Đánh giá này sẽ KHÔNG được tính vào điểm trung bình nếu logic tính toán của bạn (trong Model Sanpham) dùng điều kiện DaDuyet = true.

GO