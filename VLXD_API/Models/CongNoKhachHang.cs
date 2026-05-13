using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VLXD_API.Models;

[Table("cong_no_khach_hang")]
[Index("MaKhachHang", Name = "fk_cnkh_kh")]
public partial class CongNoKhachHang
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("ma_khach_hang")]
    public int? MaKhachHang { get; set; }

    [Column("ma_don_hang")]
    public int? MaDonHang { get; set; }

    [Column("so_tien_no")]
    [Precision(18, 2)]
    public decimal SoTienNo { get; set; }

    [Column("ngay_phat_sinh", TypeName = "datetime")]
    public DateTime? NgayPhatSinh { get; set; }

    [Column("trang_thai", TypeName = "enum('dang_no','hoan_tat')")]
    public string? TrangThai { get; set; }

    [ForeignKey("MaKhachHang")]
    [InverseProperty("CongNoKhachHangs")]
    public virtual KhachHang? MaKhachHangNavigation { get; set; }
}
