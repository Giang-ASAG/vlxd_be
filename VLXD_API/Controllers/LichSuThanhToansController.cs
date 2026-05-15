using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using VLXD_API.Common;
using VLXD_API.DTOs.DonHang;
using VLXD_API.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace VLXD_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LichSuThanhToansController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public LichSuThanhToansController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<LichSuThanhToan>>>> GetAll()
        {
            var entities = await _context.LichSuThanhToans.ToListAsync();
            return Ok(ApiResponse<IEnumerable<LichSuThanhToan>>.Ok(entities));
        }
        [HttpGet("{maNcc}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<object>>>> GetLichSuThanhToanNcc(
    int maNcc,
    bool isNcc)
        {
            if (maNcc <= 0)
            {
                return BadRequest(
                    ApiResponse<IEnumerable<object>>
                    .Fail("400", "Mã nhà cung cấp không hợp lệ"));
            }

            IQueryable<object> query;

            if (isNcc)
            {
                query =
                    from ls in _context.LichSuThanhToans
                    join cn in _context.CongNoNccs
                        on ls.conNoID equals cn.Id
                    join pnk in _context.PhieuNhapKhos
                        on cn.MaPhieuNhap equals pnk.MaPhieuNhap
                    join k in _context.Khos
                        on pnk.MaKhoNhap equals k.MaKho into khoGroup
                    from k in khoGroup.DefaultIfEmpty()

                    where cn.MaNcc == maNcc
                          && ls.IsNhaCungCap == true

                    orderby ls.NgayThanhToan descending

                    select new
                    {
                        MaPhieu = pnk.MaPhieuNhap,
                        NgayPhatSinh = ls.NgayThanhToan,
                        KhoNhap = k != null ? k.TenKho : "Không xác định",
                        TongTienNhap = pnk.TongTienNhap,

                        SoTienThanhToan = ls.SoTien,

                        TongDaThanhToan = pnk.DaThanhToanNcc,

                        ConNo = cn.SoTienNo,

                        PhuongThucThanhToan =
                            ls.PhuongThucThanhToan
                                ? "Chuyển khoản"
                                : "Tiền mặt",

                        GhiChu = ls.GhiChu
                    };
            }
            else
            {
                query =
                    from ls in _context.LichSuThanhToans
                    join cn in _context.CongNoKhachHangs
                        on ls.conNoID equals cn.Id
                    join dh in _context.DonHangs
                        on cn.MaDonHang equals dh.MaDonHang

                    where cn.MaKhachHang == maNcc
                          && ls.IsNhaCungCap == false

                    orderby ls.NgayThanhToan descending

                    select new
                    {
                        MaDonHang = dh.MaDonHang,
                        NgayPhatSinh = ls.NgayThanhToan,

                        TongTienNhap = dh.TongTien,

                        SoTienThanhToan = ls.SoTien,

                        TongDaThanhToan = dh.SoTienTra,

                        ConNo = cn.SoTienNo,

                        PhuongThucThanhToan =
                            ls.PhuongThucThanhToan
                                ? "Chuyển khoản"
                                : "Tiền mặt",

                        GhiChu = ls.GhiChu
                    };
            }

            var result = await query
                .AsNoTracking()
                .ToListAsync();

            return Ok(
                ApiResponse<IEnumerable<object>>
                .Ok(result));
        }
        [HttpPost]
        public async Task<ActionResult<ApiResponse<IEnumerable<string>>>> CreateList([FromBody] List<LichSuThanhToan> listRequest)
        {
            if (listRequest == null || !listRequest.Any())
            {
                return BadRequest(ApiResponse<string>.Fail("","Danh sách thanh toán trống"));
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var item in listRequest)
                {
                    if (item.IsNhaCungCap)
                    {
                        var congNoncc = await _context.CongNoNccs.FindAsync(item.conNoID);
                        if (congNoncc == null) continue; // or throw/return error

                        var phieunhap = await _context.PhieuNhapKhos.FindAsync(congNoncc.MaPhieuNhap);
                        if (phieunhap == null) continue;

                        var originalDebt = congNoncc.SoTienNo;

                        phieunhap.DaThanhToanNcc += item.SoTien;
                        congNoncc.SoTienNo -= item.SoTien;

                        if (item.SoTien >= originalDebt ||
                            phieunhap.DaThanhToanNcc >= phieunhap.TongTienNhap)
                        {
                            congNoncc.TrangThai = "hoan_tat";
                        }
                    }
                    else
                    {
                        var congNoKh = await _context.CongNoKhachHangs.FindAsync(item.conNoID);
                        if (congNoKh == null) continue;

                        var donhang = await _context.DonHangs.FindAsync(congNoKh.MaDonHang);
                        if (donhang == null) continue;

                        var originalDebt = congNoKh.SoTienNo;

                        donhang.SoTienTra += item.SoTien;
                        congNoKh.SoTienNo -= item.SoTien;

                        if (item.SoTien >= originalDebt ||
                            donhang.SoTienTra >= donhang.TongTien)
                        {
                            congNoKh.TrangThai = "hoan_tat";
                            donhang.TrangThaiThanhToan = "da_thanh_toan";
                        }
                    }
                    item.NgayThanhToan = DateTime.UtcNow.AddHours(7);
                }

                await _context.LichSuThanhToans.AddRangeAsync(listRequest);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(ApiResponse<IEnumerable<string>>.Succes("Khong loi", "Tạo lịch sử thanh toán thành công"));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Log error
                return StatusCode(500, ApiResponse<string>.Fail("Lỗi hệ thống: " , ex.Message));
            }
        }
    }
}
