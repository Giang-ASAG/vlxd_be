using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VLXD_API.Models;

[Table("don_hang")]
[Index("MaKhachHang", Name = "fk_dh_kh")]
[Index("MaNguoiTao", Name = "fk_dh_nv")]
public partial class DonHang
{
    [Key]
    [Column("ma_don_hang")]
    public int MaDonHang { get; set; }

    [Column("ma_khach_hang")]
    public int? MaKhachHang { get; set; }

    [Column("ma_nguoi_tao")]
    public int? MaNguoiTao { get; set; }

    [Column("ngay_tao", TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [Column("tong_tien")]
    [Precision(18, 2)]
    public decimal? TongTien { get; set; }

    [Column("trang_thai_thanh_toan", TypeName = "enum('chua_thanh_toan','thanh_toan_mot_phan','da_thanh_toan')")]
    public string? TrangThaiThanhToan { get; set; }

    [Column("so_tien_tra")]
    [Precision(18, 2)]
    public decimal? SoTienTra { get; set; }

    [Column("hinh_thuc")]
    public bool? HinhThuc { get; set; }

    [InverseProperty("MaDonHangNavigation")]
    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    [ForeignKey("MaKhachHang")]
    [InverseProperty("DonHangs")]
    public virtual KhachHang? MaKhachHangNavigation { get; set; }

    [ForeignKey("MaNguoiTao")]
    [InverseProperty("DonHangs")]
    public virtual NguoiDung? MaNguoiTaoNavigation { get; set; }

    [InverseProperty("MaDonHangNavigation")]
    public virtual ICollection<PhieuXuatKho> PhieuXuatKhos { get; set; } = new List<PhieuXuatKho>();
}
