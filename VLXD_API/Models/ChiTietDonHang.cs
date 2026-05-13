using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VLXD_API.Models;

[Table("chi_tiet_don_hang")]
[Index("MaDonHang", Name = "fk_ctdh_dh")]
[Index("MaSanPham", Name = "fk_ctdh_sp")]
public partial class ChiTietDonHang
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("ma_don_hang")]
    public int? MaDonHang { get; set; }

    [Column("ma_san_pham")]
    public int? MaSanPham { get; set; }

    [Column("so_luong")]
    [Precision(18, 2)]
    public decimal SoLuong { get; set; }

    [Column("don_vi_tinh")]
    [StringLength(20)]
    public string? DonViTinh { get; set; }

    [Column("don_gia")]
    [Precision(18, 2)]
    public decimal DonGia { get; set; }

    [Column("thanh_tien")]
    [Precision(18, 2)]
    public decimal? ThanhTien { get; set; }

    [ForeignKey("MaDonHang")]
    [InverseProperty("ChiTietDonHangs")]
    public virtual DonHang? MaDonHangNavigation { get; set; }

    [ForeignKey("MaSanPham")]
    [InverseProperty("ChiTietDonHangs")]
    public virtual SanPham? MaSanPhamNavigation { get; set; }
}
