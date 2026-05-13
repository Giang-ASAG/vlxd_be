using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VLXD_API.Models;

[Table("the_kho")]
[Index("MaKho", Name = "fk_tk_kho")]
[Index("MaSanPham", Name = "fk_tk_sp")]
public partial class TheKho
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("ma_san_pham")]
    public int? MaSanPham { get; set; }

    [Column("ma_kho")]
    public int? MaKho { get; set; }

    [Column("ngay_thay_doi", TypeName = "datetime")]
    public DateTime? NgayThayDoi { get; set; }

    [Column("loai_giao_dich", TypeName = "enum('NHAP_HANG','XUAT_HANG','KIEM_KE','TRA_HANG')")]
    public string LoaiGiaoDich { get; set; } = null!;

    [Column("so_luong_thay_doi")]
    [Precision(18, 2)]
    public decimal SoLuongThayDoi { get; set; }

    [Column("so_luong_ton_sau_khi_thay_doi")]
    [Precision(18, 2)]
    public decimal SoLuongTonSauKhiThayDoi { get; set; }

    [Column("ma_chung_tu_lien_quan")]
    [StringLength(50)]
    public string? MaChungTuLienQuan { get; set; }

    [ForeignKey("MaKho")]
    [InverseProperty("TheKhos")]
    public virtual Kho? MaKhoNavigation { get; set; }

    [ForeignKey("MaSanPham")]
    [InverseProperty("TheKhos")]
    public virtual SanPham? MaSanPhamNavigation { get; set; }
}
