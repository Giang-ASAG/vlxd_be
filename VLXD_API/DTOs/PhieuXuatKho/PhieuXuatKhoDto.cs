namespace VLXD_API.DTOs.PhieuXuatKho
{
    public class PhieuXuatKhoDto
    {
        public int MaPhieuXuat { get; set; }
        public DateTime? NgayXuat { get; set; }
        public string? TrangThaiXuat { get; set; }

        // Don hang
        public int? MaDonHang { get; set; }

        // Kho
        public int? MaKhoXuat { get; set; }

        // Nguoi xuat
        public int? MaNguoiXuat { get; set; }

    }
}
