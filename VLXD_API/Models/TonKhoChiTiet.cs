using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VLXD_API.Models;

[PrimaryKey("MaKho", "MaSanPham")]
[Table("ton_kho_chi_tiet")]
[Index("MaSanPham", Name = "fk_ton_sp")]
public partial class TonKhoChiTiet
{
    [Key]
    [Column("ma_kho")]
    public int MaKho { get; set; }

    [Key]
    [Column("ma_san_pham")]
    public int MaSanPham { get; set; }

    [Column("so_luong_ton")]
    [Precision(18, 2)]
    public decimal? SoLuongTon { get; set; }

    [Column("vi_tri_cu_the")]
    [StringLength(50)]
    public string? ViTriCuThe { get; set; }

    [ForeignKey("MaKho")]
    [InverseProperty("TonKhoChiTiets")]
    public virtual Kho MaKhoNavigation { get; set; } = null!;

    [ForeignKey("MaSanPham")]
    [InverseProperty("TonKhoChiTiets")]
    public virtual SanPham MaSanPhamNavigation { get; set; } = null!;
}
