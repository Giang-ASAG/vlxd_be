using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace VLXD_API.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

    public virtual DbSet<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; }

    public virtual DbSet<CongNoKhachHang> CongNoKhachHangs { get; set; }

    public virtual DbSet<CongNoNcc> CongNoNccs { get; set; }

    public virtual DbSet<DanhMuc> DanhMucs { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<DonViQuyDoi> DonViQuyDois { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<Kho> Khos { get; set; }
    public virtual DbSet<LichSuThanhToan> LichSuThanhToans { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<NhaCungCap> NhaCungCaps { get; set; }

    public virtual DbSet<PhieuNhapKho> PhieuNhapKhos { get; set; }

    public virtual DbSet<PhieuXuatKho> PhieuXuatKhos { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    public virtual DbSet<TheKho> TheKhos { get; set; }

    public virtual DbSet<TonKhoChiTiet> TonKhoChiTiets { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseMySql("server=localhost;port=3306;database=deafaultdb;user=root;password=123123", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.43-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.ThanhTien).HasComputedColumnSql("`so_luong` * `don_gia`", true);

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.ChiTietDonHangs).HasConstraintName("fk_ctdh_dh");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.ChiTietDonHangs).HasConstraintName("fk_ctdh_sp");
        });
        modelBuilder.Entity<LichSuThanhToan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            // Đảm bảo tên bảng khớp với DB (Lưu ý hoa/thường)
            entity.ToTable("lichsuthanhtoan");

            entity.Property(e => e.IsNhaCungCap)
                .HasColumnType("tinyint(1)")
                .IsRequired();

            // BỔ SUNG: Ánh xạ conNoID
            entity.Property(e => e.conNoID)
                .HasColumnName("conNoID") // Map chính xác tên cột bạn đã yêu cầu đổi
                .IsRequired();

            entity.Property(e => e.SoTien)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(e => e.PhuongThucThanhToan)
                .HasColumnType("tinyint(1)")
                .HasDefaultValue(false)
                .HasComment("0: tien_mat, 1: chuyen_khoan");

            entity.Property(e => e.NgayThanhToan)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.Property(e => e.GhiChu)
                .HasMaxLength(500)
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");
        });
        modelBuilder.Entity<ChiTietPhieuNhap>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.ThanhTien)
                .HasComputedColumnSql("`so_luong` * `gia_nhap`", true);

            entity.Property(e => e.LoaiNhap)
                .HasDefaultValue(false);

            entity.HasOne(d => d.MaPhieuNhapNavigation)
                .WithMany(p => p.ChiTietPhieuNhaps)
                .HasConstraintName("fk_ctpn_pn");

            entity.HasOne(d => d.MaSanPhamNavigation)
                .WithMany(p => p.ChiTietPhieuNhaps)
                .HasConstraintName("fk_ctpn_sp");
        });

        modelBuilder.Entity<CongNoKhachHang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.NgayPhatSinh).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.TrangThai).HasDefaultValueSql("'dang_no'");

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.CongNoKhachHangs).HasConstraintName("fk_cnkh_kh");
        });

        modelBuilder.Entity<CongNoNcc>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.NgayPhatSinh).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.TrangThai).HasDefaultValueSql("'dang_no'");

            entity.HasOne(d => d.MaNccNavigation).WithMany(p => p.CongNoNccs).HasConstraintName("fk_cnncc_ncc");
        });

        modelBuilder.Entity<DanhMuc>(entity =>
        {
            entity.HasKey(e => e.MaDanhMuc).HasName("PRIMARY");
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.MaDonHang).HasName("PRIMARY");

            entity.Property(e => e.NgayTao).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.TongTien).HasDefaultValueSql("'0.00'");
            entity.Property(e => e.TrangThaiThanhToan).HasDefaultValueSql("'chua_thanh_toan'");

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.DonHangs).HasConstraintName("fk_dh_kh");

            entity.HasOne(d => d.MaNguoiTaoNavigation).WithMany(p => p.DonHangs).HasConstraintName("fk_dh_nv");
        });

        modelBuilder.Entity<DonViQuyDoi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.DonViQuyDois).HasConstraintName("fk_quydoi_sp");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKhachHang).HasName("PRIMARY");

            entity.Property(e => e.HanMucNo).HasDefaultValueSql("'0.00'");
        });

        modelBuilder.Entity<Kho>(entity =>
        {
            entity.HasKey(e => e.MaKho).HasName("PRIMARY");

            entity.Property(e => e.TrangThai).HasDefaultValueSql("'1'");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaNguoiDung).HasName("PRIMARY");

            entity.Property(e => e.VaiTro).HasDefaultValueSql("'ban_hang'");
        });

        modelBuilder.Entity<NhaCungCap>(entity =>
        {
            entity.HasKey(e => e.MaNcc).HasName("PRIMARY");
        });

        modelBuilder.Entity<PhieuNhapKho>(entity =>
        {
            entity.HasKey(e => e.MaPhieuNhap).HasName("PRIMARY");

            entity.Property(e => e.DaThanhToanNcc).HasDefaultValueSql("'0.00'");
            entity.Property(e => e.NgayNhap).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.TongTienNhap).HasDefaultValueSql("'0.00'");
            entity.Property(e => e.TrangThai).HasDefaultValueSql("'cho_nhap'");

            entity.HasOne(d => d.MaKhoNhapNavigation).WithMany(p => p.PhieuNhapKhos).HasConstraintName("fk_pnk_kho");

            entity.HasOne(d => d.MaNccNavigation).WithMany(p => p.PhieuNhapKhos).HasConstraintName("fk_pnk_ncc");

            entity.HasOne(d => d.MaNguoiLapNavigation).WithMany(p => p.PhieuNhapKhos).HasConstraintName("fk_pnk_nv");
        });

        modelBuilder.Entity<PhieuXuatKho>(entity =>
        {
            entity.HasKey(e => e.MaPhieuXuat).HasName("PRIMARY");

            entity.Property(e => e.NgayXuat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.TrangThaiXuat).HasDefaultValueSql("'cho_xuat'");

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.PhieuXuatKhos).HasConstraintName("fk_pxk_dh");

            entity.HasOne(d => d.MaKhoXuatNavigation).WithMany(p => p.PhieuXuatKhos).HasConstraintName("fk_pxk_kho");
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.MaSanPham).HasName("PRIMARY");

            entity.Property(e => e.SoLuong)
                .HasColumnName("so_luong")
                .HasDefaultValue(0);

            entity.Property(e => e.GiaBanLe)
                .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.GiaNhapGanNhat)
                .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.Thue)
                .HasPrecision(5, 2)
                .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.GiaSauThue)
                .HasPrecision(18, 2)
                .HasDefaultValueSql("'0.00'");

            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.TonKhoToiDa)
                .HasDefaultValueSql("'500'");

            entity.Property(e => e.TonKhoToiThieu)
                .HasDefaultValueSql("'10'");

            entity.HasOne(d => d.MaDanhMucNavigation)
                .WithMany(p => p.SanPhams)
                .HasConstraintName("fk_sp_danhmuc");

            entity.HasOne(d => d.MaNccMacDinhNavigation)
                .WithMany(p => p.SanPhams)
                .HasConstraintName("fk_sp_ncc");
        });

        modelBuilder.Entity<TheKho>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.NgayThayDoi).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.MaKhoNavigation).WithMany(p => p.TheKhos).HasConstraintName("fk_tk_kho");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.TheKhos).HasConstraintName("fk_tk_sp");
        });

        modelBuilder.Entity<TonKhoChiTiet>(entity =>
        {
            entity.HasKey(e => new { e.MaKho, e.MaSanPham })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.Property(e => e.SoLuongTon).HasDefaultValueSql("'0.00'");

            entity.HasOne(d => d.MaKhoNavigation).WithMany(p => p.TonKhoChiTiets)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ton_kho");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.TonKhoChiTiets)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ton_sp");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
