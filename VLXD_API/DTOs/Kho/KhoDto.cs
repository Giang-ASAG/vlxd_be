namespace VLXD_API.DTOs.Kho
{
    public class KhoDto
    {
        public int MaKho { get; set; }
        public string TenKho { get; set; } = null!;
        public string? DiaChi { get; set; }
        public string? SoDienThoaiKho { get; set; }
        public bool? TrangThai { get; set; }
    }
}
