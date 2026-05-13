using VLXD_API.DTOs.ChiTietPhieuNhap;
using VLXD_API.DTOs.SanPham;

namespace VLXD_API.DTOs.PhieuNhapKho;

public class PhieuNhapKhoDto
{
    public int MaPhieuNhap { get; set; }
    public int? MaNcc { get; set; }
    public int? MaKhoNhap { get; set; }
    public int? MaNguoiLap { get; set; }
    public DateTime? NgayNhap { get; set; }
    public decimal? TongTienNhap { get; set; }
    public decimal? DaThanhToanNcc { get; set; }
    public string? TrangThai { get; set; }
}



