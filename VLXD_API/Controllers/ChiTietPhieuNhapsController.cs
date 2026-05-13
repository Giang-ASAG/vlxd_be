using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VLXD_API.Common;
using VLXD_API.DTOs.ChiTietPhieuNhap;
using VLXD_API.Models;

namespace VLXD_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChiTietPhieuNhapsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ChiTietPhieuNhapsController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ChiTietPhieuNhapDto>>>> GetAll()
    {
        var entities = await _context.ChiTietPhieuNhaps.AsNoTracking().ToListAsync();
        var dto = _mapper.Map<List<ChiTietPhieuNhapDto>>(entities);

        return Ok(ApiResponse<IEnumerable<ChiTietPhieuNhapDto>>.Ok(dto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ChiTietPhieuNhapDto>>> GetById(int id)
    {
        var entity = await _context.ChiTietPhieuNhaps.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            return NotFound(ApiResponse<ChiTietPhieuNhapDto>.Fail("NOT_FOUND", "ChiTietPhieuNhap not found."));
        }

        var dto = _mapper.Map<ChiTietPhieuNhapDto>(entity);
        return Ok(ApiResponse<ChiTietPhieuNhapDto>.Ok(dto));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ChiTietPhieuNhapDto>>> Create(ChiTietPhieuNhapDto dto)
    {
        var entity = _mapper.Map<ChiTietPhieuNhap>(dto);
        _context.ChiTietPhieuNhaps.Add(entity);
        await _context.SaveChangesAsync();

        var result = _mapper.Map<ChiTietPhieuNhapDto>(entity);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, ApiResponse<ChiTietPhieuNhapDto>.Ok(result));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> Update(int id, ChiTietPhieuNhapDto dto)
    {
        var entity = await _context.ChiTietPhieuNhaps.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            return NotFound(ApiResponse<string>.Fail("NOT_FOUND", "ChiTietPhieuNhap not found."));
        }

        dto.Adapt(entity);
        entity.Id = id;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<string>.Ok("Updated successfully."));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
    {
        var entity = await _context.ChiTietPhieuNhaps.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            return NotFound(ApiResponse<string>.Fail("NOT_FOUND", "ChiTietPhieuNhap not found."));
        }

        _context.ChiTietPhieuNhaps.Remove(entity);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<string>.Ok("Deleted successfully."));
    }
}
