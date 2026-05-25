public class UpdateNccWithProductsDto
{
    public int MaNcc { get; set; } // <--- Bắt buộc phải có để xác định NCC cần cập nhật
    public string TenNcc { get; set; }
    public string SoDienThoai { get; set; }
    public string DiaChi { get; set; }
    public string Email { get; set; }
    public string GhiChu { get; set; }

    public int MaKhoNhap { get; set; }
    public int MaNguoiLap { get; set; }
    public List<int> SelectedProductIds { get; set; } // Danh sách sản phẩm mồ côi muốn gom thêm (nếu có)
}