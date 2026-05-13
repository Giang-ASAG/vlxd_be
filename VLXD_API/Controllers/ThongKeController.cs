using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Xml.Linq;
using VLXD_API.Common;
using VLXD_API.DTOs;
using VLXD_API.DTOs.BaoCaoTaiChinh;
using VLXD_API.DTOs.SanPham;
using VLXD_API.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VLXD_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThongKeController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ThongKeController(AppDbContext context) { 
            _context = context;
        }

        [HttpGet]   
        public async Task<ActionResult<ApiResponse<ThongKeNamDto>>> DoanhThuNam(int year)
        {
            // Query trực tiếp DB (không ToList sớm)
            var donHangs = _context.DonHangs
                .Where(x => x.NgayTao.Value.Year == year);

            // Group đơn hàng theo tháng
            var doanhThuThang = await donHangs
                .GroupBy(x => x.NgayTao.Value.Month)
                .Select(g => new ThongKeThangDto
                {
                    Thang = g.Key,
                    TongDonHang = g.Count(),
                    TongDoanhThu = (decimal)g.Sum(x => x.TongTien),
                    TongSanPhamBan = (int)g.Sum(x => x.ChiTietDonHangs.Sum(p => p.SoLuong)),
                }).ToListAsync();

            // Đảm bảo đủ 12 tháng
            var fullMonths = Enumerable.Range(1, 12)
                .Select(m => doanhThuThang.FirstOrDefault(x => x.Thang == m)
                    ?? new ThongKeThangDto
                    {
                        Thang = m,
                        TongDoanhThu = 0,
                        TongDonHang = 0,
                    })
                .ToList();

            var result = new ThongKeNamDto
            {
                Nam = year,
                DanhSachThang = fullMonths,
                TongDoanhThu = fullMonths.Sum(x => x.TongDoanhThu),
                TongDonHang = fullMonths.Sum(x => x.TongDonHang),
                TongSanPhamBan = fullMonths.Sum(x => x.TongSanPhamBan),
            };

            return Ok(ApiResponse<ThongKeNamDto>.Ok(result));
        }

        [HttpGet("homnay")]
        public async Task<ActionResult<ApiResponse<ThongKeHomNayDto>>> GetThongKeHomNay()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var yesterday = today.AddDays(-1);
            var last_year = today.Year - 1;
            var donHangsTodayQuery = _context.DonHangs
                .Where(dh => dh.NgayTao >= today && dh.NgayTao < tomorrow);

            var donHangsYesterdayQuery = _context.DonHangs
                .Where(dh => dh.NgayTao >= yesterday && dh.NgayTao < today);

            // ===== HÔM NAY =====
            var tongDonHang = await donHangsTodayQuery.CountAsync();

            var doanhThuToday = await donHangsTodayQuery
                .SumAsync(dh => dh.TongTien ?? 0);

            var tienDaThu = await donHangsTodayQuery
                .SumAsync(dh => dh.SoTienTra ?? 0);

            var soSanPham = await donHangsTodayQuery
                .SelectMany(dh => dh.ChiTietDonHangs)
                .SumAsync(ct => (int?)ct.SoLuong) ?? 0;

            // ===== HÔM QUA =====
            var doanhThuYesterday = await donHangsYesterdayQuery
                .SumAsync(dh => dh.TongTien ?? 0);

            var donYesterday = await donHangsYesterdayQuery.CountAsync();

            // ===== TOÀN BỘ =====
            var doanhThuNamNay = await _context.DonHangs.Where(o=>o.NgayTao.Value.Year==today.Year)
                .SumAsync(x => x.TongTien ?? 0);

            var doanhThuNamTrc = await _context.DonHangs.Where(o => o.NgayTao.Value.Year == last_year)
                .SumAsync(x => x.TongTien ?? 0);
            decimal tyle_doanhthutheonam = 0;
            if (doanhThuNamTrc > 0)
                tyle_doanhthutheonam = ((doanhThuNamNay - doanhThuNamTrc) / doanhThuNamTrc * 100);
            else
                tyle_doanhthutheonam = 100;

            var sanPhamSapHet = await _context.TonKhoChiTiets
                    .CountAsync(x => x.SoLuongTon <= 30);

            // ===== TỶ LỆ =====
            decimal tyLeDoanhThu=0;
            if (doanhThuYesterday > 0)
                tyLeDoanhThu = ((doanhThuToday - doanhThuYesterday) / doanhThuYesterday * 100);
            else
                tyLeDoanhThu = 100;

            var countToday = donHangsTodayQuery.Count();
            var countYesterday = donHangsYesterdayQuery.Count();
            double tyLeDon = 0;
            if (countYesterday > 0)
            {
                // Ép kiểu sang double để có kết quả chính xác (ví dụ: 0.25 thay vì 0)
                tyLeDon = (double)(countToday - countYesterday) / countYesterday * 100;
            }
            else if (countToday > 0)
            {
                tyLeDon = 100; // Hoặc logic tùy bạn nếu hôm qua không có đơn nào
            }

            // ===== RESULT =====
            var result = new ThongKeHomNayDto
            {
                TongDonHang = tongDonHang,
                TongDoanhThu = doanhThuToday,
                TongTienDaThu = tienDaThu,
                SoSanPhamDaBan = soSanPham,
                DoanhThuNamNay = doanhThuNamNay,
                tyle_theonam = tyle_doanhthutheonam,
                SanPhamSapHet = sanPhamSapHet,
                tyle_doanhthu = tyLeDoanhThu,
                tyle_donhang = tyLeDon
            };

            return Ok(ApiResponse<ThongKeHomNayDto>.Ok(result));
        }

        private DateTime ConvertToVietNamTime(DateTime dt)
        {
            // 1. Xác định ID múi giờ VN (Hỗ trợ cả Windows và Linux)
            string tzId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                          ? "SE Asia Standard Time"
                          : "Asia/Ho_Chi_Minh";
            TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById(tzId);

            // 2. Lấy giờ HIỆN TẠI của Việt Nam làm mốc chuẩn
            DateTime nowVn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone);

            // 3. Tính độ lệch giữa giờ máy tính đang chạy và giờ trong DB
            // Nếu DB lưu UTC, độ lệch so với máy tính VN sẽ xấp xỉ 7 tiếng
            double offset = (DateTime.Now - dt).TotalHours;

            if (Math.Abs(offset) >= 6.5 && Math.Abs(offset) <= 7.5)
            {
                // Nếu rơi vào khoảng lệch 7 tiếng -> DB đang là UTC -> Chuyển sang VN
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(dt, DateTimeKind.Utc), vnTimeZone);
            }

            // Nếu không lệch (offset gần bằng 0) -> DB đã là giờ VN rồi, trả về luôn
            return dt;
        }

        [HttpGet("doanhthuTuan")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ThongKeTuanDto>>>> GetDoanhThuTuanNay()
        {
            // Xác định múi giờ VN
            string tzId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "SE Asia Standard Time" : "Asia/Ho_Chi_Minh";
            TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById(tzId);

            // Lấy mốc thời gian "Bây giờ" chuẩn VN
            DateTime nowVn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone);

            // Tính ngày Thứ 2 đầu tuần (giờ VN)
            int diff = (7 + (nowVn.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime startOfWeekVn = nowVn.AddDays(-1 * diff).Date;
            DateTime endOfWeekVn = startOfWeekVn.AddDays(7);

            // Truy vấn dữ liệu
            var dailyData = await _context.DonHangs
                .AsNoTracking()
                .Where(dh => dh.NgayTao.HasValue)
                .ToListAsync(); // Lấy về để xử lý logic múi giờ linh hoạt

            var result = new List<ThongKeTuanDto>();
            string[] names = { "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "Chủ Nhật" };

            for (int i = 0; i < 7; i++)
            {
                var currentDayVn = startOfWeekVn.AddDays(i);

                // Dùng hàm ConvertToVietNamTime để chuẩn hóa từng dòng dữ liệu trước khi so sánh
                var sumDoanhThu = dailyData
                    .Where(dh => ConvertToVietNamTime(dh.NgayTao.Value).Date == currentDayVn.Date)
                    .Sum(dh => dh.TongTien ?? 0);

                result.Add(new ThongKeTuanDto
                {
                    Ngay = $"{names[i]} ({currentDayVn:dd/MM})",
                    DoanhThu = sumDoanhThu
                });
            }

            return Ok(ApiResponse<IEnumerable<ThongKeTuanDto>>.Ok(result));
        }

        [HttpGet("spBanChay")]
        public async Task<ActionResult<ApiResponse<IEnumerable<SanPhamBanChayDto>>>> GetTopProducts(int limit = 5)
        {
            var topProducts = await _context.ChiTietDonHangs
                .GroupBy(ct => new
                {
                    ct.MaSanPham,
                    TenSanPham = ct.MaSanPhamNavigation.TenSanPham
                })
                .Select(g => new SanPhamBanChayDto
                {
                    MaSanPham = g.Key.MaSanPham ?? 0,
                    TenSanPham = g.Key.TenSanPham ?? "N/A",
                    SoLuongDaBan = (int)g.Sum(ct => ct.SoLuong),
                    TongDoanhThu = g.Sum(ct => (decimal)ct.SoLuong * ct.DonGia),
                })
                .OrderByDescending(x => x.SoLuongDaBan)
                .Take(limit)
                .ToListAsync();

            var maSpList = topProducts.Select(x => x.MaSanPham).ToList();

            var tonKhos = await _context.TonKhoChiTiets
                .Where(t => maSpList.Contains(t.MaSanPham))
                .GroupBy(t => t.MaSanPham)
                .Select(g => new
                {
                    MaSanPham = g.Key,
                    TongTon = g.Sum(x => (int?)x.SoLuongTon) ?? 0
                })
                .ToListAsync();

            var tonDict = tonKhos.ToDictionary(x => x.MaSanPham, x => x.TongTon);

            foreach (var item in topProducts)
            {
                // nếu TongTonKho = tồn hiện tại:
                // item.TongTonKho = tonDict.TryGetValue(item.MaSanPham, out var tongTon) ? tongTon : 0;

                // nếu bạn thật sự muốn = tồn + đã bán thì:
                var sp = _context.SanPhams.First(x=>x.MaSanPham==item.MaSanPham);
                item.TongTonKho = (tonDict.TryGetValue(item.MaSanPham, out var tongTon) ? tongTon : 0) + item.SoLuongDaBan + sp.SoLuong;
            }

            return Ok(ApiResponse<IEnumerable<SanPhamBanChayDto>>.Ok(topProducts));
        }



        // Hàm phụ trợ để hiển thị thời gian theo chuẩn Việt Nam
        private string GetThoiGianHienThiVietNam(DateTime? ngayTao)
        {
            if (!ngayTao.HasValue) return "Không xác định";

            // 1. Lấy ID múi giờ VN (không quan tâm thiết bị đang ở đâu)
            string tzId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                          ? "SE Asia Standard Time"
                          : "Asia/Ho_Chi_Minh";
            TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById(tzId);

            // 2. Lấy giờ HIỆN TẠI của Việt Nam từ giờ UTC chuẩn
            DateTime nowVn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone);

            // 3. Xử lý NgayTao từ Database
            DateTime dtNgayTao = ngayTao.Value;

            // Tính toán thử độ lệch giữa giờ hiện tại (máy đang chạy) và giờ đơn hàng
            // Mục tiêu: Xác định xem DB đang lưu kiểu gì để đưa về giờ VN chuẩn
            double diffWithSystem = (DateTime.Now - dtNgayTao).TotalHours;

            DateTime dtNgayTaoVn;
            if (Math.Abs(diffWithSystem - 7) < 1) // Nếu lệch xấp xỉ 7 tiếng so với máy VN
            {
                // DB đang lưu UTC -> Đưa về VN
                dtNgayTaoVn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(dtNgayTao, DateTimeKind.Utc), vnTimeZone);
            }
            else
            {
                // DB đã lưu giờ VN (hoặc cùng múi giờ với thiết bị)
                dtNgayTaoVn = dtNgayTao;
            }

            // 4. Tính toán khoảng cách (Dùng mốc VN - VN để tuyệt đối chính xác)
            TimeSpan timeSpan = nowVn - dtNgayTaoVn;
            double seconds = timeSpan.TotalSeconds;

            if (seconds < 30) return "Vừa xong";
            if (seconds < 60) return $"{(int)seconds} giây trước";

            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} phút trước";

            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} tiếng trước";

            return dtNgayTaoVn.ToString("dd/MM/yyyy HH:mm");
        }

        [HttpGet("donhangGanNhat")]
        public async Task<ActionResult<ApiResponse<IEnumerable<DonHangGanDayDto>>>> GetRecentOrders()
        {
            // 1. Lấy 5 đơn hàng mới nhất từ database
            var recentOrders = await _context.DonHangs
                .AsNoTracking()
                .Include(dh => dh.MaKhachHangNavigation)
                .OrderByDescending(dh => dh.NgayTao)
                .Take(5)
                .ToListAsync();

            // 2. Chuyển đổi sang DTO và xử lý hiển thị thời gian theo giờ VN
            var result = recentOrders.Select(dh => new DonHangGanDayDto
            {
                MaDonHang = dh.MaDonHang,
                TenKhachHang = dh.MaKhachHangNavigation?.HoTen ?? "Khách lẻ",
                TrangThaiThanhToan = dh.TrangThaiThanhToan ?? "Chưa thanh toán",
                TongTien = dh.TongTien ?? 0,
                ThoiGianHienThi = GetThoiGianHienThiVietNam(dh.NgayTao)
            });

            return Ok(ApiResponse<IEnumerable<DonHangGanDayDto>>.Ok(result));
        }


        [HttpGet("bao-cao-tai-chinh-theo-ngay")]
        public ActionResult<ApiResponse<BaoCaoTaiChinhDto>> baocaotaichinhttheongay(
    [FromQuery] DateTime dau,
    [FromQuery] DateTime cuoi,
    [FromQuery] string groupBy = "month")
        {
            if (groupBy == "day")
            {
                // 1. Tạo danh sách ngày
                var kyBC_list = new List<KyBaoCao>();

                for (var date = dau.Date; date <= cuoi.Date; date = date.AddDays(1))
                {
                    kyBC_list.Add(new KyBaoCao
                    {
                        Key = date.ToString("yyyy-MM-dd"),
                        Label = date.ToString("dd/MM/yyyy")
                    });
                }

                // 2. Doanh thu
                var doanhThuData = _context.DonHangs
                    .Where(x => x.NgayTao.HasValue &&
                                x.NgayTao.Value.Date >= dau.Date &&
                                x.NgayTao.Value.Date <= cuoi.Date)
                    .GroupBy(x => x.NgayTao.Value.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        Total = g.Sum(x => x.SoTienTra)
                    })
                    .ToList();

                // 3. Giá vốn (COGS)
                var cogsData = _context.PhieuNhapKhos
                    .Where(x => x.NgayNhap.HasValue &&
                                x.NgayNhap.Value.Date >= dau.Date &&
                                x.NgayNhap.Value.Date <= cuoi.Date)
                    .GroupBy(x => x.NgayNhap.Value.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        Total = g.Sum(x => x.DaThanhToanNcc)
                    })
                    .ToList();

                // 4. Chi phí vận chuyển
                var shippingDict = new Dictionary<string, decimal>();
                var salaryDict = new Dictionary<string, decimal>();
                var otherDict = new Dictionary<string, decimal>();
                var revenueDict = new Dictionary<string, decimal>();
                var cogsDict = new Dictionary<string, decimal>();
                var profitDict = new Dictionary<string, decimal>();

                foreach (var col in kyBC_list)
                {
                    var date = DateTime.Parse(col.Key);

                    var revenue = doanhThuData.FirstOrDefault(x => x.Date == date)?.Total ?? 0;
                    var cogs = cogsData.FirstOrDefault(x => x.Date == date)?.Total ?? 0;


                    //var shipping = revenue * 0.05m;
                    //var salary = revenue * 0.10m;
                    //var other = revenue * 0.03m;

                    var shipping = 0.0m;
                    var salary = 0.0m;
                    var other = 0.0m;

                    revenueDict[col.Key] = revenue;
                    cogsDict[col.Key] = cogs;
                    shippingDict[col.Key] = shipping;
                    salaryDict[col.Key] = salary;
                    otherDict[col.Key] = other;

                    profitDict[col.Key] = revenue - cogs - shipping - salary - other;
                }


                // 8. Rows
                var dongTaiChinh = new List<DongTaiChinh>
                {
                    new DongTaiChinh
                    {
                        Id = "revenue",
                        Name = "Doanh thu bán hàng",
                        Values = revenueDict
                    },
                    new DongTaiChinh
                    {
                        Id = "cogs",
                        Name = "Giá vốn hàng bán",
                        Values = cogsDict
                    },
                    new DongTaiChinh
                    {
                        Id = "shipping",
                        Name = "Chi phí vận chuyển",
                        Values = shippingDict
                    },
                    new DongTaiChinh
                    {
                        Id = "salary",
                        Name = "Chi phí lương",
                        Values = salaryDict
                    },
                    new DongTaiChinh
                    {
                        Id = "other_expenses",
                        Name = "Chi phí khác",
                        Values = otherDict
                    },
                    new DongTaiChinh
                    {
                        Id = "profit",
                        Name = "Tổng lợi nhuận",
                        Values = profitDict
                    }
                };

                var baoCao = new BaoCaoTaiChinhDto
                {
                    Columns = kyBC_list,
                    Rows = dongTaiChinh
                };

                return Ok(ApiResponse<BaoCaoTaiChinhDto>.Ok(baoCao));
            }
            else if (groupBy == "month")
            {
                var kyBaos = new List<KyBaoCao>();

                var startDate = new DateTime(dau.Year, dau.Month, 1);
                var endDate = new DateTime(cuoi.Year, cuoi.Month, 1).AddMonths(1).AddDays(-1);
                for (var date = startDate; date <= endDate; date = date.AddMonths(1))
                {
                    kyBaos.Add(new KyBaoCao
                    {
                        Key = date.ToString("yyyy-MM"),
                        Label = date.ToString("MM/yyyy")
                    });
                }
                var doanhThuData = _context.DonHangs
                    .Where(x => x.NgayTao.HasValue &&
                                x.NgayTao.Value >= startDate &&
                                x.NgayTao.Value <= endDate)
                    .GroupBy(x => new { x.NgayTao.Value.Year, x.NgayTao.Value.Month })
                    .Select(g => new
                    {
                        Key = $"{g.Key.Year}-{g.Key.Month:D2}",
                        Total = g.Sum(x => x.SoTienTra)
                    })
                    .ToDictionary(x => x.Key, x => x.Total);

                var cogsData = _context.PhieuNhapKhos
               .Where(x => x.NgayNhap.HasValue &&
                           x.NgayNhap.Value >= startDate &&
                           x.NgayNhap.Value <= endDate)
               .GroupBy(x => new { x.NgayNhap.Value.Year, x.NgayNhap.Value.Month })
               .Select(g => new
               {
                   Key = $"{g.Key.Year}-{g.Key.Month:D2}",
                   Total = g.Sum(x => x.DaThanhToanNcc)
               })
               .ToDictionary(x => x.Key, x => x.Total);

                // 4. Chi phí vận chuyển
                var revenueDict = new Dictionary<string, decimal>();
                var cogsDict = new Dictionary<string, decimal>();
                var shippingDict = new Dictionary<string, decimal>();
                var salaryDict = new Dictionary<string, decimal>();
                var otherDict = new Dictionary<string, decimal>();
                var profitDict = new Dictionary<string, decimal>();

                foreach (var col in kyBaos)
                {
                    var key = col.Key;

                    var revenue = doanhThuData.GetValueOrDefault(key, 0);
                    var cogs = cogsData.GetValueOrDefault(key, 0);

                    //var shipping = revenue * 0.05m;
                    //var salary = revenue * 0.10m;
                    //var other = revenue * 0.03m;

                    var shipping = 0.0m;
                    var salary = 0.0m;
                    var other = 0.0m;

                    revenueDict[key] = (decimal)revenue;
                    cogsDict[key] = (decimal)cogs;
                    shippingDict[key] = (decimal)shipping;
                    salaryDict[key] = (decimal)salary;
                    otherDict[key] = (decimal)other;

                    profitDict[key] = (decimal)(revenue - cogs - shipping - salary - other);
                }

                // 8. Rows
                var dongTaiChinh = new List<DongTaiChinh>
                {
                    new DongTaiChinh
                    {
                        Id = "revenue",
                        Name = "Doanh thu bán hàng",
                        Values = revenueDict
                    },
                    new DongTaiChinh
                    {
                        Id = "cogs",
                        Name = "Giá vốn hàng bán",
                        Values = cogsDict
                    },
                    new DongTaiChinh
                    {
                        Id = "shipping",
                        Name = "Chi phí vận chuyển",
                        Values = shippingDict
                    },
                    new DongTaiChinh
                    {
                        Id = "salary",
                        Name = "Chi phí lương",
                        Values = salaryDict
                    },
                    new DongTaiChinh
                    {
                        Id = "other_expenses",
                        Name = "Chi phí khác",
                        Values = otherDict
                    },
                    new DongTaiChinh
                    {
                        Id = "profit",
                        Name = "Tổng lợi nhuận",
                        Values = profitDict
                    }
                };
                var baoCao = new BaoCaoTaiChinhDto
                {
                    Columns = kyBaos,
                    Rows = dongTaiChinh
                };

                return Ok(ApiResponse<BaoCaoTaiChinhDto>.Ok(baoCao));
            }
            else if (groupBy == "quarter")
            {
                // 1. xác định quý bắt đầu / kết thúc
                int startQuarter = ((dau.Month - 1) / 3) + 1;
                int endQuarter = ((cuoi.Month - 1) / 3) + 1;

                int year = dau.Year;

                // 2. helper lấy range quý
                (DateTime start, DateTime end) GetQuarterRange(int y, int q)
                {
                    int startMonth = (q - 1) * 3 + 1;
                    int endMonth = startMonth + 2;

                    var start = new DateTime(y, startMonth, 1);
                    var end = new DateTime(y, endMonth,
                        DateTime.DaysInMonth(y, endMonth));

                    return (start, end);
                }

                // 3. columns (Q1-2026, Q2-2026,...)
                var kyBaos = new List<KyBaoCao>();

                for (int q = startQuarter; q <= endQuarter; q++)
                {
                    kyBaos.Add(new KyBaoCao
                    {
                        Key = $"Q{q}-{year}",
                        Label = $"Q{q}-{year}"
                    });
                }

                // 4. dict
                var revenueDict = new Dictionary<string, decimal>();
                var cogsDict = new Dictionary<string, decimal>();
                var shippingDict = new Dictionary<string, decimal>();
                var salaryDict = new Dictionary<string, decimal>();
                var otherDict = new Dictionary<string, decimal>();
                var profitDict = new Dictionary<string, decimal>();

                // 5. loop từng quý
                for (int q = startQuarter; q <= endQuarter; q++)
                {
                    var key = $"Q{q}-{year}";
                    var (start, end) = GetQuarterRange(year, q);

                    var revenue = _context.DonHangs
                        .Where(x => x.NgayTao.HasValue &&
                                    x.NgayTao.Value >= start &&
                                    x.NgayTao.Value <= end)
                        .Sum(x => x.SoTienTra);

                    var cogs = _context.PhieuNhapKhos
                        .Where(x => x.NgayNhap.HasValue &&
                                    x.NgayNhap.Value >= start &&
                                    x.NgayNhap.Value <= end)
                        .Sum(x => x.DaThanhToanNcc);

                    //var shipping = revenue * 0.05m;
                    //var salary = revenue * 0.10m;
                    //var other = revenue * 0.03m;

                    var shipping = 0.0m;
                    var salary = 0.0m;
                    var other = 0.0m;

                    revenueDict[key] = (decimal)revenue;
                    cogsDict[key] = (decimal)cogs;
                    shippingDict[key] = (decimal)shipping;
                    salaryDict[key] = (decimal)salary;
                    otherDict[key] = (decimal)other;

                    profitDict[key] = (decimal)(revenue - cogs - shipping - salary - other);
                }

                // 6. result
                var dongTaiChinh = new List<DongTaiChinh>
                {
                    new DongTaiChinh
                    {
                        Id = "revenue",
                        Name = "Doanh thu bán hàng",
                        Values = revenueDict
                    },
                    new DongTaiChinh
                    {
                        Id = "cogs",
                        Name = "Giá vốn hàng bán",
                        Values = cogsDict
                    },
                    new DongTaiChinh
                    {
                        Id = "shipping",
                        Name = "Chi phí vận chuyển",
                        Values = shippingDict
                    },
                    new DongTaiChinh
                    {
                        Id = "salary",
                        Name = "Chi phí lương",
                        Values = salaryDict
                    },
                    new DongTaiChinh
                    {
                        Id = "other_expenses",
                        Name = "Chi phí khác",
                        Values = otherDict
                    },
                    new DongTaiChinh
                    {
                        Id = "profit",
                        Name = "Tổng lợi nhuận",
                        Values = profitDict
                    }
                };

                var baoCao = new BaoCaoTaiChinhDto
                {
                    Columns = kyBaos,
                    Rows = dongTaiChinh
                };

                return Ok(ApiResponse<BaoCaoTaiChinhDto>.Ok(baoCao));
            }
            else
            {
                // 1. tạo list năm
                var kyBaos = new List<KyBaoCao>();

                for (int year = dau.Year; year <= cuoi.Year; year++)
                {
                    kyBaos.Add(new KyBaoCao
                    {
                        Key = year.ToString(),
                        Label = year.ToString()
                    });
                }

                // 2. dict
                var revenueDict = new Dictionary<string, decimal>();
                var cogsDict = new Dictionary<string, decimal>();
                var shippingDict = new Dictionary<string, decimal>();
                var salaryDict = new Dictionary<string, decimal>();
                var otherDict = new Dictionary<string, decimal>();
                var profitDict = new Dictionary<string, decimal>();

                // 3. loop từng năm
                for (int year = dau.Year; year <= cuoi.Year; year++)
                {
                    var startDate = new DateTime(year, 1, 1);
                    var endDate = new DateTime(year, 12, 31);

                    var revenue = _context.DonHangs
                        .Where(x => x.NgayTao.HasValue &&
                                    x.NgayTao.Value >= startDate &&
                                    x.NgayTao.Value <= endDate)
                        .Sum(x => x.SoTienTra);

                    var cogs = _context.PhieuNhapKhos
                        .Where(x => x.NgayNhap.HasValue &&
                                    x.NgayNhap.Value >= startDate &&
                                    x.NgayNhap.Value <= endDate)
                        .Sum(x => x.DaThanhToanNcc);

                    //                var cogs = _context.PhieuNhapKhos
                    //.Join(
                    //    _context.ChiTietPhieuNhaps,
                    //    pnk => pnk.MaPhieuNhap,
                    //    ctnk => ctnk.MaPhieuNhap,
                    //    (pnk, ctnk) => new { pnk, ctnk }
                    //)
                    //.Where(x =>
                    //    x.pnk.NgayNhap.HasValue &&
                    //    x.pnk.NgayNhap.Value >= startDate &&
                    //    x.pnk.NgayNhap.Value <= endDate
                    //)
                    //.Sum(x => x.ctnk.ThanhTien);

                    //var shipping = revenue * 0.05m;
                    //var salary = revenue * 0.10m;
                    //var other = revenue * 0.03m;

                    var shipping = 0.0m;
                    var salary = 0.0m;
                    var other = 0.0m;

                    var key = year.ToString();

                    revenueDict[key] = (decimal)revenue;
                    cogsDict[key] = (decimal)cogs;
                    shippingDict[key] = (decimal)shipping;
                    salaryDict[key] = (decimal)salary;
                    otherDict[key] = (decimal)other;

                    profitDict[key] = (decimal)(revenue - cogs - shipping - salary - other);
                }

                // 4. result
                var dongTaiChinh = new List<DongTaiChinh>
                {
                    new DongTaiChinh
                    {
                        Id = "revenue",
                        Name = "Doanh thu bán hàng",
                        Values = revenueDict
                    },
                    new DongTaiChinh
                    {
                        Id = "cogs",
                        Name = "Giá vốn hàng bán",
                        Values = cogsDict
                    },
                    new DongTaiChinh
                    {
                        Id = "shipping",
                        Name = "Chi phí vận chuyển",
                        Values = shippingDict
                    },
                    new DongTaiChinh
                    {
                        Id = "salary",
                        Name = "Chi phí lương",
                        Values = salaryDict
                    },
                    new DongTaiChinh
                    {
                        Id = "other_expenses",
                        Name = "Chi phí khác",
                        Values = otherDict
                    },
                    new DongTaiChinh
                    {
                        Id = "profit",
                        Name = "Tổng lợi nhuận",
                        Values = profitDict
                    }
                };

                var baoCao = new BaoCaoTaiChinhDto
                {
                    Columns = kyBaos,
                    Rows = dongTaiChinh
                };

                return Ok(ApiResponse<BaoCaoTaiChinhDto>.Ok(baoCao));
            }

        }
       

    }
    
}
