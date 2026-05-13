using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VLXD_API.Common;
using VLXD_API.DTOs.NguoiDung;
using VLXD_API.Handlers;
using VLXD_API.Models;
using VLXD_API.Models;

namespace VLXD_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NguoiDungsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public NguoiDungsController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<NguoiDungDto>>>> GetAll()
    {
        var entities = await _context.NguoiDungs.AsNoTracking().ToListAsync();
        var dto = _mapper.Map<List<NguoiDungDto>>(entities);

        return Ok(ApiResponse<IEnumerable<NguoiDungDto>>.Ok(dto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<NguoiDungDto>>> GetById(int id)
    {
        var entity = await _context.NguoiDungs.AsNoTracking().FirstOrDefaultAsync(x => x.MaNguoiDung == id);
        if (entity is null)
        {
            return NotFound(ApiResponse<NguoiDungDto>.Fail("NOT_FOUND", "NguoiDung not found."));
        }

        var dto = _mapper.Map<NguoiDungDto>(entity);
        return Ok(ApiResponse<NguoiDungDto>.Ok(dto));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<NguoiDungDto>>> Create(NguoiDungDto dto)
    {
        var entity = _mapper.Map<NguoiDung>(dto);
        if(!string.IsNullOrEmpty(dto.MatKhauHash))
        {
            // Hash the password before saving
            entity.MatKhauHash = BamMatKhauHandler.PasswordHash(dto.MatKhauHash);
        }
        _context.NguoiDungs.Add(entity);
        await _context.SaveChangesAsync();

        var result = _mapper.Map<NguoiDungDto>(entity);
        return CreatedAtAction(nameof(GetById), new { id = entity.MaNguoiDung }, ApiResponse<NguoiDungDto>.Ok(result));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> Update(int id, NguoiDungDto dto)
    {
        var entity = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.MaNguoiDung == id);
        if (entity is null)
        {
            return NotFound(ApiResponse<string>.Fail("NOT_FOUND", "NguoiDung not found."));
        }

        dto.Adapt(entity);
        entity.MaNguoiDung = id;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<string>.Ok("Updated successfully."));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
    {
        var entity = await _context.NguoiDungs.FirstOrDefaultAsync(x => x.MaNguoiDung == id);
        if (entity is null)
        {
            return NotFound(ApiResponse<string>.Fail("NOT_FOUND", "NguoiDung not found."));
        }

        _context.NguoiDungs.Remove(entity);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<string>.Ok("Deleted successfully."));
    }
}


