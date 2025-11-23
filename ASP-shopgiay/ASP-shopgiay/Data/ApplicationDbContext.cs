using System;
using System.Collections.Generic;
using ASP_shopgiay.Models;
using Microsoft.EntityFrameworkCore;

namespace ASP_shopgiay.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BientheSanpham> BientheSanphams { get; set; }

    public virtual DbSet<Chucvu> Chucvus { get; set; }

    public virtual DbSet<Cthoadon> Cthoadons { get; set; }

    public virtual DbSet<Danhgium> Danhgia { get; set; }

    public virtual DbSet<Danhmuc> Danhmucs { get; set; }

    public virtual DbSet<DanhsachYeuThich> DanhsachYeuThiches { get; set; }

    public virtual DbSet<Diachi> Diachis { get; set; }

    public virtual DbSet<Giohang> Giohangs { get; set; }

    public virtual DbSet<Hoadon> Hoadons { get; set; }

    public virtual DbSet<Kichthuoc> Kichthuocs { get; set; }

    public virtual DbSet<Magiamgium> Magiamgia { get; set; }

    public virtual DbSet<Mausac> Mausacs { get; set; }

    public virtual DbSet<Phuongthucthanhtoan> Phuongthucthanhtoans { get; set; }

    public virtual DbSet<Sanpham> Sanphams { get; set; }

    public virtual DbSet<Taikhoan> Taikhoans { get; set; }

    public virtual DbSet<Thuonghieu> Thuonghieus { get; set; }

    public virtual DbSet<TrangthaiDonhang> TrangthaiDonhangs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=ADMIN\\MAY10;Initial Catalog=ShopGiay;Integrated Security=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BientheSanpham>(entity =>
        {
            entity.HasKey(e => e.MaBienThe).HasName("PK__BIENTHE___3987CEF5B9396783");

            entity.Property(e => e.SoLuong).HasDefaultValue(0);

            entity.HasOne(d => d.MaKichThuocNavigation).WithMany(p => p.BientheSanphams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BT_SIZE");

            entity.HasOne(d => d.MaMauNavigation).WithMany(p => p.BientheSanphams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BT_MAU");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.BientheSanphams).HasConstraintName("FK_BT_SP");
        });

        modelBuilder.Entity<Chucvu>(entity =>
        {
            entity.HasKey(e => e.MaCv).HasName("PK__CHUCVU__27258E76699B3539");
        });

        modelBuilder.Entity<Cthoadon>(entity =>
        {
            entity.HasKey(e => e.MaCthd).HasName("PK__CTHOADON__1E4FA771EB9448F8");

            entity.Property(e => e.GiamGiaCt).HasDefaultValue(0m);
            entity.Property(e => e.ThanhTienGoc).HasComputedColumnSql("([SoLuong]*[DonGia])", true);

            entity.HasOne(d => d.MaBienTheNavigation).WithMany(p => p.Cthoadons)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTHD_BT");

            entity.HasOne(d => d.MaHdNavigation).WithMany(p => p.Cthoadons).HasConstraintName("FK_CTHD_HD");
        });

        modelBuilder.Entity<Danhgium>(entity =>
        {
            entity.HasKey(e => e.MaDanhGia).HasName("PK__DANHGIA__AA9515BFD433BC61");

            entity.Property(e => e.DaDuyet).HasDefaultValue(false);
            entity.Property(e => e.NgayDanhGia).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.Danhgia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DG_SP");

            entity.HasOne(d => d.MaTkNavigation).WithMany(p => p.Danhgia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DG_TAIKHOAN");
        });

        modelBuilder.Entity<Danhmuc>(entity =>
        {
            entity.HasKey(e => e.MaDm).HasName("PK__DANHMUC__2725866E6FFE09C4");

            entity.Property(e => e.ThuTu).HasDefaultValue(0);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent).HasConstraintName("FK_DANHMUC_PARENT");
        });

        modelBuilder.Entity<DanhsachYeuThich>(entity =>
        {
            entity.HasKey(e => e.MaYeuThich).HasName("PK__DANHSACH__B9007E4C56972CB6");

            entity.Property(e => e.NgayThem).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaBienTheNavigation).WithMany(p => p.DanhsachYeuThiches)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_YT_BT");

            entity.HasOne(d => d.MaTkNavigation).WithMany(p => p.DanhsachYeuThiches).HasConstraintName("FK_YT_TAIKHOAN");
        });

        modelBuilder.Entity<Diachi>(entity =>
        {
            entity.HasKey(e => e.MaDc).HasName("PK__DIACHI__27258664B5943362");

            entity.Property(e => e.MacDinh).HasDefaultValue(false);

            entity.HasOne(d => d.MaTkNavigation).WithMany(p => p.Diachis).HasConstraintName("FK_DIACHI_TAIKHOAN");
        });

        modelBuilder.Entity<Giohang>(entity =>
        {
            entity.HasKey(e => e.MaGioHang).HasName("PK__GIOHANG__F5001DA30A1A339B");

            entity.Property(e => e.NgayThem).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SoLuong).HasDefaultValue(1);

            entity.HasOne(d => d.MaBienTheNavigation).WithMany(p => p.Giohangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GH_BT");

            entity.HasOne(d => d.MaTkNavigation).WithMany(p => p.Giohangs).HasConstraintName("FK_GH_TAIKHOAN");
        });

        modelBuilder.Entity<Hoadon>(entity =>
        {
            entity.HasKey(e => e.MaHd).HasName("PK__HOADON__2725A6E011673B1F");

            entity.Property(e => e.GiamGia).HasDefaultValue(0m);
            entity.Property(e => e.Ngay).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.PhiVanChuyen).HasDefaultValue(0m);
            entity.Property(e => e.TamTinh).HasDefaultValue(0m);
            entity.Property(e => e.TongTien).HasDefaultValue(0m);

            entity.HasOne(d => d.MaGiamGiaNavigation).WithMany(p => p.Hoadons).HasConstraintName("FK_HD_GG");

            entity.HasOne(d => d.MaPtNavigation).WithMany(p => p.Hoadons)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HD_PT");

            entity.HasOne(d => d.MaTkNavigation).WithMany(p => p.Hoadons)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HD_TAIKHOAN");

            entity.HasOne(d => d.MaTrangThaiNavigation).WithMany(p => p.Hoadons)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HD_TT");
        });

        modelBuilder.Entity<Kichthuoc>(entity =>
        {
            entity.HasKey(e => e.MaKichThuoc).HasName("PK__KICHTHUO__22BFD66471A2527E");
        });

        modelBuilder.Entity<Magiamgium>(entity =>
        {
            entity.HasKey(e => e.MaGiamGia).HasName("PK__MAGIAMGI__EF9458E40BACB660");

            entity.Property(e => e.NgayBatDau).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.PhanTramGiam).HasDefaultValue(0.0);
            entity.Property(e => e.SoLuong).HasDefaultValue(1);
            entity.Property(e => e.SoTienGiam).HasDefaultValue(0);
        });

        modelBuilder.Entity<Mausac>(entity =>
        {
            entity.HasKey(e => e.MaMau).HasName("PK__MAUSAC__3A5BBB7D7780E09E");
        });

        modelBuilder.Entity<Phuongthucthanhtoan>(entity =>
        {
            entity.HasKey(e => e.MaPt).HasName("PK__PHUONGTH__2725E7F690ADFFF7");

            entity.Property(e => e.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<Sanpham>(entity =>
        {
            entity.HasKey(e => e.MaSp).HasName("PK__SANPHAM__2725081C6FB5A1F9");

            entity.Property(e => e.LuotMua).HasDefaultValue(0);
            entity.Property(e => e.LuotXem).HasDefaultValue(0);

            entity.HasOne(d => d.MaDmNavigation).WithMany(p => p.Sanphams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SP_DM");

            entity.HasOne(d => d.MaThNavigation).WithMany(p => p.Sanphams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SP_TH");
        });

        modelBuilder.Entity<Taikhoan>(entity =>
        {
            entity.HasKey(e => e.MaTk).HasName("PK__TAIKHOAN__2725007085695207");

            entity.Property(e => e.TrangThai).HasDefaultValue(true);

            entity.HasOne(d => d.MaCvNavigation).WithMany(p => p.Taikhoans).HasConstraintName("FK_TAIKHOAN_CHUCVU");
        });

        modelBuilder.Entity<Thuonghieu>(entity =>
        {
            entity.HasKey(e => e.MaTh).HasName("PK__THUONGHI__272500753C0B0092");
        });

        modelBuilder.Entity<TrangthaiDonhang>(entity =>
        {
            entity.HasKey(e => e.MaTrangThai).HasName("PK__TRANGTHA__AADE41381286BAD0");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
