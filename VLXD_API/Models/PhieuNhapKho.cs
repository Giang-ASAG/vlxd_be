using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VLXD_API.Models;

[Table("phieu_nhap_kho")]
[Index("MaKhoNhap", Name = "fk_pnk_kho")]
[Index("MaNcc", Name = "fk_pnk_ncc")]
[Index("MaNguoiLap", Name = "fk_pnk_nv")]
public partial class PhieuNhapKho
{
    [Key]
    [Column("ma_phieu_nhap")]
    public int MaPhieuNhap { get; set; }

    [Column("ma_ncc")]
    public int? MaNcc { get; set; }

    [Column("ma_kho_nhap")]
    public int? MaKhoNhap { get; set; }

    [Column("ma_nguoi_lap")]
    public int? MaNguoiLap { get; set; }

    [Column("ngay_nhap", TypeName = "datetime")]
    public DateTime? NgayNhap { get; set; }

    [Column("tong_tien_nhap")]
    [Precision(18, 2)]
    public decimal? TongTienNhap { get; set; }

    [Column("da_thanh_toan_ncc")]
    [Precision(18, 2)]
    public decimal? DaThanhToanNcc { get; set; }

    [Column("trang_thai", TypeName = "enum('cho_nhap','da_nhap_kho','da_huy')")]
    public string? TrangThai { get; set; }

    [InverseProperty("MaPhieuNhapNavigation")]
    public virtual ICollection<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; } = new List<ChiTietPhieuNhap>();

    [ForeignKey("MaKhoNhap")]
    [InverseProperty("PhieuNhapKhos")]
    public virtual Kho? MaKhoNhapNavigation { get; set; }

    [ForeignKey("MaNcc")]
    [InverseProperty("PhieuNhapKhos")]
    public virtual NhaCungCap? MaNccNavigation { get; set; }

    [ForeignKey("MaNguoiLap")]
    [InverseProperty("PhieuNhapKhos")]
    public virtual NguoiDung? MaNguoiLapNavigation { get; set; }
}
