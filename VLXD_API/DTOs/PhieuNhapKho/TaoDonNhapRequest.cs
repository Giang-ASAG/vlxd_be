using VLXD_API.DTOs.ChiTietPhieuNhap;
using VLXD_API.DTOs.SanPham;

namespace VLXD_API.DTOs.PhieuNhapKho
{
    public class TaoDonNhapRequest
    {
        public PhieuNhapKhoDto phieuNhapKho { get; set; }
        public List<ChiTietPhieuNhapDto> chiTietPhieuNhapKhos { get; set; }
        public List<SanPhamDto>? sanPhams { get; set; }

    }
}
