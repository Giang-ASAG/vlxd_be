using VLXD_API.DTOs.ChiTietPhieuNhap;

namespace VLXD_API.DTOs.PhieuNhapKho
{
    public class GetPhieuNhapKhoDto
    {
        public int MaPhieuNhap { get; set; }
        public int? MaNcc { get; set; }
        public string? TenNcc { get; set; }
        public int? MaKhoNhap { get; set; }
        public string? TenKho { get; set; }
        public int? MaNguoiLap { get; set; }
        public string? TenNgLap { get; set; }
        public DateTime? NgayNhap { get; set; }
        public decimal? TongTienNhap { get; set; }
        public decimal? DaThanhToanNcc { get; set; }
        public string? TrangThai { get; set; }
        public List<ChiTietPhieuNhapDto> sanPhamDtos { get; set; }
    }
}
