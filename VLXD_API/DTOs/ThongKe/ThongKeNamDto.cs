namespace VLXD_API.DTOs
{
    public class ThongKeNamDto
    {
        public int Nam { get; set; }

        public decimal TongDoanhThu { get; set; }
        public int TongDonHang { get; set; }
        public int TongSanPhamBan { get; set; }
        public List<ThongKeThangDto> DanhSachThang { get; set; } = new();
    }
}
