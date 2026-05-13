namespace VLXD_API.DTOs.KhachHang
{
    public class KhachHangThongKeDto
    {
        public int MaKhachHang { get; set; }
        public string HoTen { get; set; } = null!;
        public string? SoDienThoai { get; set; }
        public string? DiaChi { get; set; }
        public decimal? HanMucNo { get; set; }

        // Thống kê đơn hàng
        public int SoLuongDonHang { get; set; }
        public decimal TongTienDonHang { get; set; }
        public decimal TongTienDaTra { get; set; }
        public decimal TongConNo => TongTienDonHang - TongTienDaTra;
    }
}
