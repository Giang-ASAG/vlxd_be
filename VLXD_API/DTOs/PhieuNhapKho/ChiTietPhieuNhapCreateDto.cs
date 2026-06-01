namespace VLXD_API.DTOs.PhieuNhapKho
{
    public class ChiTietPhieuNhapCreateDto
    {
        public int MaSanPham { get; set; }
        public decimal SoLuong { get; set; } // Khớp kiểu decimal(18,2) trong DB mới
        public decimal GiaNhap { get; set; }
    }
}
