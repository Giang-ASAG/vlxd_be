using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VLXD_API.Models;

[Table("cong_no_ncc")]
[Index("MaNcc", Name = "fk_cnncc_ncc")]
public partial class CongNoNcc
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("ma_ncc")]
    public int? MaNcc { get; set; }

    [Column("ma_phieu_nhap")]
    public int? MaPhieuNhap { get; set; }

    [Column("so_tien_no")]
    [Precision(18, 2)]
    public decimal SoTienNo { get; set; }

    [Column("ngay_phat_sinh", TypeName = "datetime")]
    public DateTime? NgayPhatSinh { get; set; }

    [Column("trang_thai", TypeName = "enum('dang_no','hoan_tat')")]
    public string? TrangThai { get; set; }

    [ForeignKey("MaNcc")]
    [InverseProperty("CongNoNccs")]
    public virtual NhaCungCap? MaNccNavigation { get; set; }
}
