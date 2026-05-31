namespace VLXD_API.DTOs.PhieuNhapKho
{
    public class PhieuNhapCreateDto
    {
       public int MaNcc { get; set; }
        public int? MaKhoNhap { get; set; } = 1; // Mặc định kho 1 nếu client không truyền
        public int? MaNguoiLap { get; set; }
        public DateTime NgayNhap { get; set; } = DateTime.Now;
        public List<ChiTietPhieuNhapCreateDto> ChiTiets { get; set; } = new List<ChiTietPhieuNhapCreateDto>();
    }
}
