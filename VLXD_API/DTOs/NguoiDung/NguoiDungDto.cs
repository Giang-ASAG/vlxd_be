namespace VLXD_API.DTOs.NguoiDung;

public class NguoiDungDto
{
    public int MaNguoiDung { get; set; }
    public string TenDangNhap { get; set; } = null!;
    public string MatKhauHash { get; set; } = null!;
    public string? HoTen { get; set; }
    public string? VaiTro { get; set; }
}
