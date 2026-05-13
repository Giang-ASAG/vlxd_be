using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VLXD_API.Models;

[Table("san_pham")]
[Index("MaDanhMuc", Name = "fk_sp_danhmuc")]
[Index("MaNccMacDinh", Name = "fk_sp_ncc")]
[Index("MaSku", Name = "ma_sku", IsUnique = true)]
public partial class SanPham
{
    [Key]
    [Column("ma_san_pham")]
    public int MaSanPham { get; set; }

    [Column("ma_sku")]
    [StringLength(50)]
    public string MaSku { get; set; } = null!;

    [Column("ten_san_pham")]
    [StringLength(255)]
    public string TenSanPham { get; set; } = null!;

    [Column("ma_danh_muc")]
    public int? MaDanhMuc { get; set; }

    [Column("ma_ncc_mac_dinh")]
    public int? MaNccMacDinh { get; set; }

    [Column("don_vi_chinh")]
    [StringLength(20)]
    public string DonViChinh { get; set; } = null!;

    [Column("gia_nhap_gan_nhat")]
    [Precision(18, 2)]
    public decimal? GiaNhapGanNhat { get; set; }

    [Column("gia_ban_le")]
    [Precision(18, 2)]
    public decimal? GiaBanLe { get; set; }
    [Column("thue")]
    [Precision(5, 2)]
    public decimal? Thue { get; set; }

    [Column("gia_sau_thue")]
    [Precision(18, 2)]
    public decimal? GiaSauThue { get; set; }

    [Column("so_luong")]
    public int SoLuong { get; set; }

    [Column("ton_kho_toi_thieu")]
    public int? TonKhoToiThieu { get; set; }

    [Column("ton_kho_toi_da")]
    public int? TonKhoToiDa { get; set; }

    [Column("ngay_tao", TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [InverseProperty("MaSanPhamNavigation")]
    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    [InverseProperty("MaSanPhamNavigation")]
    public virtual ICollection<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; } = new List<ChiTietPhieuNhap>();

    [InverseProperty("MaSanPhamNavigation")]
    public virtual ICollection<DonViQuyDoi> DonViQuyDois { get; set; } = new List<DonViQuyDoi>();

    [ForeignKey("MaDanhMuc")]
    [InverseProperty("SanPhams")]
    public virtual DanhMuc? MaDanhMucNavigation { get; set; }

    [ForeignKey("MaNccMacDinh")]
    [InverseProperty("SanPhams")]
    public virtual NhaCungCap? MaNccMacDinhNavigation { get; set; }

    [InverseProperty("MaSanPhamNavigation")]
    public virtual ICollection<TheKho> TheKhos { get; set; } = new List<TheKho>();

    [InverseProperty("MaSanPhamNavigation")]
    public virtual ICollection<TonKhoChiTiet> TonKhoChiTiets { get; set; } = new List<TonKhoChiTiet>();
}
