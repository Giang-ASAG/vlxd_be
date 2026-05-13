using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VLXD_API.Common;
using VLXD_API.DTOs.SanPham;
using VLXD_API.Models;
using VLXD_API.Models;

namespace VLXD_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SanPhamsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public SanPhamsController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<SanPhamTonKhoDto>>>> GetAll()
    {
        var entities = await _context.SanPhams
            .AsNoTracking()
            .ToListAsync();

        // Query stock details once
        var tonKhoBySanPham = await _context.TonKhoChiTiets
            .AsNoTracking()
            .GroupBy(x => x.MaSanPham)
            .Select(g => new
            {
                MaSanPham = g.Key,
                SoLuongTon = g.Sum(x => x.SoLuongTon)
            })
            .ToDictionaryAsync(x => x.MaSanPham, x => x.SoLuongTon);

        var dto = _mapper.Map<List<SanPhamTonKhoDto>>(entities);

        foreach (var item in dto)
        {
            if (tonKhoBySanPham.TryGetValue(item.MaSanPham, out var soLuongTon))
                item.TonKhoHienTai = (int)soLuongTon;
            else
                item.TonKhoHienTai = 0; // or whatever default you want
        }

        return Ok(ApiResponse<IEnumerable<SanPhamTonKhoDto>>.Ok(dto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<SanPhamDto>>> GetById(int id)
    {
        var entity = await _context.SanPhams.AsNoTracking().FirstOrDefaultAsync(x => x.MaSanPham == id);
        if (entity is null)
        {
            return NotFound(ApiResponse<SanPhamDto>.Fail("NOT_FOUND", "SanPham not found."));
        }

        var dto = _mapper.Map<SanPhamDto>(entity);
        return Ok(ApiResponse<SanPhamDto>.Ok(dto));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> Create(SanPhamTonKhoDto dto)
    {
        // 1) Map & lưu SanPham
        if (await _context.SanPhams.AnyAsync(x => x.MaSku == dto.MaSku))
            return BadRequest(ApiResponse<string>.Fail("Fail", "Trùng mã"));
        var entity = _mapper.Map<SanPham>(dto);
        
        _context.SanPhams.Add(entity);

        // 2) Tạo và lưu PhieuNhapKho (không hard-code MaPhieuNhap = 1)
        var phieuNhapKho = new PhieuNhapKho
        {
            MaKhoNhap = 1,
            MaNcc = dto.MaNccMacDinh,
            MaNguoiLap = 1,
            //DaThanhToanNcc = dto.ThanhToanNcc,
            NgayNhap = DateTime.Now,
            TongTienNhap = 0,
            TrangThai = "da_nhap_kho"
            // MaPhieuNhap để identity trong DB
        };

        _context.PhieuNhapKhos.Add(phieuNhapKho);
        await _context.SaveChangesAsync(); // phải Save để có phieuNhapKho.MaPhieuNhap

        // 3) Lập danh sách chi tiết theo điều kiện
        var chiTietDons = new List<ChiTietPhieuNhap>();

        if (dto.SoLuong > 0)
        {
            chiTietDons.Add(new ChiTietPhieuNhap
            {
                SoLuong = dto.SoLuong,
                MaSanPham = entity.MaSanPham,
                MaPhieuNhap = phieuNhapKho.MaPhieuNhap,
                GiaNhap = (decimal)dto.GiaNhapGanNhat,
                LoaiNhap = false // Nhập sản phẩm
            });
        }

        if (dto.TonKhoHienTai > 0)
        {
            await _context.TonKhoChiTiets.AddAsync(new TonKhoChiTiet
            {
                MaKho = 1,
                MaSanPham = entity.MaSanPham,
                SoLuongTon = dto.TonKhoHienTai,
                ViTriCuThe = "Nha kho"
            });
            await _context.SaveChangesAsync();
            chiTietDons.Add(new ChiTietPhieuNhap
            {
                SoLuong = dto.TonKhoHienTai,
                MaSanPham = entity.MaSanPham,
                MaPhieuNhap = phieuNhapKho.MaPhieuNhap,
                GiaNhap = (decimal)dto.GiaNhapGanNhat,
                LoaiNhap = true // Nhập kho
            });
        }

        if (!chiTietDons.Any())
            return BadRequest(ApiResponse<string>.Fail("Fail", "SoLuong và TonKhoHienTai đều <= 0."));

        // 4) Lưu chi tiết
        await _context.ChiTietPhieuNhaps.AddRangeAsync(chiTietDons);
      
        await _context.SaveChangesAsync();

        // 5) Tính TongTienNhap và update
        // Nếu ThanhTien là field computed/trigger thì phải đảm bảo nó được set đúng.
        var tongTien = chiTietDons.Sum(x => x.ThanhTien);
        phieuNhapKho.TongTienNhap = tongTien;

        CongNoNcc congNo = new CongNoNcc
        {
            MaNcc = phieuNhapKho.MaNcc,
            MaPhieuNhap = phieuNhapKho.MaPhieuNhap,
            NgayPhatSinh = DateTime.Now,
            SoTienNo = (decimal)phieuNhapKho.TongTienNhap,
            TrangThai = "dang_no"
        };
        await _context.CongNoNccs.AddAsync(congNo);

        await _context.SaveChangesAsync();

        return Ok(ApiResponse<string>.Succes("Success", "Add Success"));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> Update(
    int id,
    [FromBody] SanPhamTonKhoDto request)
    {
        if (request == null)
            return BadRequest("Dữ liệu không hợp lệ.");

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var sp = await _context.SanPhams
                .FirstOrDefaultAsync(x => x.MaSanPham == id);

            if (sp == null)
                return NotFound("Sản phẩm không tồn tại.");

            // =========================
            // UPDATE THÔNG TIN SẢN PHẨM
            // =========================
            sp.TenSanPham = request.TenSanPham;
         //   sp.MaSku = request.MaSku;
            sp.GiaBanLe = request.GiaBanLe;
            sp.Thue = request.Thue;
            sp.GiaSauThue = request.GiaSauThue;
            sp.MaDanhMuc = request.MaDanhMuc;
            sp.DonViChinh = request.DonViChinh;
            sp.MaNccMacDinh = request.MaNccMacDinh;


            decimal soLuongCuaHang = request.SoLuong;
            decimal soLuongKho = request.TonKhoHienTai;
            decimal tongSoLuongNhap = soLuongCuaHang + soLuongKho;
            decimal giaNhap = request.GiaNhapGanNhat ?? 0;

            // =========================
            // NHẬP HÀNG
            // =========================
            if (tongSoLuongNhap > 0)
            {
                decimal tongTienNhap = tongSoLuongNhap * giaNhap;

                var phieuNhap = new PhieuNhapKho
                {
                    MaNcc = request.MaNccMacDinh,
                    MaKhoNhap = 1,
                    MaNguoiLap = 1,
                    NgayNhap = DateTime.Now,
                    TongTienNhap = tongTienNhap,
                    DaThanhToanNcc = 0,
                    TrangThai = "da_nhap_kho"
                };

                _context.PhieuNhapKhos.Add(phieuNhap);

                // Flush để lấy MaPhieuNhap
                await _context.SaveChangesAsync();

                var chiTietPhieuNhaps = new List<ChiTietPhieuNhap>();

                // =========================
                // NHẬP CỬA HÀNG
                // =========================
                if (soLuongCuaHang > 0)
                {
                    sp.SoLuong = (int)soLuongCuaHang;

                    chiTietPhieuNhaps.Add(new ChiTietPhieuNhap
                    {
                        MaPhieuNhap = phieuNhap.MaPhieuNhap,
                        MaSanPham = sp.MaSanPham,
                        SoLuong = soLuongCuaHang,
                        GiaNhap = giaNhap,
                        LoaiNhap = false
                    });
                }

                // =========================
                // NHẬP KHO
                // =========================
                if (soLuongKho > 0)
                {
                    var tonKho = await _context.TonKhoChiTiets
                        .FirstOrDefaultAsync(x => x.MaSanPham == id);

                    if (tonKho == null)
                    {
                        tonKho = new TonKhoChiTiet
                        {
                            MaSanPham = id,
                            SoLuongTon = 0
                        };

                        _context.TonKhoChiTiets.Add(tonKho);
                    }

                    tonKho.SoLuongTon = soLuongKho;

                    chiTietPhieuNhaps.Add(new ChiTietPhieuNhap
                    {
                        MaPhieuNhap = phieuNhap.MaPhieuNhap,
                        MaSanPham = sp.MaSanPham,
                        SoLuong = soLuongKho,
                        GiaNhap = giaNhap,
                        LoaiNhap = true
                    });
                }

                _context.ChiTietPhieuNhaps.AddRange(chiTietPhieuNhaps);

                // =========================
                // CÔNG NỢ NCC
                // =========================
                _context.CongNoNccs.Add(new CongNoNcc
                {
                    MaNcc = request.MaNccMacDinh,
                    MaPhieuNhap = phieuNhap.MaPhieuNhap,
                    SoTienNo = tongTienNhap,
                    NgayPhatSinh = DateTime.Now,
                    TrangThai = "dang_no"
                });
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new
            {
                success = true,
                message = "Cập nhật thành công!"
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
    {
        var entity = await _context.SanPhams
            .FirstOrDefaultAsync(x => x.MaSanPham == id);

        if (entity == null)
        {
            return NotFound(
                ApiResponse<string>.Fail("NOT_FOUND", "SanPham not found.")
            );
        }

        var chitietPN = await _context.ChiTietPhieuNhaps
            .Where(x => x.MaSanPham == id)
            .ToListAsync();

        var tonKhoCT = await _context.TonKhoChiTiets
            .Where(x => x.MaSanPham == id)
            .ToListAsync();

        // Xóa dữ liệu liên quan
        if (chitietPN.Any())
            _context.ChiTietPhieuNhaps.RemoveRange(chitietPN);

        if (tonKhoCT.Any())
            _context.TonKhoChiTiets.RemoveRange(tonKhoCT);

        // Xóa sản phẩm
        _context.SanPhams.Remove(entity);

        await _context.SaveChangesAsync();

        return Ok(ApiResponse<string>.Ok("Deleted successfully."));
    }
}


