namespace VLXD_API.DTOs.ChiTietDonHang;

public class ChiTietDonHangDto
{
    public int Id { get; set; }
    public int? MaDonHang { get; set; }
    public int? MaSanPham { get; set; }
    public decimal SoLuong { get; set; }
    public string? DonViTinh { get; set; }
    public decimal DonGia { get; set; }
    public decimal? ThanhTien { get; set; }
}
