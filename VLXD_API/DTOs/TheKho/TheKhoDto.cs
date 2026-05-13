using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VLXD_API.DTOs.TheKho
{
    public class TheKhoDto
    {
        public int Id { get; set; }
        public int? MaSanPham { get; set; }
        public int? MaKho { get; set; }
 
        public DateTime? NgayThayDoi { get; set; }
    
        public string LoaiGiaoDich { get; set; } = null!;

        public decimal SoLuongThayDoi { get; set; }

        public decimal SoLuongTonSauKhiThayDoi { get; set; }

        public string? MaChungTuLienQuan { get; set; }
    }
}
