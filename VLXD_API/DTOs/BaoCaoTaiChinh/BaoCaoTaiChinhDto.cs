namespace VLXD_API.DTOs.BaoCaoTaiChinh
{
    public class BaoCaoTaiChinhDto
    {
        public List<KyBaoCao> Columns { get; set; } = new();

        // Danh sách các dòng chỉ tiêu (Cấu trúc cây)
        public List<DongTaiChinh> Rows { get; set; } = new();

    }
    public class KyBaoCao
    {
        public string Key { get; set; }   // Để map dữ liệu, ví dụ: "2024-01"
        public string Label { get; set; } // Hiển thị trên UI, ví dụ: "01/2024"
    }

    public class DongTaiChinh
    {
        public string Id { get; set; }          // Định danh: "revenue", "cogs"...
        public string Name { get; set; }        // Tên hiển thị: "Doanh thu", "Giá vốn"...
        public string Section { get; set; }     // Nhóm: "income", "expense"...    // Nếu true, UI sẽ in đậm dòng này

        // Dữ liệu số: Key là mã kỳ (2024-01), Value là số tiền
        public Dictionary<string, decimal> Values { get; set; } = new();

    }
}
