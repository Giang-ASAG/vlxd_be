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
    public async Task<ActionResult<ApiResponse<string>>> CreateNccOptionalImport(CreateNccWithProductsDto dto)
    {
        // Kiểm tra tính hợp lệ của thông tin nhà cung cấp cơ bản trước
        if (string.IsNullOrEmpty(dto.TenNcc))
        {
            return BadRequest(ApiResponse<string>.Fail("400", "Tên nhà cung cấp không được để trống."));
        }

        // TRƯỜNG HỢP 1: Người dùng CHỈ tạo mới nhà cung cấp (Không chọn sản phẩm nào)
        if (dto.SelectedProductIds == null || !dto.SelectedProductIds.Any())
        {
            try
            {
                var nhaCungCapMoi = new NhaCungCap
                {
                    TenNcc = dto.TenNcc,
                    SoDienThoai = dto.SoDienThoai,
                    DiaChi = dto.DiaChi,
                    Email = dto.Email,
                    GhiChu = dto.GhiChu
                };

                _context.NhaCungCaps.Add(nhaCungCapMoi);
                await _context.SaveChangesAsync();

                return Ok(ApiResponse<string>.Succes("200", "Thêm mới nhà cung cấp thành công (Không lập đơn nhập)."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.Fail("500", $"Lỗi khi tạo nhà cung cấp: {ex.Message}"));
            }
        }

        // TRƯỜNG HỢP 2: Người dùng vừa tạo nhà cung cấp VỪA chọn sản phẩm để nhập kho
        // Sử dụng Transaction để bảo vệ dữ liệu liên kết nhiều bảng
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Tạo mới Nhà cung cấp
            var nhaCungCap = new NhaCungCap
            {
                TenNcc = dto.TenNcc,
                SoDienThoai = dto.SoDienThoai,
                DiaChi = dto.DiaChi,
                Email = dto.Email,
                GhiChu = dto.GhiChu
            };
            _context.NhaCungCaps.Add(nhaCungCap);
            await _context.SaveChangesAsync(); // Lưu để lấy nhaCungCap.MaNcc

            // 2. Tạo vỏ Phiếu nhập kho (Tổng tiền tạm thời bằng 0)
            var phieuNhapKho = new PhieuNhapKho
            {
                MaKhoNhap = 1,
                MaNcc = nhaCungCap.MaNcc,
                MaNguoiLap = dto.MaNguoiLap,
                NgayNhap = DateTime.Now,
                TongTienNhap = 0,
                TrangThai = "da_nhap_kho"
            };
            _context.PhieuNhapKhos.Add(phieuNhapKho);
            await _context.SaveChangesAsync(); // Lưu để lấy phieuNhapKho.MaPhieuNhap

            // 3. Cập nhật mã nhà cung cấp cho các sản phẩm được chọn (thay thế giá trị null cũ)
            var sanPhams = await _context.SanPhams
                                         .Where(x => dto.SelectedProductIds.Contains(x.MaSanPham))
                                         .ToListAsync();

            foreach (var sp in sanPhams)
            {
                sp.MaNccMacDinh = nhaCungCap.MaNcc;
            }

            // 4. Tìm các chi tiết phiếu nhập "mồ côi" (MaPhieuNhap đang null) để gắn vào mã Phiếu nhập vừa sinh ra
            var chiTietPhieuNhaps = await _context.ChiTietPhieuNhaps
                                                  .Where(x => x.MaPhieuNhap == null && dto.SelectedProductIds.Contains(x.MaSanPham.Value))
                                                  .ToListAsync();

            foreach (var ct in chiTietPhieuNhaps)
            {
                ct.MaPhieuNhap = phieuNhapKho.MaPhieuNhap;
            }

            // Lưu thay đổi của Sản phẩm và Chi tiết xuống DB
            await _context.SaveChangesAsync();

            // 5. Tính toán lại tổng tiền thực tế của phiếu nhập dựa trên số lượng và giá nhập của các chi tiết
            var tongTienPhieuNhap = chiTietPhieuNhaps.Sum(x => x.ThanhTien);
            phieuNhapKho.TongTienNhap = tongTienPhieuNhap;

            // 6. Tạo dữ liệu công nợ cho nhà cung cấp tương ứng với phiếu nhập kho này
            var congNo = new CongNoNcc
            {
                MaNcc = nhaCungCap.MaNcc,
                MaPhieuNhap = phieuNhapKho.MaPhieuNhap,
                NgayPhatSinh = DateTime.Now,
                SoTienNo = (decimal)tongTienPhieuNhap, // Mặc định ghi nhận nợ bằng tổng tiền của đơn nhập hàng
                TrangThai = "dang_no"
            };
            _context.CongNoNccs.Add(congNo);

            // Lưu cập nhật tổng tiền và bảng công nợ lần cuối
            await _context.SaveChangesAsync();

            // Xác nhận hoàn tất Transaction thành công hoàn toàn
            await transaction.CommitAsync();

            return Ok(ApiResponse<string>.Succes("200", "Tạo nhà cung cấp, liên kết sản phẩm và lập phiếu nhập kho thành công!"));
        }
        catch (Exception ex)
        {
            // Có bất kỳ lỗi gì phát sinh (ví dụ: lỗi khóa ngoại ID kho, ID người dùng...), hủy bỏ toàn bộ tiến trình
            await transaction.RollbackAsync();
            return StatusCode(500, ApiResponse<string>.Fail("500", $"Có lỗi xảy ra trong quá trình xử lý: {ex.Message}"));
        }
    }

    [HttpPut("{id}")] // Frontend gọi bằng: PUT /api/NhaCungCaps/5
    public async Task<ActionResult<ApiResponse<string>>> Update(int id, CreateNccWithProductsDto dto)
    {
        // 1. Dùng trực tiếp tham số 'id' từ URL để tìm Nhà cung cấp
        var nhaCungCap = await _context.NhaCungCaps.FirstOrDefaultAsync(x => x.MaNcc == id);
        if (nhaCungCap == null)
        {
            return NotFound(ApiResponse<string>.Fail("404", "Không tìm thấy nhà cung cấp cần cập nhật."));
        }

        if (string.IsNullOrEmpty(dto.TenNcc))
        {
            return BadRequest(ApiResponse<string>.Fail("400", "Tên nhà cung cấp không được để trống."));
        }

        // Cập nhật các thông tin cơ bản
        nhaCungCap.TenNcc = dto.TenNcc;
        nhaCungCap.SoDienThoai = dto.SoDienThoai;
        nhaCungCap.DiaChi = dto.DiaChi;
        nhaCungCap.Email = dto.Email;
        nhaCungCap.GhiChu = dto.GhiChu;

        // TRƯỜNG HỢP 1: Chỉ cập nhật thông tin (Không chọn thêm sản phẩm mồ côi)
        if (dto.SelectedProductIds == null || !dto.SelectedProductIds.Any())
        {
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

        // TRƯỜNG HỢP 2: Vừa cập nhật thông tin vừa gom thêm sản phẩm mồ côi vào một đơn nhập mới
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Lưu thông tin nhà cung cấp đã sửa đổi trước
            await _context.SaveChangesAsync();

            // Tạo một Phiếu nhập kho MỚI gắn với Nhà cung cấp này (sử dụng biến 'id' từ URL hoặc nhaCungCap.MaNcc)
            var phieuNhapKho = new PhieuNhapKho
            {
                MaKhoNhap = 1,
                MaNcc = nhaCungCap.MaNcc,
                MaNguoiLap = dto.MaNguoiLap,
                NgayNhap = DateTime.Now,
                TongTienNhap = 0,
                TrangThai = "da_nhap_kho"
            };
            _context.PhieuNhapKhos.Add(phieuNhapKho);
            await _context.SaveChangesAsync();

            // Cập nhật mã nhà cung cấp cho các sản phẩm mồ côi được chọn
            var sanPhams = await _context.SanPhams
                                         .Where(x => dto.SelectedProductIds.Contains(x.MaSanPham))
                                         .ToListAsync();

            foreach (var sp in sanPhams)
            {
                sp.MaNccMacDinh = nhaCungCap.MaNcc;
            }

            // Tìm các chi tiết phiếu nhập mồ côi để gắn mã phiếu nhập vào
            var chiTietPhieuNhaps = await _context.ChiTietPhieuNhaps
                                                  .Where(x => x.MaPhieuNhap == null
                                                           && x.MaSanPham != null
                                                           && dto.SelectedProductIds.Contains(x.MaSanPham.Value))
                                                  .ToListAsync();

            foreach (var ct in chiTietPhieuNhaps)
            {
                ct.MaPhieuNhap = phieuNhapKho.MaPhieuNhap;
            }

            await _context.SaveChangesAsync();

            // Tính toán tổng tiền
            var tongTienPhieuNhap = chiTietPhieuNhaps.Sum(x => x.ThanhTien);
            phieuNhapKho.TongTienNhap = tongTienPhieuNhap;

            // Sinh công nợ mới phát sinh cho đợt nhập hàng này
            var congNo = new CongNoNcc
            {
                MaNcc = nhaCungCap.MaNcc,
                MaPhieuNhap = phieuNhapKho.MaPhieuNhap,
                NgayPhatSinh = DateTime.Now,
                SoTienNo = (decimal)tongTienPhieuNhap,
                TrangThai = "dang_no"
            };
            _context.CongNoNccs.Add(congNo);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(ApiResponse<string>.Succes("200", "Cập nhật nhà cung cấp và lập phiếu nhập kho bổ sung thành công!"));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, ApiResponse<string>.Fail("500", $"Có lỗi xảy ra: {ex.Message}"));
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


