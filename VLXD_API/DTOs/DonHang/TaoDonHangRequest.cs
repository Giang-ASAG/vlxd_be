using VLXD_API.DTOs.ChiTietDonHang;

namespace VLXD_API.DTOs.DonHang
{
    public class TaoDonHangRequest
    {
        public DonHangDto DonHang { get; set; } = null!;
        public List<ChiTietDonHangDto> ChiTietDonHangs { get; set; }
        //public decimal? SoTienTra { get; set; }
    }
}
