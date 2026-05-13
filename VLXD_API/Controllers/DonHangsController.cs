using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VLXD_API.Common;
using VLXD_API.DTOs.DonHang;
using VLXD_API.DTOs.PhieuXuatKho;
using VLXD_API.Models;

namespace VLXD_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DonHangsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public DonHangsController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<DonHangDto>>>> GetAll()
    {
        var entities = await _context.DonHangs.AsNoTracking().ToListAsync();
        var dto = _mapper.Map<List<DonHangDto>>(entities);

        return Ok(ApiResponse<IEnumerable<DonHangDto>>.Ok(dto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<DonHangDto>>> GetById(int id)
    {
        var entity = await _context.DonHangs.AsNoTracking().FirstOrDefaultAsync(x => x.MaDonHang == id);
        if (entity is null)
        {
            return NotFound(ApiResponse<DonHangDto>.Fail("NOT_FOUND", "DonHang not found."));
        }

        var dto = _mapper.Map<DonHangDto>(entity);
        return Ok(ApiResponse<DonHangDto>.Ok(dto));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<DonHangDto>>> Create(DonHangDto dto)
    {
        var entity = _mapper.Map<DonHang>(dto);
        _context.DonHangs.Add(entity);
        await _context.SaveChangesAsync();

        var result = _mapper.Map<DonHangDto>(entity);
        return CreatedAtAction(nameof(GetById), new { id = entity.MaDonHang }, ApiResponse<DonHangDto>.Ok(result));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> Update(int id, DonHangDto dto)
    {
        var entity = await _context.DonHangs.FirstOrDefaultAsync(x => x.MaDonHang == id);
        if (entity is null)
        {
            return NotFound(ApiResponse<string>.Fail("NOT_FOUND", "DonHang not found."));
        }

        dto.Adapt(entity);
        entity.MaDonHang = id;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<string>.Ok("Updated successfully."));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
    {
        var entity = await _context.DonHangs.FirstOrDefaultAsync(x => x.MaDonHang == id);
        if (entity is null)
        {
            return NotFound(ApiResponse<string>.Fail("NOT_FOUND", "DonHang not found."));
        }

        _context.DonHangs.Remove(entity);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<string>.Ok("Deleted successfully."));
    }
    [HttpPost("themgiohang")]
    public async Task<ActionResult<ApiResponse<object>>> ThemGioHang(TaoDonHangRequest request)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var donHang = _mapper.Map<DonHang>(request.DonHang);
            donHang.NgayTao = DateTime.Now;
            donHang.TrangThaiThanhToan = donHang.SoTienTra <= 0 ? "chua_thanh_toan" :
                                         donHang.SoTienTra >= donHang.TongTien ? "da_thanh_toan" :
                                                                           "thanh_toan_mot_phan";
            await _context.DonHangs.AddAsync(donHang);
            await _context.SaveChangesAsync();

            var chiTiets = new List<ChiTietDonHang>();
            foreach (var item in request.ChiTietDonHangs)
            {
                var sanPham = _context.SanPhams.First(x => x.MaSanPham == item.MaSanPham);
                item.MaDonHang = donHang.MaDonHang;
                item.DonGia = (decimal)sanPham.GiaBanLe;
                if (sanPham.SoLuong > item.SoLuong)
                {
                    sanPham.SoLuong -= (int)item.SoLuong;
                }
                
                else
                {
                    var slton = _context.TonKhoChiTiets.First(x => x.MaSanPham == item.MaSanPham);
                    if(slton.SoLuongTon < item.SoLuong) throw new Exception($"Sản phẩm {sanPham.MaSanPham} không đủ hàng");
                    var slhientai = (int)item.SoLuong - sanPham.SoLuong;
                    sanPham.SoLuong = 0;
                    slton.SoLuongTon -= slhientai;
                    PhieuXuatKhoDto xuatKhoDto = new PhieuXuatKhoDto
                    {
                        MaDonHang = donHang.MaDonHang,
                        MaKhoXuat = 1,
                        MaNguoiXuat = 1,
                         NgayXuat = DateTime.Now,
                         TrangThaiXuat = "da_xuat_kho"
                    };
                    _context.PhieuXuatKhos.Add(_mapper.Map<PhieuXuatKho>(xuatKhoDto));
                    //await _context.SaveChangesAsync();
                }
                    chiTiets.Add(_mapper.Map<ChiTietDonHang>(item));
            }
            await _context.ChiTietDonHangs.AddRangeAsync(chiTiets);
            await _context.SaveChangesAsync();


            // 3. Công nợ
            var soTienNo = (decimal)(donHang.TongTien - donHang.SoTienTra);

            var congNo = new CongNoKhachHang
            {
                MaDonHang = donHang.MaDonHang,
                MaKhachHang = donHang.MaKhachHang,
                NgayPhatSinh = DateTime.Now,
                SoTienNo = soTienNo,
                TrangThai = soTienNo > 0 ? "dang_no" : "hoan_tat"
            };
            await _context.CongNoKhachHangs.AddAsync(congNo);
            await _context.SaveChangesAsync(); // sinh Id tại đây
            if (donHang.SoTienTra > 0)
            {
                var lichSuThanhToan = new LichSuThanhToan
                {
                    conNoID = congNo.Id,
                    IsNhaCungCap = false,
                    GhiChu = donHang.TrangThaiThanhToan,
                    NgayThanhToan = DateTime.Now,
                    PhuongThucThanhToan = (bool)donHang.HinhThuc,
                    SoTien = (decimal)donHang.SoTienTra,
                };
                await _context.LichSuThanhToans.AddAsync(lichSuThanhToan);
            }
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return Ok(ApiResponse<IEnumerable<string>>.Succes("Khong loi", "Tạo don hang thành công"));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return BadRequest(new ApiResponse<object> { Success = false });
        }
    }

    [HttpGet("HoaDon")]
    public async Task<ActionResult<ApiResponse<object>>> getHoaDon() 
    {
        var hoadon = _context.DonHangs.Select(x => new HoaDonDto
        {
            MaDonHang = x.MaDonHang,
            HinhThuc = x.HinhThuc,
            MaKhachHang = x.MaKhachHang,
            NgayTao = x.NgayTao,
            TenKhachHang = _context.KhachHangs.First(t=>t.MaKhachHang==x.MaKhachHang).HoTen,
            TongTien = x.TongTien, 
            khachDaTra = x.SoTienTra,
            TrangThaiThanhToan = x.TrangThaiThanhToan,
            MaNguoiTao = x.MaNguoiTao,
            TenNguoiTao = _context.NguoiDungs.First(q=>q.MaNguoiDung==x.MaNguoiTao).HoTen
        }).ToList();
        return Ok(ApiResponse<IEnumerable<HoaDonDto>>.Ok(hoadon));
    }
}
