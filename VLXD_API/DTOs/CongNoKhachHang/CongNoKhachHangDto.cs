namespace VLXD_API.DTOs.CongNoKhachHang;

public class CongNoKhachHangDto
{
    public int Id { get; set; }
    public int? MaKhachHang { get; set; }
    public int? MaDonHang { get; set; }
    public decimal SoTienNo { get; set; }
    public DateTime? NgayPhatSinh { get; set; }
    public string? TrangThai { get; set; }
}
