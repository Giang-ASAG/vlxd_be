using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VLXD_API.Models;

[Table("don_vi_quy_doi")]
[Index("MaSanPham", Name = "fk_quydoi_sp")]
public partial class DonViQuyDoi
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("ma_san_pham")]
    public int? MaSanPham { get; set; }

    [Column("ten_don_vi")]
    [StringLength(20)]
    public string TenDonVi { get; set; } = null!;

    [Column("ty_le_quy_doi")]
    [Precision(18, 2)]
    public decimal TyLeQuyDoi { get; set; }

    [Column("gia_ban_theo_dv")]
    [Precision(18, 2)]
    public decimal? GiaBanTheoDv { get; set; }

    [ForeignKey("MaSanPham")]
    [InverseProperty("DonViQuyDois")]
    public virtual SanPham? MaSanPhamNavigation { get; set; }
}
