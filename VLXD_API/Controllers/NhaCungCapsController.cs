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
    public async Task<ActionResult<ApiResponse<string>>> Create([FromBody] NhaCungCapDto dto)
    {
        if (dto is null)
            return BadRequest(ApiResponse<string>.Fail("400", "Dữ liệu không hợp lệ."));

        if (string.IsNullOrEmpty(dto.TenNcc))
            return BadRequest(ApiResponse<string>.Fail("400", "Tên nhà cung cấp không được để trống."));

        try
        {
            var nhaCungCapMoi = new NhaCungCap
            {
                TenNcc = dto.TenNcc,
                SoDienThoai = dto.SoDienThoai,
                Email = dto.Email,
                DiaChi = dto.DiaChi,
                GhiChu = dto.GhiChu
            };

            _context.NhaCungCaps.Add(nhaCungCapMoi);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<string>.Succes("200", "Thêm mới nhà cung cấp thành công."));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<string>.Fail("500", $"Lỗi khi tạo nhà cung cấp: {ex.Message}"));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> Update(int id, [FromBody] NhaCungCapDto dto)
    {
        if (dto is null)
            return BadRequest(ApiResponse<string>.Fail("400", "Dữ liệu không hợp lệ."));

        // Tìm nhà cung cấp theo id từ URL
        var nhaCungCap = await _context.NhaCungCaps.FirstOrDefaultAsync(x => x.MaNcc == id);
        if (nhaCungCap == null)
            return NotFound(ApiResponse<string>.Fail("404", "Không tìm thấy nhà cung cấp cần cập nhật."));

        if (string.IsNullOrEmpty(dto.TenNcc))
            return BadRequest(ApiResponse<string>.Fail("400", "Tên nhà cung cấp không được để trống."));

        // Chỉ cập nhật thông tin bảng nha_cung_cap
        nhaCungCap.TenNcc = dto.TenNcc;
        nhaCungCap.SoDienThoai = dto.SoDienThoai;
        nhaCungCap.DiaChi = dto.DiaChi;
        nhaCungCap.Email = dto.Email;
        nhaCungCap.GhiChu = dto.GhiChu;

        try
        {
            await _context.SaveChangesAsync();
            return Ok(ApiResponse<string>.Succes("200", "Cập nhật thông tin nhà cung cấp thành công."));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<string>.Fail("500", $"Lỗi khi cập nhật: {ex.Message}"));
        }
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


