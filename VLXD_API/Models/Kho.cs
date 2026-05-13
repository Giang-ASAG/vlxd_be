using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VLXD_API.Models;

[Table("kho")]
public partial class Kho
{
    [Key]
    [Column("ma_kho")]
    public int MaKho { get; set; }

    [Column("ten_kho")]
    [StringLength(100)]
    public string TenKho { get; set; } = null!;

    [Column("dia_chi")]
    [StringLength(255)]
    public string? DiaChi { get; set; }

    [Column("so_dien_thoai_kho")]
    [StringLength(15)]
    public string? SoDienThoaiKho { get; set; }

    [Column("trang_thai")]
    public bool? TrangThai { get; set; }

    [InverseProperty("MaKhoNhapNavigation")]
    public virtual ICollection<PhieuNhapKho> PhieuNhapKhos { get; set; } = new List<PhieuNhapKho>();

    [InverseProperty("MaKhoXuatNavigation")]
    public virtual ICollection<PhieuXuatKho> PhieuXuatKhos { get; set; } = new List<PhieuXuatKho>();

    [InverseProperty("MaKhoNavigation")]
    public virtual ICollection<TheKho> TheKhos { get; set; } = new List<TheKho>();

    [InverseProperty("MaKhoNavigation")]
    public virtual ICollection<TonKhoChiTiet> TonKhoChiTiets { get; set; } = new List<TonKhoChiTiet>();
}
