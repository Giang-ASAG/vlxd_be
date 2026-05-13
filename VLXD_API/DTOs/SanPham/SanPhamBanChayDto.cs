namespace VLXD_API.DTOs.SanPham
{
    public class SanPhamBanChayDto
    {
        public int MaSanPham { get; set; }
        public string TenSanPham { get; set; } = null!;
        public int SoLuongDaBan { get; set; }
        public decimal TongDoanhThu { get; set; }
        public double TongTonKho { get; set; }
    }
}
