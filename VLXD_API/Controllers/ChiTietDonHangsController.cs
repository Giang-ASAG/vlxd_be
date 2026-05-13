using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VLXD_API.Common;
using VLXD_API.DTOs.ChiTietDonHang;
using VLXD_API.Models;
using VLXD_API.Models;

namespace VLXD_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChiTietDonHangsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ChiTietDonHangsController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ChiTietDonHangDto>>>> GetAll()
    {
        var entities = await _context.ChiTietDonHangs.AsNoTracking().ToListAsync();
        var dto = _mapper.Map<List<ChiTietDonHangDto>>(entities);

        return Ok(ApiResponse<IEnumerable<ChiTietDonHangDto>>.Ok(dto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ChiTietDonHangDto>>> GetById(int id)
    {
        var entity = await _context.ChiTietDonHangs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            return NotFound(ApiResponse<ChiTietDonHangDto>.Fail("NOT_FOUND", "ChiTietDonHang not found."));
        }

        var dto = _mapper.Map<ChiTietDonHangDto>(entity);
        return Ok(ApiResponse<ChiTietDonHangDto>.Ok(dto));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ChiTietDonHangDto>>> Create(ChiTietDonHangDto dto)
    {
        var entity = _mapper.Map<ChiTietDonHang>(dto);
        _context.ChiTietDonHangs.Add(entity);
        await _context.SaveChangesAsync();

        var result = _mapper.Map<ChiTietDonHangDto>(entity);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, ApiResponse<ChiTietDonHangDto>.Ok(result));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> Update(int id, ChiTietDonHangDto dto)
    {
        var entity = await _context.ChiTietDonHangs.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            return NotFound(ApiResponse<string>.Fail("NOT_FOUND", "ChiTietDonHang not found."));
        }

        dto.Adapt(entity);
        entity.Id = id;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<string>.Ok("Updated successfully."));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
    {
        var entity = await _context.ChiTietDonHangs.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            return NotFound(ApiResponse<string>.Fail("NOT_FOUND", "ChiTietDonHang not found."));
        }

        _context.ChiTietDonHangs.Remove(entity);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<string>.Ok("Deleted successfully."));
    }
}


