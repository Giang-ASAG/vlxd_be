using System.ComponentModel.DataAnnotations.Schema;

namespace VLXD_API.DTOs.ChiTietPhieuNhap;

public class ChiTietPhieuNhapDto
{
    public int Id { get; set; }
    public int? MaPhieuNhap { get; set; }
    public int? MaSanPham { get; set; }
    public string? TenSanPham { get; set; }
    public decimal SoLuong { get; set; }
    public decimal GiaNhap { get; set; }
    public bool? LoaiNhap { get; set; } //False = nhap sp, true = nhap kho
    public decimal? ThanhTien { get; set; }
}
