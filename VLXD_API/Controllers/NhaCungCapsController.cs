using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VLXD_API.Common;
using VLXD_API.DTOs.NhaCungCap;
using VLXD_API.Models;
using VLXD_API.Models;

namespace VLXD_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NhaCungCapsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public NhaCungCapsController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<NhaCungCapDto>>>> GetAll()
    {
        var entities = await _context.NhaCungCaps.AsNoTracking().ToListAsync();
        var dto = _mapper.Map<List<NhaCungCapDto>>(entities);

        return Ok(ApiResponse<IEnumerable<NhaCungCapDto>>.Ok(dto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<NhaCungCapDto>>> GetById(int id)
    {
        var entity = await _context.NhaCungCaps.AsNoTracking().FirstOrDefaultAsync(x => x.MaNcc == id);
        if (entity is null)
        {
            return NotFound(ApiResponse<NhaCungCapDto>.Fail("NOT_FOUND", "NhaCungCap not found."));
        }

        var dto = _mapper.Map<NhaCungCapDto>(entity);
        return Ok(ApiResponse<NhaCungCapDto>.Ok(dto));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<NhaCungCapDto>>> Create(NhaCungCapDto dto)
    {
        var entity = _mapper.Map<NhaCungCap>(dto);
        _context.NhaCungCaps.Add(entity);
        await _context.SaveChangesAsync();

        var result = _mapper.Map<NhaCungCapDto>(entity);
        return CreatedAtAction(nameof(GetById), new { id = entity.MaNcc }, ApiResponse<NhaCungCapDto>.Ok(result));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> Update(int id, NhaCungCapDto dto)
    {
        var entity = await _context.NhaCungCaps.FirstOrDefaultAsync(x => x.MaNcc == id);
        if (entity is null)
        {
            return NotFound(ApiResponse<string>.Fail("NOT_FOUND", "NhaCungCap not found."));
        }

        dto.Adapt(entity);
        entity.MaNcc = id;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<string>.Ok("Updated successfully."));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
    {
        var entity = await _context.NhaCungCaps.FirstOrDefaultAsync(x => x.MaNcc == id);
        if (entity is null)
        {
            return NotFound(ApiResponse<string>.Fail("NOT_FOUND", "NhaCungCap not found."));
        }

        _context.NhaCungCaps.Remove(entity);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<string>.Ok("Deleted successfully."));
    }
    [HttpGet("danhsachthongkencc")]
    public async Task<ActionResult<ApiResponse<IEnumerable<DSTKNccDto>>>> GetSummary()
    {
        var summary = await _context.NhaCungCaps
            .AsNoTracking()
            .Select(ncc => new DSTKNccDto
            {
                MaNcc = ncc.MaNcc,
                TenNcc = ncc.TenNcc,
                SoDienThoai = ncc.SoDienThoai,
                Email = ncc.Email,
                DiaChi = ncc.DiaChi,

                // Thống kê từ bảng PhieuNhapKho
                SoDonNhap = _context.PhieuNhapKhos.Count(p => p.MaNcc == ncc.MaNcc),

                TongTienNhap = _context.PhieuNhapKhos
                    .Where(p => p.MaNcc == ncc.MaNcc)
                    .Sum(p => p.TongTienNhap ?? 0),

                DaThanhToan = _context.PhieuNhapKhos
                    .Where(p => p.MaNcc == ncc.MaNcc)
                    .Sum(p => p.DaThanhToanNcc ?? 0)
            })
            .ToListAsync();

        return Ok(ApiResponse<IEnumerable<DSTKNccDto>>.Ok(summary));
    }
}


