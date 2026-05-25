public class CreateNccWithProductsDto
{
    // Thông tin nhà cung cấp mới
    public string TenNcc { get; set; }
    public string SoDienThoai { get; set; }
    public string DiaChi { get; set; }
    public string Email { get; set; }
    public string GhiChu { get; set; }

    // Thông tin phụ cho phiếu nhập kho
    //public int MaKhoNhap { get; set; } // Kho chứa hàng
    public int MaNguoiLap { get; set; } // Nhân viên bấm máy

    // Danh sách các ID sản phẩm được chọn từ giao diện (những SP đang có MaNcc = null)
    public List<int> SelectedProductIds { get; set; }
}