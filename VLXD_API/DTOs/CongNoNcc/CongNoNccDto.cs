using VLXD_API.DTOs.PhieuNhapKho;

namespace VLXD_API.DTOs.CongNoNcc;

public class CongNoNccDto
{
    public int Id { get; set; }
    public int? MaNcc { get; set; }
    public int? MaPhieuNhap { get; set; }
    public decimal SoTienNo { get; set; }
    public DateTime? NgayPhatSinh { get; set; }
    public string? TrangThai { get; set; }

    public PhieuNhapKhoDto PhieuNhapKhoDto { get; set; }
}
