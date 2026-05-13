namespace VLXD_API.DTOs.SanPham
{
    public class SanPhamTonKhoDto
    {
        public int MaSanPham { get; set; }
        public string MaSku { get; set; } = null!;
        public string TenSanPham { get; set; } = null!;
        public int? MaDanhMuc { get; set; }
        public int? MaNccMacDinh { get; set; }
        public string DonViChinh { get; set; } = null!;
        public decimal? GiaNhapGanNhat { get; set; }
        public decimal? GiaBanLe { get; set; }

        public decimal? Thue { get; set; }
        public decimal? GiaSauThue { get; set; }

        public int SoLuong { get; set; }
        public int TonKhoHienTai { get; set; }
        public int? TonKhoToiThieu { get; set; }
        public int? TonKhoToiDa { get; set; }
        //    public decimal? ThanhToanNcc { get; set; }
        public DateTime? NgayTao { get; set; }
    }
}
