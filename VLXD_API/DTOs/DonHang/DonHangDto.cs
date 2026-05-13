using VLXD_API.DTOs.ChiTietDonHang;

namespace VLXD_API.DTOs.DonHang;

public class DonHangDto
{
    public int MaDonHang { get; set; }
    public int? MaKhachHang { get; set; }
    public int? MaNguoiTao { get; set; }
    public DateTime? NgayTao { get; set; }
    public decimal? TongTien { get; set; }
    public string? TrangThaiThanhToan { get; set; }
    public bool HinhThuc { get; set; }
    public decimal? SoTienTra { get; set; }
}




