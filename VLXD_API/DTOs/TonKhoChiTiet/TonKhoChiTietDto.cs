using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VLXD_API.DTOs.TonKhoChiTiet
{
    public class TonKhoChiTietDto
    {
        public int MaKho { get; set; }

        public int MaSanPham { get; set; }

        public decimal? SoLuongTon { get; set; }
        public string? ViTriCuThe { get; set; }
    }
}
