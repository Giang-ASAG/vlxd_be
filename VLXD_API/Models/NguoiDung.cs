using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VLXD_API.Models;

[Table("nguoi_dung")]
[Index("TenDangNhap", Name = "ten_dang_nhap", IsUnique = true)]
public partial class NguoiDung
{
    [Key]
    [Column("ma_nguoi_dung")]
    public int MaNguoiDung { get; set; }

    [Column("ten_dang_nhap")]
    [StringLength(50)]
    public string TenDangNhap { get; set; } = null!;

    [Column("mat_khau_hash")]
    [StringLength(255)]
    public string MatKhauHash { get; set; } = null!;

    [Column("ho_ten")]
    [StringLength(100)]
    public string? HoTen { get; set; }

    [Column("vai_tro", TypeName = "enum('admin','ban_hang','thu_kho','ke_toán')")]
    public string? VaiTro { get; set; }

    [InverseProperty("MaNguoiTaoNavigation")]
    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    [InverseProperty("MaNguoiLapNavigation")]
    public virtual ICollection<PhieuNhapKho> PhieuNhapKhos { get; set; } = new List<PhieuNhapKho>();
}
