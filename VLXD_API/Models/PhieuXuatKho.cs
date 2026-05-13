using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VLXD_API.Models;

[Table("phieu_xuat_kho")]
[Index("MaDonHang", Name = "fk_pxk_dh")]
[Index("MaKhoXuat", Name = "fk_pxk_kho")]
public partial class PhieuXuatKho
{
    [Key]
    [Column("ma_phieu_xuat")]
    public int MaPhieuXuat { get; set; }

    [Column("ma_don_hang")]
    public int? MaDonHang { get; set; }

    [Column("ma_kho_xuat")]
    public int? MaKhoXuat { get; set; }

    [Column("ma_nguoi_xuat")]
    public int? MaNguoiXuat { get; set; }

    [Column("ngay_xuat", TypeName = "datetime")]
    public DateTime? NgayXuat { get; set; }

    [Column("trang_thai_xuat", TypeName = "enum('cho_xuat','da_xuat_kho')")]
    public string? TrangThaiXuat { get; set; }

    [ForeignKey("MaDonHang")]
    [InverseProperty("PhieuXuatKhos")]
    public virtual DonHang? MaDonHangNavigation { get; set; }

    [ForeignKey("MaKhoXuat")]
    [InverseProperty("PhieuXuatKhos")]
    public virtual Kho? MaKhoXuatNavigation { get; set; }
}
