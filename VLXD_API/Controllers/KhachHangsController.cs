using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VLXD_API.Common;
using VLXD_API.DTOs.KhachHang;
using VLXD_API.Models;
using VLXD_API.Models;

namespace VLXD_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KhachHangsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public KhachHangsController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<KhachHangDto>>>> GetAll()
    {
        var entities = await _context.KhachHangs.AsNoTracking().ToListAsync();
        var dto = _mapper.Map<List<KhachHangDto>>(entities);

        return Ok(ApiResponse<IEnumerable<KhachHangDto>>.Ok(dto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<KhachHangDto>>> GetById(int id)
    {
        var entity = await _context.KhachHangs.AsNoTracking().FirstOrDefaultAsync(x => x.MaKhachHang == id);
        if (entity is null)
        {
            return NotFound(ApiResponse<KhachHangDto>.Fail("NOT_FOUND", "KhachHang not found."));
        }

        var dto = _mapper.Map<KhachHangDto>(entity);
        return Ok(ApiResponse<KhachHangDto>.Ok(dto));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<KhachHangDto>>> Create(KhachHangDto dto)
    {
        var entity = _mapper.Map<KhachHang>(dto);
        _context.KhachHangs.Add(entity);
        await _context.SaveChangesAsync();

        var result = _mapper.Map<KhachHangDto>(entity);
        return CreatedAtAction(nameof(GetById), new { id = entity.MaKhachHang }, ApiResponse<KhachHangDto>.Ok(result));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> Update(int id, KhachHangDto dto)
    {
        var entity = await _context.KhachHangs.FirstOrDefaultAsync(x => x.MaKhachHang == id);
        if (entity is null)
        {
            return NotFound(ApiResponse<string>.Fail("NOT_FOUND", "KhachHang not found."));
        }

        dto.Adapt(entity);
        entity.MaKhachHang = id;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<string>.Ok("Updated successfully."));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
    {
        var entity = await _context.KhachHangs.FirstOrDefaultAsync(x => x.MaKhachHang == id);
        if (entity is null)
        {
            return NotFound(ApiResponse<string>.Fail("NOT_FOUND", "KhachHang not found."));
        }

        _context.KhachHangs.Remove(entity);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<string>.Ok("Deleted successfully."));
    }

    /////// Thống kê khách hàng(danh sach)
    [HttpGet("thong-ke")]
    public async Task<ActionResult<ApiResponse<IEnumerable<KhachHangThongKeDto>>>> GetAllWithThongKe()
    {
        var result = await _context.KhachHangs
            .AsNoTracking()
            .Select(kh => new KhachHangThongKeDto
            {
                MaKhachHang = kh.MaKhachHang,
                HoTen = kh.HoTen,
                SoDienThoai = kh.SoDienThoai,
                DiaChi = kh.DiaChi,
                HanMucNo = kh.HanMucNo,
                SoLuongDonHang = kh.DonHangs.Count(),
                TongTienDonHang = kh.DonHangs
                                    .Sum(dh => (decimal?)dh.TongTien) ?? 0,
                TongTienDaTra = kh.DonHangs
                                    .Sum(dh => (decimal?)dh.SoTienTra) ?? 0
            })
            .ToListAsync();

        return Ok(ApiResponse<IEnumerable<KhachHangThongKeDto>>.Ok(result));
    }
    //////// Thống kê khách hàng(theo id)
    [HttpGet("{id}/thong-ke")]
    public async Task<ActionResult<ApiResponse<KhachHangThongKeDto>>> GetByIdWithThongKe(int id)
    {
        var result = await _context.KhachHangs
            .AsNoTracking()
            .Where(kh => kh.MaKhachHang == id)
            .Select(kh => new KhachHangThongKeDto
            {
                MaKhachHang = kh.MaKhachHang,
                HoTen = kh.HoTen,
                SoDienThoai = kh.SoDienThoai,
                DiaChi = kh.DiaChi,
                HanMucNo = kh.HanMucNo,
                SoLuongDonHang = kh.DonHangs.Count(),
                TongTienDonHang = kh.DonHangs
                                    .Sum(dh => (decimal?)dh.TongTien) ?? 0,
                TongTienDaTra = kh.DonHangs
                                    .Sum(dh => (decimal?)dh.SoTienTra) ?? 0
            })
            .FirstOrDefaultAsync();

        if (result is null)
            return NotFound(ApiResponse<KhachHangThongKeDto>.Fail("NOT_FOUND", "KhachHang not found."));

        return Ok(ApiResponse<KhachHangThongKeDto>.Ok(result));
    }
}


