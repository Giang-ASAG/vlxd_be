namespace VLXD_API.DTOs.KhachHang;

public class KhachHangDto
{
    public int MaKhachHang { get; set; }
    public string HoTen { get; set; } = null!;
    public string? SoDienThoai { get; set; }
    public string? DiaChi { get; set; }
    public decimal? HanMucNo { get; set; }
}
