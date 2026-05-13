using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VLXD_API.Models;

[Table("danh_muc")]
public partial class DanhMuc
{
    [Key]
    [Column("ma_danh_muc")]
    public int MaDanhMuc { get; set; }

    [Column("ten_danh_muc")]
    [StringLength(100)]
    public string TenDanhMuc { get; set; } = null!;

    [Column("mo_ta", TypeName = "text")]
    public string? MoTa { get; set; }

    [InverseProperty("MaDanhMucNavigation")]
    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
