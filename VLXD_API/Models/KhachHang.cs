using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VLXD_API.Models;

[Table("khach_hang")]
[Index("SoDienThoai", Name = "so_dien_thoai", IsUnique = true)]
public partial class KhachHang
{
    [Key]
    [Column("ma_khach_hang")]
    public int MaKhachHang { get; set; }

    [Column("ho_ten")]
    [StringLength(100)]
    public string HoTen { get; set; } = null!;

    [Column("so_dien_thoai")]
    [StringLength(15)]
    public string? SoDienThoai { get; set; }

    [Column("dia_chi")]
    [StringLength(255)]
    public string? DiaChi { get; set; }

    [Column("han_muc_no")]
    [Precision(18, 2)]
    public decimal? HanMucNo { get; set; }

    [InverseProperty("MaKhachHangNavigation")]
    public virtual ICollection<CongNoKhachHang> CongNoKhachHangs { get; set; } = new List<CongNoKhachHang>();

    [InverseProperty("MaKhachHangNavigation")]
    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
}
