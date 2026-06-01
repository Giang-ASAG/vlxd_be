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
                    GhiChu = temp.pn.GhiChu,
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
    public async Task<ActionResult<ApiResponse<string>>> Create(PhieuNhapCreateDto dto)
    {
        if (dto.ChiTiets == null || !dto.ChiTiets.Any())
        {
            return BadRequest(ApiResponse<string>.Fail("FAIL", "Danh sách mặt hàng nhập không được để trống."));
        }

        // Mở Transaction bảo vệ toàn vẹn dữ liệu
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Tính toán tổng tiền nhập hàng
            decimal tongTienNhap = 0;
            foreach (var item in dto.ChiTiets)
            {
                tongTienNhap += item.SoLuong * item.GiaNhap;
            }

            // Giới hạn số tiền thực nhận không vượt quá tổng hóa đơn để tránh sai lệch kế toán
            decimal soTienThanhToanThucTe = dto.SoTienThanhToanNgay > tongTienNhap
                ? tongTienNhap
                : dto.SoTienThanhToanNgay;

            // 2. Thêm mới bản ghi vào bảng phieu_nhap_kho
            var phieuNhap = new PhieuNhapKho
            {
                MaNcc = dto.MaNcc,
                MaKhoNhap = dto.MaKhoNhap ?? 1,
                MaNguoiLap = dto.MaNguoiLap,
                NgayNhap = dto.NgayNhap,
                TongTienNhap = tongTienNhap,
                DaThanhToanNcc = soTienThanhToanThucTe, // Cập nhật số tiền đã trả ngay vào phiếu nhập
                TrangThai = "da_nhap_kho",
                GhiChu = dto.GhiChu
            };

            await _context.PhieuNhapKhos.AddAsync(phieuNhap);
            await _context.SaveChangesAsync(); // Lưu để sinh tự động ma_phieu_nhap

            // 3. Xử lý chi tiết mặt hàng, đồng bộ kho và nhà cung cấp
            foreach (var item in dto.ChiTiets)
            {
                // 3.1. Thêm mới chi_tiet_phieu_nhap
                var chiTiet = new ChiTietPhieuNhap
                {
                    MaPhieuNhap = phieuNhap.MaPhieuNhap,
                    MaSanPham = item.MaSanPham,
                    SoLuong = item.SoLuong,
                    GiaNhap = item.GiaNhap,// Mặc định hàng nhập mới thông thường
                };
                await _context.ChiTietPhieuNhaps.AddAsync(chiTiet);

                // 3.2. Đồng bộ gắn Nhà cung cấp mặc định nếu đang trống (NULL)
                var sanPham = await _context.SanPhams
                    .FirstOrDefaultAsync(x => x.MaSanPham == item.MaSanPham);

                if (sanPham != null)
                {
                    if (sanPham.MaNccMacDinh == null)
                    {
                        sanPham.MaNccMacDinh = dto.MaNcc;
                    }
                    sanPham.GiaNhapGanNhat = item.GiaNhap;
                    _context.SanPhams.Update(sanPham);
                }

                // 3.3. Cộng dồn số lượng vào bảng ton_kho_chi_tiet
                var tonKho = await _context.TonKhoChiTiets
                    .FirstOrDefaultAsync(x => x.MaSanPham == item.MaSanPham && x.MaKho == phieuNhap.MaKhoNhap);

                if (tonKho != null)
                {
                    tonKho.SoLuongTon += item.SoLuong;
                    _context.TonKhoChiTiets.Update(tonKho);
                }
                else
                {
                    var moiTonKho = new TonKhoChiTiet
                    {
                        MaKho = phieuNhap.MaKhoNhap ?? 1,
                        MaSanPham = item.MaSanPham,
                        SoLuongTon = item.SoLuong,
                        ViTriCuThe = "Nhà kho"
                    };
                    await _context.TonKhoChiTiets.AddAsync(moiTonKho);
                }
            }

            // 4. Xử lý logic Công nợ (Bảng cong_no_ncc)
            decimal soTienNoConLai = tongTienNhap - soTienThanhToanThucTe;
            string trangThaiCongNo = soTienNoConLai == 0 ? "hoan_tat" : "dang_no";

            var congNoMoi = new CongNoNcc
            {
                MaNcc = dto.MaNcc,
                MaPhieuNhap = phieuNhap.MaPhieuNhap,
                SoTienNo = soTienNoConLai, // Số tiền còn nợ (0 hoặc khoản chênh lệch)
                NgayPhatSinh = dto.NgayNhap,
                TrangThai = trangThaiCongNo
            };

            await _context.CongNoNccs.AddAsync(congNoMoi);
            await _context.SaveChangesAsync(); // Lưu để sinh tự động id của công nợ làm khóa ngoại cho lịch sử thanh toán

            // 5. Xử lý Lịch sử thanh toán (Bảng lichsuthanhtoan) nếu có phát sinh giao dịch tiền lẻ/tiền mặt
            if (soTienThanhToanThucTe > 0)
            {
                var lichSu = new LichSuThanhToan
                {
                    IsNhaCungCap = true,                   // Luôn luôn là true theo yêu cầu nhập hàng NCC
                    conNoID = congNoMoi.Id,                // Lấy ID tự sinh từ bảng công nợ vừa tạo phía trên
                    SoTien = soTienThanhToanThucTe,
                    PhuongThucThanhToan = true,               // Mặc định 1: Chuyển khoản hoặc bạn tự ánh xạ từ client
                    NgayThanhToan = dto.NgayNhap,
                    GhiChu = soTienNoConLai == 0
                        ? $"Thanh toán hoàn tất toàn bộ hóa đơn nhập kho #{phieuNhap.MaPhieuNhap}"
                        : $"Thanh toán một phần hóa đơn nhập kho #{phieuNhap.MaPhieuNhap}"
                };
                await _context.LichSuThanhToans.AddAsync(lichSu);
            }

            // 6. Đồng bộ toàn bộ dữ liệu xuống database
            await _context.SaveChangesAsync();

            // Commit Transaction thành công mỹ mãn
            await transaction.CommitAsync();

            return Ok(ApiResponse<string>.Succes("SUCCESS", "Xử lý phiếu nhập, cập nhật dữ liệu kho, nhà cung cấp và hoạch toán công nợ thành công!"));
        }
        catch (Exception ex)
        {
            // Thu hồi dữ liệu ngay lập tức nếu bất kỳ bước nào xảy ra ngoại lệ
            await transaction.RollbackAsync();
            return StatusCode(500, ApiResponse<string>.Fail("ERROR", $"Lỗi hệ thống: {ex.Message}"));
        }
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
        decimal tongTienDonHang = (decimal)(request.TonKhoNhap) * request.DonGiaNhap;

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
            if (request.TonKhoNhap > 0)
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

            if (request.TonKhoNhap > 0)
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
