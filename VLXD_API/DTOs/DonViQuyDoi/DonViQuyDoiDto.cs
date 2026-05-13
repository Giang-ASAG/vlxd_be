namespace VLXD_API.DTOs.DonViQuyDoi
{
    public class DonViQuyDoiDto
    {
        public int Id { get; set; }
        public int? MaSanPham { get; set; }
        public string TenDonVi { get; set; } = null!;
        public decimal TyLeQuyDoi { get; set; }
        public decimal? GiaBanTheoDv { get; set; }
    }
}
