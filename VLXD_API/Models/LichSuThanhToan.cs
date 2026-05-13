namespace VLXD_API.Models
{
    public class LichSuThanhToan
    {
        public int Id { get; set; }

        /// <summary>
        /// true = NCC, false = KhachHang
        /// </summary>
        public bool IsNhaCungCap { get; set; }

        public int conNoID { get; set; }

        public decimal SoTien { get; set; }
        /// <summary>
        /// false (0): Tiền mặt, true (1): Chuyển khoản
        /// </summary>
        public bool PhuongThucThanhToan { get; set; }

        public DateTime NgayThanhToan { get; set; }

        public string? GhiChu { get; set; }
    }
}
