using VLXD_API.DTOs.SanPham;

public class NhapThemHangDto
{
    public int MaNhaCungCap { get; set; }
    public int MaSanPham { get; set; }
    public SanPhamDto SanPham { get; set; } // Chứa TenSanPham, GiaBan... từ UI
    public double SoLuongNhap { get; set; } // Giá trị từ ô "Số lượng"
    public double TonKhoNhap { get; set; }  // Giá trị từ ô "Tồn kho"
    public decimal DonGiaNhap { get; set; }
    public int? MaKho { get; set; }
    public int MaNguoiDung { get; set; }
    public decimal SoTienThanhToanNgay { get; set; }
}