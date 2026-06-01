namespace VLXD_API.DTOs.PhieuNhapKho
{
    public class PhieuNhapCreateDto
    {
        public int MaNcc { get; set; }
        public int? MaKhoNhap { get; set; } = 1;
        public int? MaNguoiLap { get; set; }
        public DateTime NgayNhap { get; set; }
        public decimal SoTienThanhToanNgay { get; set; } // Ô nhập tiền thanh toán trên giao diện
        public string? GhiChu { get; set; }
        public List<ChiTietPhieuNhapCreateDto> ChiTiets { get; set; } = new List<ChiTietPhieuNhapCreateDto>();
    }
}
