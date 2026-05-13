namespace VLXD_API.DTOs
{
    public class ThongKeThangDto
    {
        public int Thang { get; set; }              // 1 -> 12
        public decimal TongDoanhThu { get; set; }  // Tổng doanh thu
        public int TongDonHang { get; set; }       // Tổng đơn hàng
        public int TongSanPhamBan { get; set; }    // Tổng sản phẩm bán
    }
}
