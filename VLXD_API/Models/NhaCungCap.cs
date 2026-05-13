using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VLXD_API.Models;

[Table("nha_cung_cap")]
public partial class NhaCungCap
{
    [Key]
    [Column("ma_ncc")]
    public int MaNcc { get; set; }

    [Column("ten_ncc")]
    [StringLength(255)]
    public string TenNcc { get; set; } = null!;

    [Column("so_dien_thoai")]
    [StringLength(15)]
    public string? SoDienThoai { get; set; }

    [Column("email")]
    [StringLength(100)]
    public string? Email { get; set; }

    [Column("dia_chi", TypeName = "text")]
    public string? DiaChi { get; set; }

    [Column("ghi_chu", TypeName = "text")]
    public string? GhiChu { get; set; }

    [InverseProperty("MaNccNavigation")]
    public virtual ICollection<CongNoNcc> CongNoNccs { get; set; } = new List<CongNoNcc>();

    [InverseProperty("MaNccNavigation")]
    public virtual ICollection<PhieuNhapKho> PhieuNhapKhos { get; set; } = new List<PhieuNhapKho>();

    [InverseProperty("MaNccMacDinhNavigation")]
    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
