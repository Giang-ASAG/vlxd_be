using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VLXD_API.Common;
using VLXD_API.DTOs.CongNoKhachHang;
using VLXD_API.Models;

namespace VLXD_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CongNoKhachHangController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CongNoKhachHangController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<CongNoKhachHangDto>>>> GetAll()
        {
            var entities = await _context.CongNoKhachHangs.AsNoTracking().ToListAsync();
            var dto = _mapper.Map<List<CongNoKhachHangDto>>(entities);

            return Ok(ApiResponse<IEnumerable<CongNoKhachHangDto>>.Ok(dto));
        }

        [HttpGet("khach-hang/{maKh}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<object>>>> GetByKhachHang(
        int maKh,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
        {
            // Sử dụng LINQ Join để làm phẳng dữ liệu
            var query = from cn in _context.CongNoKhachHangs
                        join kh in _context.KhachHangs on cn.MaKhachHang equals kh.MaKhachHang
                        join dh in _context.DonHangs on cn.MaDonHang equals dh.MaDonHang
                        join nd in _context.NguoiDungs on dh.MaNguoiTao equals nd.MaNguoiDung

                        // Left Join với PhieuXuatKho vì đơn hàng có thể chưa xuất kho hoặc bán tại cửa hàng
                        join pxk in _context.PhieuXuatKhos on dh.MaDonHang equals pxk.MaDonHang into pxkGroup
                        from pxk in pxkGroup.DefaultIfEmpty()

                            // Left Join tiếp với bảng Kho thông qua PhieuXuatKho
                        join k in _context.Khos on pxk.MaKhoXuat equals k.MaKho into khoGroup
                        from k in khoGroup.DefaultIfEmpty()

                        where cn.MaKhachHang == maKh && cn.TrangThai == "dang_no"
                        orderby cn.NgayPhatSinh descending
                        select new
                        {
                            IdCongNo = cn.Id,
                            MaKhachHang = cn.MaKhachHang,
                            TenKhachHang = kh.HoTen,
                            MaDonHang = cn.MaDonHang,
                            SoTienNo = cn.SoTienNo,
                            NgayPhatSinh = cn.NgayPhatSinh,
                            TrangThaiNo = cn.TrangThai,

                            // Logic: Nếu tìm thấy thông tin kho từ phiếu xuất thì hiện tên kho, ngược lại là Cửa hàng
                            MaKho = pxk != null ? pxk.MaKhoXuat : null,
                            TenKho = k != null ? k.TenKho : "Lấy tại cửa hàng",

                            MaNguoiTao = dh.MaNguoiTao,
                            TenNguoiTao = nd.HoTen,
                            NgayTaoDon = dh.NgayTao,
                            TongTienDonHang = dh.TongTien,
                            SoTienDaTra = dh.SoTienTra,
                            TrangThaiThanhToan = dh.TrangThaiThanhToan
                        };

            var result = await query
                .AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<object>>.Ok(result));
        }

    }
}
