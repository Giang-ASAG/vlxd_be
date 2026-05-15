using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VLXD_API.Common;
using VLXD_API.DTOs.CongNoNcc;
using VLXD_API.DTOs.PhieuNhapKho;
using VLXD_API.Models;

namespace VLXD_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CongNoNccsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public CongNoNccsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

        }
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<CongNoNccDto>>>> GetAll()
        {
            var entities = await _context.CongNoNccs
                .AsNoTracking()
                .ToListAsync();

            var dto = _mapper.Map<List<CongNoNccDto>>(entities);

            // Lấy toàn bộ mã phiếu nhập cần dùng
            var maPhieuNhaps = dto
                .Select(x => x.MaPhieuNhap)
                .Distinct()
                .ToList();

            // Query 1 lần
            var phieuNhapDict = await _context.PhieuNhapKhos
                .AsNoTracking()
                .Where(x => maPhieuNhaps.Contains(x.MaPhieuNhap))
                .ToDictionaryAsync(x => x.MaPhieuNhap);

            // Gán dữ liệu từ RAM
            foreach (var item in dto)
            {
                if (phieuNhapDict.TryGetValue((int)item.MaPhieuNhap, out var phieuNhap))
                {
                    item.PhieuNhapKhoDto = _mapper.Map<PhieuNhapKhoDto>(phieuNhap);
                }
            }

            return Ok(ApiResponse<IEnumerable<CongNoNccDto>>.Ok(dto));
        }
        [HttpPost]
        public async Task<ActionResult<ApiResponse<CongNoNccDto>>> Create(CongNoNccDto dto)
        {
            var entity = _mapper.Map<CongNoNcc>(dto);
            _context.CongNoNccs.Add(entity);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<CongNoNccDto>(entity);
            return ApiResponse<CongNoNccDto>.Ok(result);
        }
        //[HttpPut("{id}")]
        //public async Task<ActionResult<ApiResponse<string>>> Update(int id, CongNoNccDto dto)
        //{
        //    var congNoTam = await _context.CongNoNccs.FindAsync(id);
        //    var pnk = await _context.PhieuNhapKhos.FindAsync(congNoTam.MaPhieuNhap);
        //    if (dto.SoTienNo)

        //        return Ok(ApiResponse<string>.Ok("Updated successfully."));
        //}

        [HttpGet("nha-cung-cap/{maNcc}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<object>>>> GetByNhaCungCap(int maNcc)
        {
            // Sử dụng LINQ Join để kết hợp dữ liệu từ các bảng: 
            // CongNoNccs, NhaCungCaps, PhieuNhapKhos, Khos, NguoiDungs
            var result = await (from cn in _context.CongNoNccs
                                join ncc in _context.NhaCungCaps on cn.MaNcc equals ncc.MaNcc
                                join pnk in _context.PhieuNhapKhos on cn.MaPhieuNhap equals pnk.MaPhieuNhap
                                join ctpn in _context.ChiTietPhieuNhaps on pnk.MaPhieuNhap equals ctpn.MaPhieuNhap
                                join k in _context.Khos on pnk.MaKhoNhap equals k.MaKho
                                join nd in _context.NguoiDungs on pnk.MaNguoiLap equals nd.MaNguoiDung
                                where cn.MaNcc == maNcc && cn.TrangThai == "dang_no"
                                select new
                                {
                                    IdCongNo = cn.Id,
                                    MaNhaCungCap = cn.MaNcc,
                                    MaPhieuNhap = cn.MaPhieuNhap,
                                    LoaiNhap = ctpn.LoaiNhap,
                                    SoTienNo = cn.SoTienNo,
                                    NgayPhatSinh = cn.NgayPhatSinh,
                                    TrangThaiNo = cn.TrangThai, // Trạng thái nợ (dang_no)
                                    MaKhoNhap = pnk.MaKhoNhap,
                                    TenKhoNhap = k.TenKho,
                                    MaNguoiLap = pnk.MaNguoiLap,
                                    TenNguoiLap = nd.HoTen,
                                    NgayNhapHang = pnk.NgayNhap,
                                    TongTienNhap = pnk.TongTienNhap,
                                    DaThanhToanNcc = pnk.DaThanhToanNcc,
                                    TrangThaiNhapHang = pnk.TrangThai // Trạng thái phiếu nhập (cho_nhap, da_nhap_kho...)
                                }).Distinct().ToListAsync();

            return Ok(ApiResponse<IEnumerable<object>>.Ok(result));
        }
    }
}
