using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VLXD_API.Common;
using VLXD_API.DTOs.ChiTietPhieuNhap;
using VLXD_API.DTOs.PhieuNhapKho;
using VLXD_API.Models;

namespace VLXD_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PhieuNhapKhosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public PhieuNhapKhosController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<GetPhieuNhapKhoDto>>>> GetAll()
    {
        var result = await _context.PhieuNhapKhos
            .AsNoTracking()
            .Join(_context.Khos,
                pn => pn.MaKhoNhap,
                kho => kho.MaKho,
                (pn, kho) => new { pn, kho })
            .Join(_context.NhaCungCaps,
                temp => temp.pn.MaNcc,
                ncc => ncc.MaNcc,
                (temp, ncc) => new { temp.pn, temp.kho, ncc })
            .Join(_context.NguoiDungs,
                temp => temp.pn.MaNguoiLap,
                nd => nd.MaNguoiDung,
                (temp, nd) => new GetPhieuNhapKhoDto
                {
                    MaPhieuNhap = temp.pn.MaPhieuNhap,
                    MaNcc = temp.pn.MaNcc,
                    MaKhoNhap = temp.pn.MaKhoNhap,
                    MaNguoiLap = temp.pn.MaNguoiLap,
                    NgayNhap = temp.pn.NgayNhap,
                    TongTienNhap = temp.pn.TongTienNhap,
                    DaThanhToanNcc = temp.pn.DaThanhToanNcc,
                    TrangThai = temp.pn.TrangThai,

                    TenKho = temp.kho.TenKho,
                    TenNcc = temp.ncc.TenNcc,
                    TenNgLap = nd.HoTen
                })
            .OrderByDescending(x => x.NgayNhap)
            .ToListAsync();

        foreach (var item in result)
        {
            var list = await _context.SanPhams
                .Join(
                    _context.ChiTietPhieuNhaps,
                    sp => sp.MaSanPham,
                    tk => tk.MaSanPham,
                    (sp, tk) => new
                    {
                        sp,
                        tk
                    }
                )
                .Where(x => x.tk.MaPhieuNhap == item.MaPhieuNhap)
                .ToListAsync();

            item.sanPhamDtos = list.Select(x => new ChiTietPhieuNhapDto
            {
                MaSanPham = x.tk.MaSanPham,
                SoLuong = x.tk.SoLuong,
                GiaNhap = x.tk.GiaNhap,
                TenSanPham = x.sp.TenSanPham,
                LoaiNhap = x.tk.LoaiNhap,
                MaPhieuNhap = x.tk.MaPhieuNhap,
                ThanhTien = x.tk.ThanhTien
            }).ToList();
        }

        return Ok(ApiResponse<IEnumerable<GetPhieuNhapKhoDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<PhieuNhapKhoDto>>> GetById(int id)
    {
        var entity = await _context.PhieuNhapKhos.AsNoTracking().FirstOrDefaultAsync(x => x.MaPhieuNhap == id);
        if (entity is null)
        {
            return NotFound(ApiResponse<PhieuNhapKhoDto>.Fail("NOT_FOUND", "PhieuNhapKho not found."));
        }

        var dto = _mapper.Map<PhieuNhapKhoDto>(entity);
        return Ok(ApiResponse<PhieuNhapKhoDto>.Ok(dto));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<PhieuNhapKhoDto>>> Create(PhieuNhapKhoDto dto)
    {
        var entity = _mapper.Map<PhieuNhapKho>(dto);
        _context.PhieuNhapKhos.Add(entity);
        await _context.SaveChangesAsync();

        var result = _mapper.Map<PhieuNhapKhoDto>(entity);
        return CreatedAtAction(nameof(GetById), new { id = entity.MaPhieuNhap }, ApiResponse<PhieuNhapKhoDto>.Ok(result));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> Update(int id, PhieuNhapKhoDto dto)
    {
        var entity = await _context.PhieuNhapKhos.FirstOrDefaultAsync(x => x.MaPhieuNhap == id);
        if (entity is null)
        {
            return NotFound(ApiResponse<string>.Fail("NOT_FOUND", "PhieuNhapKho not found."));
        }

        dto.Adapt(entity);
        entity.MaPhieuNhap = id;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<string>.Ok("Updated successfully."));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
    {
        var entity = await _context.PhieuNhapKhos.FirstOrDefaultAsync(x => x.MaPhieuNhap == id);
        if (entity is null)
        {
            return NotFound(ApiResponse<string>.Fail("NOT_FOUND", "PhieuNhapKho not found."));
        }

        _context.PhieuNhapKhos.Remove(entity);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<string>.Ok("Deleted successfully."));
    }

    [HttpPost("nhap-them-hang")]
    public async Task<IActionResult> NhapThemHang([FromBody] NhapThemHangDto request)
    {
        // 1. Tính toán tổng tiền (Cửa hàng + Kho)
        decimal tongTienDonHang = (decimal)(request.SoLuongNhap + request.TonKhoNhap) * request.DonGiaNhap;

        if (request.SoTienThanhToanNgay > tongTienDonHang)
        {
            return BadRequest(new { success = false, message = "Số tiền thanh toán không được lớn hơn tổng đơn." });
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var sp = await _context.SanPhams.FindAsync(request.MaSanPham);
            if (sp == null) return NotFound("Sản phẩm không tồn tại.");

            // Cập nhật giá nhập gần nhất (Giá vốn trên UI)
            if (request.SoLuongNhap > 0 || request.TonKhoNhap > 0)
            {
                sp.GiaNhapGanNhat = request.DonGiaNhap;
            }

            // CẬP NHẬT THÔNG TIN SẢN PHẨM (Đã bổ sung Thuế và Giá sau thuế)
            if (request.SanPham != null)
            {
                sp.TenSanPham = request.SanPham.TenSanPham;
                sp.MaSku = request.SanPham.MaSku;
                sp.GiaBanLe = request.SanPham.GiaBanLe;  // Giá bán trước thuế
                sp.Thue = request.SanPham.Thue;          // VAT (%)
                sp.GiaSauThue = request.SanPham.GiaSauThue; // Giá bán sau thuế
                sp.MaDanhMuc = request.SanPham.MaDanhMuc;
                sp.DonViChinh = request.SanPham.DonViChinh;
            }

            if (request.SoLuongNhap > 0 || request.TonKhoNhap > 0)
            {
                // A. Khởi tạo Phiếu Nhập Kho
                var phieuNhap = new PhieuNhapKho
                {
                    MaNcc = request.MaNhaCungCap,
                    MaKhoNhap = request.MaKho,
                    MaNguoiLap = request.MaNguoiDung,
                    NgayNhap = DateTime.UtcNow.AddHours(7),
                    TongTienNhap = tongTienDonHang,
                    DaThanhToanNcc = request.SoTienThanhToanNgay,
                    TrangThai = "da_nhap_kho"
                };
                _context.PhieuNhapKhos.Add(phieuNhap);
                await _context.SaveChangesAsync();

                // B. XỬ LÝ NHẬP CHO CỬA HÀNG
                if (request.SoLuongNhap > 0)
                {
                    sp.SoLuong += (int)request.SoLuongNhap;

                    _context.ChiTietPhieuNhaps.Add(new ChiTietPhieuNhap
                    {
                        MaPhieuNhap = phieuNhap.MaPhieuNhap,
                        MaSanPham = request.MaSanPham,
                        SoLuong = (decimal)request.SoLuongNhap,
                        GiaNhap = request.DonGiaNhap, // Giá vốn tại thời điểm nhập
                        ThanhTien = (decimal)request.SoLuongNhap * request.DonGiaNhap,
                        LoaiNhap = false
                    });

                    _context.TheKhos.Add(new TheKho
                    {
                        MaSanPham = request.MaSanPham,
                        MaKho = null,
                        NgayThayDoi = DateTime.UtcNow.AddHours(7),
                        LoaiGiaoDich = "NHAP_HANG",
                        SoLuongThayDoi = (decimal)request.SoLuongNhap,
                        SoLuongTonSauKhiThayDoi = (decimal)sp.SoLuong,
                        MaChungTuLienQuan = "PNK" + phieuNhap.MaPhieuNhap
                    });
                }

                // C. XỬ LÝ NHẬP CHO KHO
                if (request.TonKhoNhap > 0 && request.MaKho.HasValue)
                {
                    var tonKho = await _context.TonKhoChiTiets
                        .FirstOrDefaultAsync(tk => tk.MaKho == request.MaKho && tk.MaSanPham == request.MaSanPham);

                    if (tonKho != null)
                    {
                        tonKho.SoLuongTon = (tonKho.SoLuongTon ?? 0) + (decimal)request.TonKhoNhap;
                    }
                    else
                    {
                        tonKho = new TonKhoChiTiet
                        {
                            MaKho = request.MaKho.Value,
                            MaSanPham = request.MaSanPham,
                            SoLuongTon = (decimal)request.TonKhoNhap,
                            ViTriCuThe = "Khu vực nhập mới"
                        };
                        _context.TonKhoChiTiets.Add(tonKho);
                    }

                    _context.ChiTietPhieuNhaps.Add(new ChiTietPhieuNhap
                    {
                        MaPhieuNhap = phieuNhap.MaPhieuNhap,
                        MaSanPham = request.MaSanPham,
                        SoLuong = (decimal)request.TonKhoNhap,
                        GiaNhap = request.DonGiaNhap,
                        ThanhTien = (decimal)request.TonKhoNhap * request.DonGiaNhap,
                        LoaiNhap = true
                    });

                    _context.TheKhos.Add(new TheKho
                    {
                        MaSanPham = request.MaSanPham,
                        MaKho = request.MaKho,
                        NgayThayDoi = DateTime.UtcNow.AddHours(7),
                        LoaiGiaoDich = "NHAP_HANG",
                        SoLuongThayDoi = (decimal)request.TonKhoNhap,
                        SoLuongTonSauKhiThayDoi = tonKho.SoLuongTon ?? 0,
                        MaChungTuLienQuan = "PNK" + phieuNhap.MaPhieuNhap
                    });
                }

                // D. XỬ LÝ CÔNG NỢ
                decimal tienNo = tongTienDonHang - request.SoTienThanhToanNgay;
                if (tienNo > 0)
                {
                    _context.CongNoNccs.Add(new CongNoNcc
                    {
                        MaNcc = request.MaNhaCungCap,
                        MaPhieuNhap = phieuNhap.MaPhieuNhap,
                        SoTienNo = tienNo,
                        NgayPhatSinh = DateTime.UtcNow.AddHours(7),
                        TrangThai = "dang_no"
                    });
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new { success = true, message = "Cập nhật thành công!" });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return BadRequest(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
        }
    }


    //public async Task<ActionResult<ApiResponse<object>>> NhapSanPham(TaoDonNhapRequest request)
    //{
    //    await using var transaction = await _context.Database.BeginTransactionAsync();
    //    try
    //    {
    //        // 1. Tạo phiếu nhập
    //        var phieuNhap = _mapper.Map<PhieuNhapKho>(request.phieuNhapKho);
    //        await _context.PhieuNhapKhos.AddAsync(phieuNhap);
    //        await _context.SaveChangesAsync();

    //        // 2. Lấy danh sách tồn kho hiện có (1 lần query duy nhất)
    //        var maSanPhams = request.sanPhams.Select(x => x.MaSanPham).ToList();
    //        var tonKhoHienCo = await _context.TonKhoChiTiets
    //            .Where(t => maSanPhams.Contains(t.MaSanPham))
    //            .ToListAsync();
    //        var maDaTonKho = tonKhoHienCo.Select(t => t.MaSanPham).ToHashSet();

    //        var spChuaTonKho = request.sanPhams.Where(x => !maDaTonKho.Contains(x.MaSanPham)).ToList();
    //        var spDaTonKho = request.sanPhams.Where(x => maDaTonKho.Contains(x.MaSanPham)).ToList();

    //        // 3. Cập nhật số lượng tồn kho đã có
    //        foreach (var tonKho in tonKhoHienCo)
    //        {
    //            var chiTiet = request.chiTietPhieuNhapKhos.FirstOrDefault(x => x.MaSanPham == tonKho.MaSanPham);
    //            if (chiTiet != null)
    //                tonKho.SoLuongTon += chiTiet.SoLuong;
    //        }

    //        // 4. Thêm sản phẩm mới + tạo tồn kho
    //        var sanPhamMois = _mapper.Map<List<SanPham>>(spChuaTonKho);
    //        await _context.SanPhams.AddRangeAsync(sanPhamMois);
    //        await _context.SaveChangesAsync();

    //        var tonKhoMois = spChuaTonKho.Select(item =>
    //        {
    //            var chiTiet = request.chiTietPhieuNhapKhos.First(x => x.MaSanPham == item.MaSanPham);
    //            return new TonKhoChiTiet
    //            {
    //                MaSanPham = item.MaSanPham,
    //                SoLuongTon = chiTiet.SoLuong,
    //                ViTriCuThe = "test"
    //            };
    //        });
    //        await _context.TonKhoChiTiets.AddRangeAsync(tonKhoMois);

    //        // 5. Thêm chi tiết phiếu nhập
    //        // ✅ ĐÚNG
    //        var chiTiets = request.chiTietPhieuNhapKhos.Select(item =>
    //        {
    //            item.MaPhieuNhap = phieuNhap.MaPhieuNhap;
    //            return _mapper.Map<ChiTietPhieuNhap>(item);
    //        }).ToList();
    //        await _context.ChiTietPhieuNhaps.AddRangeAsync(chiTiets);
    //        phieuNhap.TongTienNhap = chiTiets.Sum(x => x.ThanhTien);
    //        await _context.SaveChangesAsync();
    //        await transaction.CommitAsync();

    //        return Ok(new ApiResponse<object> { Success = true });
    //    }
    //    catch (Exception ex)
    //    {
    //        await transaction.RollbackAsync();
    //        return BadRequest(new ApiResponse<object> { Success = false });
    //    }
    //}

}
