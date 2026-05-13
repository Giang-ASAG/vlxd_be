using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VLXD_API.Models;

[Table("chi_tiet_phieu_nhap")]
[Index("MaPhieuNhap", Name = "fk_ctpn_pn")]
[Index("MaSanPham", Name = "fk_ctpn_sp")]
public partial class ChiTietPhieuNhap
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("ma_phieu_nhap")]
    public int? MaPhieuNhap { get; set; }

    [Column("ma_san_pham")]
    public int? MaSanPham { get; set; }

    [Column("so_luong")]
    [Precision(18, 2)]
    public decimal SoLuong { get; set; }

    [Column("gia_nhap")]
    [Precision(18, 2)]
    public decimal GiaNhap { get; set; }
    [Column("loai_nhap")]
    public bool? LoaiNhap { get; set; }

    [Column("thanh_tien")]
    [Precision(18, 2)]
    public decimal? ThanhTien { get; set; }

    [ForeignKey("MaPhieuNhap")]
    [InverseProperty("ChiTietPhieuNhaps")]
    public virtual PhieuNhapKho? MaPhieuNhapNavigation { get; set; }

    [ForeignKey("MaSanPham")]
    [InverseProperty("ChiTietPhieuNhaps")]
    public virtual SanPham? MaSanPhamNavigation { get; set; }
}
