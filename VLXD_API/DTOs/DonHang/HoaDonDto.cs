namespace VLXD_API.DTOs.DonHang
{
    public class HoaDonDto
    {
        public int MaDonHang { get; set; }
        public int? MaKhachHang { get; set; }
        public string? TenKhachHang { get; set; }
        public int? MaNguoiTao { get; set; }
        public string? TenNguoiTao { get; set; }

        public DateTime? NgayTao { get; set; }
        public decimal? TongTien { get; set; }
        public decimal? khachDaTra { get; set; }

        public string? TrangThaiThanhToan { get; set; }
        public bool? HinhThuc { get; set; }
        //public decimal? SoTienTra { get; set; }
    }
}
