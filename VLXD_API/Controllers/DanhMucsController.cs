using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VLXD_API.Common;
using VLXD_API.DTOs.DanhMuc;
using VLXD_API.Models;
using VLXD_API.Models;

namespace VLXD_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DanhMucsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public DanhMucsController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<DanhMucDto>>>> GetAll()
    {
        var entities = await _context.DanhMucs.AsNoTracking().ToListAsync();
        var dto = _mapper.Map<List<DanhMucDto>>(entities);

        return Ok(ApiResponse<IEnumerable<DanhMucDto>>.Ok(dto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<DanhMucDto>>> GetById(int id)
    {
        var entity = await _context.DanhMucs.AsNoTracking().FirstOrDefaultAsync(x => x.MaDanhMuc == id);
        if (entity is null)
        {
            return NotFound(ApiResponse<DanhMucDto>.Fail("NOT_FOUND", "DanhMuc not found."));
        }

        var dto = _mapper.Map<DanhMucDto>(entity);
        return Ok(ApiResponse<DanhMucDto>.Ok(dto));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<DanhMucDto>>> Create(DanhMucDto dto)
    {
        var entity = _mapper.Map<DanhMuc>(dto);
        _context.DanhMucs.Add(entity);
        await _context.SaveChangesAsync();

        var result = _mapper.Map<DanhMucDto>(entity);
        return CreatedAtAction(nameof(GetById), new { id = entity.MaDanhMuc }, ApiResponse<DanhMucDto>.Ok(result));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> Update(int id, DanhMucDto dto)
    {
        var entity = await _context.DanhMucs.FirstOrDefaultAsync(x => x.MaDanhMuc == id);
        if (entity is null)
        {
            return NotFound(ApiResponse<string>.Fail("NOT_FOUND", "DanhMuc not found."));
        }

        dto.Adapt(entity);
        entity.MaDanhMuc = id;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<string>.Ok("Updated successfully."));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
    {
        var entity = await _context.DanhMucs.FirstOrDefaultAsync(x => x.MaDanhMuc == id);
        if (entity is null)
        {
            return NotFound(ApiResponse<string>.Fail("NOT_FOUND", "DanhMuc not found."));
        }

        _context.DanhMucs.Remove(entity);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<string>.Ok("Deleted successfully."));
    }
}


