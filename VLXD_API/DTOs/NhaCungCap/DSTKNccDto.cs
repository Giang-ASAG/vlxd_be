namespace VLXD_API.DTOs.NhaCungCap
{
    public class DSTKNccDto
    {
        public int MaNcc { get; set; }
        public string TenNcc { get; set; } = null!;
        public string? SoDienThoai { get; set; }
        public string? Email { get; set; }
        public string? DiaChi { get; set; }

        // Các trường thống kê cho giao diện
        public int SoDonNhap { get; set; }
        public decimal TongTienNhap { get; set; }
        public decimal DaThanhToan { get; set; }
        public decimal ConNo => TongTienNhap - DaThanhToan;

    }
}
