namespace VLXD_API.DTOs
{
    public class DonHangGanDayDto
    {
        public int MaDonHang { get; set; }
        public string TenKhachHang { get; set; } = null!;
        public string TrangThaiThanhToan { get; set; } = null!;
        public decimal TongTien { get; set; }
        public string ThoiGianHienThi { get; set; } = null!;
    }
}
